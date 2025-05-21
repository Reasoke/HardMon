package com.ie.hardmon

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.Toast
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.ie.hardmon.models.Sensor
import com.ie.hardmon.utils.CookieHelper
import okhttp3.*
import org.json.JSONArray
import java.io.IOException

class SensorListActivity : AppCompatActivity() {

    private lateinit var recyclerView: RecyclerView
    private val client = OkHttpClient()
    private val sensorList = mutableListOf<Sensor>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_sensor_list)

        val accountId = intent.getIntExtra("accountId", -1)
        val deviceId = intent.getIntExtra("deviceId", -1)



        if (accountId == -1 ) {
            Toast.makeText(this, "Invalid accountId", Toast.LENGTH_SHORT).show()
            finish()
            return
        }
        if (deviceId == -1) {
            Toast.makeText(this, "Invalid deviceId", Toast.LENGTH_SHORT).show()
            finish()
            return
        }

        recyclerView = findViewById(R.id.sensorRecyclerView)
        recyclerView.layoutManager = LinearLayoutManager(this)
        recyclerView.adapter = SensorAdapter(sensorList) { sensor ->
            val intent = Intent(this, AlertListActivity::class.java)
            intent.putExtra("accountId", accountId)
            intent.putExtra("sensorId", sensor.id)
            startActivity(intent)
//            Toast.makeText(this, "Pressed: ${sensor.type}", Toast.LENGTH_SHORT).show()
        }

        fetchSensors(accountId, deviceId)
    }

    private fun fetchSensors(accountId: Int, deviceId: Int) {
        val cookie = CookieHelper.readCookie(this)
        if (cookie == null) {
            Toast.makeText(this, "No cookie found", Toast.LENGTH_SHORT).show()
            return
        }

        val request = Request.Builder()
            .url("http://molly-busy-jolly.ngrok-free.app/api/accounts/$accountId/devices/$deviceId/sensors")
            .addHeader("ngrok-skip-browser-warning", "true")
            .addHeader("Cookie", cookie)
            .build()

        client.newCall(request).enqueue(object : Callback {
            override fun onFailure(call: Call, e: IOException) {
                runOnUiThread {
                    Toast.makeText(this@SensorListActivity, "Network error", Toast.LENGTH_SHORT).show()
                }
            }

            override fun onResponse(call: Call, response: Response) {
                if (response.isSuccessful) {
                    val body = response.body?.string()
                    val jsonArray = JSONArray(body)

                    sensorList.clear()
                    for (i in 0 until jsonArray.length()) {
                        val obj = jsonArray.getJSONObject(i)
                        val sensor = Sensor(
                            id = obj.getInt("id"),
                            type = obj.getString("sensorType"),
                            config = obj.getString("config")
                        )
                        sensorList.add(sensor)
                    }

                    runOnUiThread {
                        recyclerView.adapter?.notifyDataSetChanged()
                    }
                } else {
                    runOnUiThread {
                        Toast.makeText(this@SensorListActivity, "Server error", Toast.LENGTH_SHORT).show()
                    }
                }
            }
        })
    }
}