package com.ie.hardmon.utils

import android.content.Context
import java.io.File
import java.io.IOException

object CookieHelper {

    private const val COOKIE_FILE_NAME = "cookie.txt"

    fun saveCookie(context: Context, cookie: String) {
        try {
            val file = File(context.filesDir, COOKIE_FILE_NAME)
            file.writeText(cookie)
        } catch (e: IOException) {
            e.printStackTrace()
        }
    }

    fun readCookie(context: Context): String? {
        return try {
            val file = File(context.filesDir, COOKIE_FILE_NAME)
            if (file.exists()) {
                file.readText()
            } else {
                null
            }
        } catch (e: IOException) {
            e.printStackTrace()
            null
        }
    }

    fun clearCookie(context: Context) {
        try {
            val file = File(context.filesDir, COOKIE_FILE_NAME)
            if (file.exists()) {
                file.delete()
            }
        } catch (e: IOException) {
            e.printStackTrace()
        }
    }
}
