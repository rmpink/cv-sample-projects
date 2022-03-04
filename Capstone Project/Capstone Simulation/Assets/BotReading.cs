using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class BotReading
    {
        public float botX { get; private set; }
        public float botY { get; private set; }

        public SensorReading sensorReading { get; set; }

        public BotReading(float x, float y, SensorReading reading)
        {
            botX = x;
            botY = y;
            sensorReading = reading;
        }
    }
}
