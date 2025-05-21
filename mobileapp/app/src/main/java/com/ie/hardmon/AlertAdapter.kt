package com.ie.hardmon

import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.ie.hardmon.models.Alert

class AlertAdapter(
    private val alerts: List<Alert>,
    private val onClick: (Alert) -> Unit
) : RecyclerView.Adapter<AlertAdapter.AlertViewHolder>() {

    class AlertViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val messageText: TextView = view.findViewById(R.id.alertMessageText)
        val statusText: TextView = view.findViewById(R.id.alertStatusText)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): AlertViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.item_alert, parent, false)
        return AlertViewHolder(view)
    }

    override fun onBindViewHolder(holder: AlertViewHolder, position: Int) {
        val alert = alerts[position]
        holder.messageText.text = alert.message
        holder.statusText.text = if (alert.isChecked) "Checked" else "New"
        holder.itemView.setOnClickListener { onClick(alert) }
    }

    override fun getItemCount(): Int = alerts.size
}