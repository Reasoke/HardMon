package com.ie.hardmon

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.Toast
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.ie.hardmon.models.Account
import com.ie.hardmon.utils.CookieHelper
import okhttp3.*
import org.json.JSONArray
import java.io.IOException

class AccountListActivity : AppCompatActivity() {

    private lateinit var recyclerView: RecyclerView
    private val client = OkHttpClient()
    private val accountList = mutableListOf<Account>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_account_list)

        recyclerView = findViewById(R.id.accountRecyclerView)
        recyclerView.layoutManager = LinearLayoutManager(this)
        recyclerView.adapter = AccountAdapter(accountList) { account ->
            val intent = Intent(this, DeviceListActivity::class.java)
            intent.putExtra("accountId", account.id)
            startActivity(intent)
//            Toast.makeText(this, "Pressed: ${account.name}", Toast.LENGTH_SHORT).show()
        }

        fetchAccounts()
    }

    private fun fetchAccounts() {
        val cookie = CookieHelper.readCookie(this)
        if (cookie == null) {
            Toast.makeText(this, "No cookie found", Toast.LENGTH_SHORT).show()
            return
        }

        val request = Request.Builder()
            .url("http://molly-busy-jolly.ngrok-free.app/api/accounts")
            .addHeader("Cookie", cookie)
            .addHeader("ngrok-skip-browser-warning", "true")
            .build()

        client.newCall(request).enqueue(object : Callback {
            override fun onFailure(call: Call, e: IOException) {
                runOnUiThread {
                    Toast.makeText(this@AccountListActivity, "Network error", Toast.LENGTH_SHORT).show()
                }
                Log.e("AccountFetch", "Error: ${e.message}")
            }

            override fun onResponse(call: Call, response: Response) {
                if (response.isSuccessful) {
                    val responseBody = response.body?.string()
                    val jsonArray = JSONArray(responseBody)

                    accountList.clear()
                    for (i in 0 until jsonArray.length()) {
                        val obj = jsonArray.getJSONObject(i)
                        val account = Account(
                            id = obj.getInt("id"),
                            name = obj.getString("name")
                        )
                        accountList.add(account)
                    }

                    runOnUiThread {
                        recyclerView.adapter?.notifyDataSetChanged()
                    }
                } else {
                    runOnUiThread {
                        Toast.makeText(this@AccountListActivity, "Server error", Toast.LENGTH_SHORT).show()
                    }
                    Log.e("AccountFetch", "Respond error: ${response.code}")
                }
            }
        })
    }
}