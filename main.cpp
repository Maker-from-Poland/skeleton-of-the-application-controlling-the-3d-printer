

#include<Arduino.h>


const int LED_PIN = 13;

void setup() {
  pinMode(LED_PIN, OUTPUT);
  Serial.begin(9600);
}

void loop() {
  if (Serial.available() > 0) {
    String data = Serial.readStringUntil('\n');
    data.trim();

    if (data == "on") {
      digitalWrite(LED_PIN, HIGH);
      Serial.println("ok");
    } else if (data == "off") {
      digitalWrite(LED_PIN, LOW);
      Serial.println("ok");
    } else {
      Serial.println("fail argument");
    }
  }
}
