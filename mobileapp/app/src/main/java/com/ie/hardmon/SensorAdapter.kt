package com.ie.hardmon

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.ie.hardmon.models.Sensor

class SensorAdapter(
    private val sensors: List<Sensor>,
    private val onClick: (Sensor) -> Unit
) : RecyclerView.Adapter<SensorAdapter.SensorViewHolder>() {

    class SensorViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val type: TextView = view.findViewById(R.id.sensorTypeText)
        val config: TextView = view.findViewById(R.id.sensorConfigText)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): SensorViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.item_sensor, parent, false)
        return SensorViewHolder(view)
    }

    override fun onBindViewHolder(holder: SensorViewHolder, position: Int) {
        val sensor = sensors[position]
        holder.type.text = "Type: ${sensor.type}"
        holder.config.text = "Config: ${sensor.config}"
        holder.itemView.setOnClickListener { onClick(sensor) }
    }

    override fun getItemCount(): Int = sensors.size
}