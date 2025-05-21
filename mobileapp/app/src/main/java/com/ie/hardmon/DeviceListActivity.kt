package com.ie.hardmon

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Toast
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.ie.hardmon.models.Device
import com.ie.hardmon.utils.CookieHelper
import okhttp3.*
import org.json.JSONArray
import java.io.IOException


class DeviceListActivity : AppCompatActivity() {
    private lateinit var recyclerView: RecyclerView
    private val devices = mutableListOf<Device>()
    private val client = OkHttpClient()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_device_list)

        val accountId = intent.getIntExtra("accountId", -1)
        if (accountId == -1) {
            Toast.makeText(this, "Error while processing accountId", Toast.LENGTH_SHORT).show()
            finish()
            return
        }

        recyclerView = findViewById(R.id.deviceRecyclerView)
        recyclerView.layoutManager = LinearLayoutManager(this)
        recyclerView.adapter = DeviceAdapter(devices) { device ->
            val intent = Intent(this, SensorListActivity::class.java)
            intent.putExtra("accountId", accountId)
            intent.putExtra("deviceId", device.id)
            startActivity(intent)
//            Toast.makeText(this, "Pressed: ${device.name}", Toast.LENGTH_SHORT).show()
        }

        fetchDevices(accountId)
    }

    private fun fetchDevices(accountId: Int) {
        val cookie = CookieHelper.readCookie(this)
        if (cookie == null) {
            Toast.makeText(this, "No cookie found", Toast.LENGTH_SHORT).show()
            return
        }

        val request = Request.Builder()
            .url("http://molly-busy-jolly.ngrok-free.app/api/accounts/$accountId/devices")
            .addHeader("ngrok-skip-browser-warning", "true")
            .addHeader("Cookie", cookie)
            .build()

        client.newCall(request).enqueue(object : Callback {
            override fun onResponse(call: Call, response: Response) {
                val body = response.body?.string()
                if (response.isSuccessful && body != null) {
                    val jsonArray = JSONArray(body)
                    for (i in 0 until jsonArray.length()) {
                        val obj = jsonArray.getJSONObject(i)
                        val device = Device(
                            id = obj.getInt("id"),
                            name = obj.getString("name")
                        )
                        devices.add(device)
                    }

                    runOnUiThread {
                        recyclerView.adapter?.notifyDataSetChanged()
                    }
                } else {
                    runOnUiThread {
                        Toast.makeText(this@DeviceListActivity, "Failed to load devices", Toast.LENGTH_SHORT).show()
                    }
                }
            }

            override fun onFailure(call: Call, e: IOException) {
                e.printStackTrace()
            }
        })
    }
}