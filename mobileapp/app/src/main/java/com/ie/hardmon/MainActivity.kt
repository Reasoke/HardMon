package com.ie.hardmon

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import com.ie.hardmon.utils.CookieHelper
import okhttp3.*
import okhttp3.MediaType.Companion.toMediaType
import java.io.File
import java.io.IOException

class MainActivity : AppCompatActivity() {
    private lateinit var emailInput: EditText
    private lateinit var passwordInput: EditText
    private val client = OkHttpClient()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        emailInput = findViewById(R.id.emailInput)
        passwordInput = findViewById(R.id.passwordInput)

        findViewById<Button>(R.id.loginButton).setOnClickListener {
            loginUser(emailInput.text.toString(), passwordInput.text.toString())
        }

        findViewById<TextView>(R.id.loginTitle).setOnClickListener {
            startActivity(Intent(this, RegisterActivity::class.java))
        }
    }

    private fun loginUser(email: String, password: String) {
        val json = """{"email":"$email","password":"$password"}"""
        val body = RequestBody.create("application/json".toMediaType(), json)
        val request = Request.Builder()
            .url("http://molly-busy-jolly.ngrok-free.app/api/profile/login")
            .addHeader("ngrok-skip-browser-warning", "true")
            .addHeader("Content-Type", "application/json")
            .post(body)
            .build()

        client.newCall(request).enqueue(object: Callback {
            override fun onResponse(call: Call, response: Response) {
                if (response.isSuccessful) {
                    val cookies = response.headers("Set-Cookie").joinToString("; ")
                    CookieHelper.saveCookie(this@MainActivity, cookies)
//                    saveCookiesToFile(cookies)
                    startActivity(Intent(this@MainActivity, AccountListActivity::class.java))
                } else {
                    runOnUiThread {
                        Toast.makeText(this@MainActivity, "Невірна пошта або пароль", Toast.LENGTH_SHORT).show()
                    }
                }
            }

            override fun onFailure(call: Call, e: IOException) {
                e.printStackTrace()
            }
        })
    }

    private fun saveCookiesToFile(cookies: String) {
        val file = File(filesDir, "cookies.txt")
        file.writeText(cookies)
    }
}