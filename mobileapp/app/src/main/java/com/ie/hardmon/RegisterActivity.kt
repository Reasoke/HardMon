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
import okhttp3.RequestBody.Companion.toRequestBody
import java.io.File
import java.io.IOException

class RegisterActivity : AppCompatActivity() {
    private lateinit var nameInput: EditText
    private lateinit var emailInput: EditText
    private lateinit var passwordInput: EditText
    private val client = OkHttpClient()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_register)

        nameInput = findViewById(R.id.nameInput)
        emailInput = findViewById(R.id.emailInput)
        passwordInput = findViewById(R.id.passwordInput)

        findViewById<Button>(R.id.registerButton).setOnClickListener {
            val name = nameInput.text.toString()
            val email = emailInput.text.toString()
            val password = passwordInput.text.toString()

            if (name.isBlank() || email.isBlank() || password.isBlank()) {
                Toast.makeText(this, "Fill all fields", Toast.LENGTH_SHORT).show()
            } else {
                registerUser(name, email, password)
            }
        }

        findViewById<TextView>(R.id.loginText).setOnClickListener {
            startActivity(Intent(this, MainActivity::class.java))
            finish()
        }
    }

    private fun registerUser(name: String, email: String, password: String) {
        val json = """{"name":"$name","email":"$email","password":"$password"}"""
        val body = json.toRequestBody("application/json".toMediaType())

        val request = Request.Builder()
            .url("http://molly-busy-jolly.ngrok-free.app/api/profile/register")
            .addHeader("ngrok-skip-browser-warning", "true")
            .addHeader("Content-Type", "application/json")
            .post(body)
            .build()

        client.newCall(request).enqueue(object : Callback {
            override fun onResponse(call: Call, response: Response) {
                if (response.isSuccessful) {
                    val cookies = response.headers("Set-Cookie").joinToString("; ")
                    CookieHelper.saveCookie(this@RegisterActivity, cookies)
//                    saveCookiesToFile(cookies)

                    runOnUiThread {
                        Toast.makeText(this@RegisterActivity, "Registration is done", Toast.LENGTH_SHORT).show()
                        startActivity(Intent(this@RegisterActivity, AccountListActivity::class.java))
                        finish()
                    }
                } else {
                    runOnUiThread {
                        Toast.makeText(this@RegisterActivity, "Registration error", Toast.LENGTH_SHORT).show()
                    }
                }
            }

            override fun onFailure(call: Call, e: IOException) {
                runOnUiThread {
                    Toast.makeText(this@RegisterActivity, "Network error", Toast.LENGTH_SHORT).show()
                }
            }
        })
    }

    private fun saveCookiesToFile(cookies: String) {
        val file = File(filesDir, "cookies.txt")
        file.writeText(cookies)
    }
}