using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLVPackaging;
using UnityEngine;

namespace Assets
{

    public abstract class Sensor : MonoBehaviour
    {
        /// <summary>
        /// The ID of the sensor. Should be unique on the bot
        /// </summary>
        public int id;

        /// <summary>
        /// The minimum range of the sensor (in meters)
        /// </summary>
        public float minRange;

        /// <summary>
        /// The max range of the sensor (in meters)
        /// </summary>
        public float maxRange;

        // TBD. Pixy and lidar will have a bearing.
        public float bearing;

        /// <summary>
        /// The object to spawn when the sensor detects something
        /// </summary>
        public GameObject spawnObject;

        /// <summary>
        /// The bot the sensor belongs to
        /// </summary>
        protected Brain bot
        {
            get
            {
                return this.transform.root.GetComponentInChildren<Brain>();
            }
        }
        
        /// <summary>
        /// Each sensor will be responsibile for processing the TLV message
        /// containing its sensor readings/bearing
        /// </summary>
        /// <param name="message">The message (sent by the teensy)</param>
        /// <returns>A sensor reading the bot can store in its point cloud</returns>
        public abstract SensorReading ProcessMessage(Message message);

        /// <summary>
        /// Draws the sensors "spawn object" X meters away from where it is currently pointing.
        /// </summary>
        /// <param name="meters">The number of meters away to draw the spawn object</param>
        protected void DrawObjectAt(float meters)
        {
            var offsetVector = transform.forward * meters;
            
            // draw an object X meters away from the sensors center
            var point = Instantiate(spawnObject.transform,
                transform.position + offsetVector,
                transform.rotation) as GameObject;
        }
    }
}
