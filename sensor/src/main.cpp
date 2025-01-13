#include <WiFi.h>
#include <HTTPClient.h>
#include <DHT.h>
#include <EEPROM.h>
#include <ArduinoJson.h>

// Wi-Fi credentials
const char *ssid = "Wokwi-GUEST";
const char *password = "";

// Server URLs
const char *serverURL = "http://molly-busy-jolly.ngrok-free.app/api/";

// DHT sensor setup
#define DHTPIN 15     // Pin where the sensor is connected
#define DHTTYPE DHT22 // Sensor type: DHT11 or DHT22
DHT dht(DHTPIN, DHTTYPE);

int buttonPin = 22;
int ledPin = 23;
boolean buttonState;

String authToken;
unsigned long lastDataSendTime = 0;

float minTemperature = -INFINITY;
float maxTemperature = INFINITY;
float minHumidity = -INFINITY;
float maxHumidity = INFINITY;
int intervalSeconds = 60;
boolean hasConfig = false;

// void getConfigFromServer()
// {
//   HTTPClient http;
//   String url = String(serverURL) + "sensors/" + authToken + "/details";
//   http.begin(url);
//   http.useHTTP10(true);
//   http.addHeader("ngrok-skip-browser-warning", "loath it");
//   Serial.println(url);
//   int httpResponseCode = http.GET();
//   if (httpResponseCode > 0)
//   {
//     String response = http.getString();
//     Serial.println("Server response: " + response);
//     parseServerResponse(response);
//   }
//   else
//   {
//     Serial.println("Failed to connect to server.");
//   }
//   http.end();
// }

void setup()
{
  Serial.begin(115200);
  pinMode(ledPin, OUTPUT);
  pinMode(buttonPin, INPUT);

  // Initialize Wi-Fi
  WiFi.begin(ssid, password);
  Serial.print("Connecting to Wi-Fi");

  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }

  Serial.println("Connected!");

  // Check and initialize token
  authToken = String(ESP.getEfuseMac());
  Serial.println(authToken);

  dht.begin();
  buttonState = digitalRead(buttonPin);
}

void sendDataToServer(String endpoint, String payload)
{
  HTTPClient http;
  String url = String(serverURL) + "sensors/" + authToken + endpoint;

  http.begin(url);
  http.addHeader("Content-Type", "application/json");
  int httpResponseCode = http.POST(payload);

  if (httpResponseCode > 0)
  {
    String response = http.getString();
    Serial.println("Server response: " + response);
  }
  else
  {
    Serial.println("Error sending request to " + endpoint);
  }

  http.end();
}

void parseServerResponse(String response)
{
  StaticJsonDocument<256> doc;
  DeserializationError error = deserializeJson(doc, response);

  if (!error)
  {
    if (doc.containsKey("minTemperature"))
      minTemperature = doc["minTemperature"].as<float>();
    if (doc.containsKey("maxTemperature"))
      maxTemperature = doc["maxTemperature"].as<float>();
    if (doc.containsKey("minHumidity"))
      minHumidity = doc["minHumidity"].as<float>();
    if (doc.containsKey("maxHumidity"))
      maxHumidity = doc["maxHumidity"].as<float>();
    if (doc.containsKey("interval_seconds"))
      intervalSeconds = doc["interval_seconds"].as<int>() * 1000;
  }
  else
  {
    Serial.println("Failed to parse server response.");
  }
}

void connectToServer()
{
  HTTPClient http;
  String url = String(serverURL) + "sensors/" + authToken + "/5/connect";

  http.begin(url);
  http.useHTTP10(true);
  http.addHeader("ngrok-skip-browser-warning", "loath it");
  Serial.println(url);
  int httpResponseCode = http.GET();

  if (httpResponseCode > 0)
  {
    digitalWrite(ledPin, HIGH);

    String response = http.getString();
    Serial.println("Server response: " + response);
    if (!response.isEmpty())
    {
      hasConfig = true;
    }
    parseServerResponse(response);
    Serial.println(hasConfig);
  }
  else
  {
    Serial.println("Failed to connect to server.");
  }
  http.end();
}

void loop()
{
  if (WiFi.status() != WL_CONNECTED)
  {
    Serial.println("Wi-Fi disconnected!");
    delay(1000);
    return;
  }

  boolean newState = digitalRead(buttonPin);
  if (newState != buttonState)
  {
    buttonState = newState;
    Serial.println("Button pressed! New state: " + String(buttonState));
    connectToServer();
  }

  float temperature = dht.readTemperature();
  float humidity = dht.readHumidity();

  Serial.println("Temperature: " + String(temperature) + "°C");
  Serial.println("Humidity: " + String(humidity) + "%");

  if (isnan(temperature) || isnan(humidity))
  {
    Serial.println("Failed to read from DHT sensor!");
    return;
  }

  String jsonPayload = "{\"temperature\":" + String(temperature) + ",\"humidity\":" + String(humidity) + "}";

  if (hasConfig && ((temperature > maxTemperature && maxTemperature != INFINITY) || (temperature < minTemperature && minTemperature != -INFINITY)))
  {
    Serial.println("Alert! Sending data to /alert endpoint.");
    String alertMessage = "Temperature is out of norm: " + String(temperature);
    sendDataToServer("/alert", "{\"alertMessage\": \"" + alertMessage + "\"}");
  }
  else if (hasConfig && ((humidity > maxHumidity && maxHumidity != INFINITY) || (humidity < minHumidity && minHumidity != -INFINITY)))
  {
    Serial.println("Alert! Sending data to /alert endpoint.");
    String alertMessage = "Humidity is out of norm: " + String(humidity);
    sendDataToServer("/alert", "{\"alertMessage\": \"" + alertMessage + "\"}");
  }
  else if (millis() - lastDataSendTime >= intervalSeconds )
  {
    Serial.println("Sending data to /data endpoint.");
    sendDataToServer("/data", jsonPayload);
    lastDataSendTime = millis();
  }
  delay(1000);
}
