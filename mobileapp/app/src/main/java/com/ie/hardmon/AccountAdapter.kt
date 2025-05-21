package com.ie.hardmon

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.ie.hardmon.models.Account

class AccountAdapter(
    private val accounts: List<Account>,
    private val onClick: (Account) -> Unit
) : RecyclerView.Adapter<AccountAdapter.AccountViewHolder>() {

    class AccountViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val accountNameText: TextView = view.findViewById(R.id.accountNameText)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): AccountViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.item_account, parent, false)
        return AccountViewHolder(view)
    }

    override fun onBindViewHolder(holder: AccountViewHolder, position: Int) {
        val account = accounts[position]
        holder.accountNameText.text = account.name
        holder.itemView.setOnClickListener { onClick(account) }
    }

    override fun getItemCount() = accounts.size
}