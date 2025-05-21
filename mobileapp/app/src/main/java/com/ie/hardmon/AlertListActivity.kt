package com.ie.hardmon

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Toast
import androidx.activity.result.ActivityResultLauncher
import androidx.activity.result.contract.ActivityResultContracts
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.ie.hardmon.models.Alert
import com.ie.hardmon.utils.CookieHelper
import okhttp3.*
import org.json.JSONArray
import java.io.IOException

class AlertListActivity : AppCompatActivity() {

    private lateinit var recyclerView: RecyclerView
    private val client = OkHttpClient()
    private val alertList = mutableListOf<Alert>()
    private lateinit var detailLauncher: ActivityResultLauncher<Intent>

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_alert_list)

        val accountId = intent.getIntExtra("accountId", -1)
        val sensorId = intent.getIntExtra("sensorId", -1)

        if (accountId == -1 && sensorId == -1) {
            Toast.makeText(this, "Not enough values", Toast.LENGTH_SHORT).show()
        }

        recyclerView = findViewById(R.id.alertRecyclerView)
        recyclerView.layoutManager = LinearLayoutManager(this)
//        recyclerView.adapter = AlertAdapter(alertList) { alert ->
//            val intent = Intent(this, AlertDetailActivity::class.java)
//            intent.putExtra("accountId", accountId)
//            intent.putExtra("sensorId", sensorId)
//            intent.putExtra("alertId", alert.id)
//            startActivity(intent)
////            Toast.makeText(this, "Выбрано: ${alert.message}", Toast.LENGTH_SHORT).show()
//        }

        detailLauncher = registerForActivityResult(ActivityResultContracts.StartActivityForResult()) { result ->
            if (result.resultCode == RESULT_OK) {
                fetchAlerts(accountId, sensorId)
            }
        }

        recyclerView.adapter = AlertAdapter(alertList) { alert ->
            val intent = Intent(this, AlertDetailActivity::class.java)
            intent.putExtra("accountId", accountId)
            intent.putExtra("sensorId", sensorId)
            intent.putExtra("alertId", alert.id)
            detailLauncher.launch(intent)
        }

        fetchAlerts(accountId, sensorId)
    }

    private fun fetchAlerts(accountId: Int, sensorId: Int) {
        val cookie = CookieHelper.readCookie(this)
        if (cookie == null) {
            Toast.makeText(this, "No cookie found", Toast.LENGTH_SHORT).show()
            return
        }


        val request = Request.Builder()
            .url("http://molly-busy-jolly.ngrok-free.app/api/accounts/$accountId/sensors/$sensorId/alert")
            .addHeader("Cookie", cookie)
            .addHeader("ngrok-skip-browser-warning", "true")
            .build()

        client.newCall(request).enqueue(object : Callback {
            override fun onFailure(call: Call, e: IOException) {
                runOnUiThread {
                    Toast.makeText(this@AlertListActivity, "Network error", Toast.LENGTH_SHORT).show()
                }
                e.printStackTrace()
            }

            override fun onResponse(call: Call, response: Response) {
                val body = response.body?.string()
                if (response.isSuccessful && body != null) {
                    val jsonArray = JSONArray(body)
                    alertList.clear()

                    for (i in 0 until jsonArray.length()) {
                        val obj = jsonArray.getJSONObject(i)
                        val alert = Alert(
                            id = obj.getInt("id"),
                            created = obj.getString("created"),
                            message = obj.getString("alertMessage"),
                            isChecked = obj.getBoolean("isChecked")
                        )
                        alertList.add(alert)
                    }

                    runOnUiThread {
                        recyclerView.adapter?.notifyDataSetChanged()
                    }
                } else {
                    runOnUiThread {
                        Toast.makeText(this@AlertListActivity, "Failed to load alerts", Toast.LENGTH_SHORT).show()
                    }
                }
            }
        })
    }
}