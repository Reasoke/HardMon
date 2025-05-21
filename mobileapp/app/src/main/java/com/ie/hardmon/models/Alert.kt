package com.ie.hardmon.models

data class Alert(
    val id: Int,
    val created: String,
    val message: String,
    val isChecked: Boolean
)