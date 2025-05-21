package com.ie.hardmon

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Button
import android.widget.CheckBox
import android.widget.TextView
import android.widget.Toast
import com.ie.hardmon.utils.CookieHelper
import okhttp3.*
import okhttp3.MediaType.Companion.toMediaTypeOrNull
import okhttp3.RequestBody.Companion.toRequestBody
import org.json.JSONException
import org.json.JSONObject
import java.io.IOException

class AlertDetailActivity : AppCompatActivity() {

    private lateinit var alertMessageText: TextView
    private lateinit var createdText: TextView
    private lateinit var isCheckedCheckbox: CheckBox
    private lateinit var saveButton: Button

    private val client = OkHttpClient()

    private var accountId = -1
    private var sensorId = -1
    private var alertId = -1

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_alert_detail)

        alertMessageText = findViewById(R.id.alertMessageText)
        createdText = findViewById(R.id.createdText)
        isCheckedCheckbox = findViewById(R.id.isCheckedCheckbox)
        saveButton = findViewById(R.id.saveButton)

        accountId = intent.getIntExtra("accountId", -1)
        sensorId = intent.getIntExtra("sensorId", -1)
        alertId = intent.getIntExtra("alertId", -1)

        if (accountId == -1 || sensorId == -1 || alertId == -1) {
            Toast.makeText(this, "Incorrect values", Toast.LENGTH_SHORT).show()
            finish()
            return
        }

        fetchAlertDetails()

        saveButton.setOnClickListener {
            updateAlertIsChecked()
        }
    }

    private fun fetchAlertDetails() {
        val cookie = CookieHelper.readCookie(this)
        if (cookie == null) {
            Toast.makeText(this, "No cookie found", Toast.LENGTH_SHORT).show()
            finish()
            return
        }

        val request = Request.Builder()
            .url("http://molly-busy-jolly.ngrok-free.app/api/accounts/$accountId/sensors/$sensorId/alert/$alertId")
            .addHeader("Cookie", cookie)
            .addHeader("ngrok-skip-browser-warning", "true")
            .build()

        client.newCall(request).enqueue(object: Callback {
            override fun onFailure(call: Call, e: IOException) {
                runOnUiThread {
                    Toast.makeText(this@AlertDetailActivity, "Network error", Toast.LENGTH_SHORT).show()
                }
            }

            override fun onResponse(call: Call, response: Response) {
                if (!response.isSuccessful) {
                    runOnUiThread {
                        Toast.makeText(this@AlertDetailActivity, "Server error", Toast.LENGTH_SHORT).show()
                    }
                    return
                }
                val body = response.body?.string()
                val json = JSONObject(body)

                try {
                    val alertMessage = json.getString("alertMessage")
                    val created = json.getString("created")
                    val isChecked = json.getBoolean("isChecked")

                    runOnUiThread {
                        alertMessageText.text = alertMessage
                        createdText.text = "Created: ${created.replace('T', ' ').substring(0, 19)}"
                        isCheckedCheckbox.isChecked = isChecked
                    }

                } catch (e: JSONException) {
                    e.printStackTrace()
                    runOnUiThread {
                        Toast.makeText(this@AlertDetailActivity, "Error data parsing", Toast.LENGTH_SHORT).show()
                    }
                }
            }
        })
    }

    private fun updateAlertIsChecked() {
        val cookie = CookieHelper.readCookie(this)
        if (cookie == null) {
            Toast.makeText(this, "No cookie found", Toast.LENGTH_SHORT).show()
            return
        }

        val jsonBody = JSONObject()
        jsonBody.put("IsChecked", isCheckedCheckbox.isChecked)

        val requestBody = jsonBody.toString().toRequestBody("application/json".toMediaTypeOrNull())

        val request = Request.Builder()
            .url("http://molly-busy-jolly.ngrok-free.app/api/accounts/$accountId/sensors/$sensorId/alert/$alertId")
            .post(requestBody)
            .addHeader("Cookie", cookie)
            .addHeader("ngrok-skip-browser-warning", "true")
            .build()

        client.newCall(request).enqueue(object: Callback {
            override fun onFailure(call: Call, e: IOException) {
                runOnUiThread {
                    Toast.makeText(this@AlertDetailActivity, "Network error while saving data", Toast.LENGTH_SHORT).show()
                }
            }

            override fun onResponse(call: Call, response: Response) {
                runOnUiThread {
                    if (response.isSuccessful) {
                        Toast.makeText(this@AlertDetailActivity, "Saved", Toast.LENGTH_SHORT).show()
                        setResult(RESULT_OK)
                        finish()
                    } else {
                        Toast.makeText(this@AlertDetailActivity, "Server error while saving data", Toast.LENGTH_SHORT).show()
                    }
                }
            }
        })
    }
}