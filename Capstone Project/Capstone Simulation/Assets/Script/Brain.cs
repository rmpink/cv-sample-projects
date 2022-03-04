using UnityEngine;
using System.Collections;
using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using TLVPackaging;
using CapstoneCommunications;
using System.IO.Ports;


/*
TODO
- track current X and Y
- store sensor readings
- send sensor raedings to bots it finds.
- store friends list
*/

/// <summary>
/// This class mimics the Raspberry Pi of the bot. 
/// </summary>
public class Brain : MonoBehaviour
{
    /// <summary>
    /// All the sensors on the bot
    /// </summary>
    private List<Sensor> sensors { get; set; }
    
    private SerialIO serialIO { get; set; }
    
    private List<BotReading> botReadings;


    private float currentX { get; set; }
    private float currentY { get; set; }


    //    private List<Brain> OtherBots { get; set; } // we have to figure this stuff out

    // what does the PI store in its point cloud?

    // sensor readings (in what format?)

    // TLV messages? Probably overkill


    // Use this for initialization
    void Start ()
    {
        this.sensors = GetComponentsInChildren<Sensor>().ToList();
        serialIO = new SerialIO();
        serialIO.Init("COM4", 9600);
    }


 
    


    // Update is called once per frame
    void Update ()
    { 
        int readCount = 0;

        Message message = null;

		//test


        //  //for (int i = 0; i < 50; i++)
        //  //{
        //  message = serialIO.ReadMessage();
        //  if (message == null)
        //  {
        //      message = serialIO.ReadMessage();
        //  }
        //  else
        //  {
        ////      Debug.Log("Only had to do 1 read");
        //  }

        //  if (message != null)
        //  {

        //      var messages = TLVPackage.UnpackTLVMessages(message);
        //      var leftUltraSonic = this.GetSensorById(2);
        //      ProcessMessage(leftUltraSonic, messages[0]);

        //  }



        //}

        while ((message = serialIO.ReadMessage()) != null)
        {

            readCount++;
            // message is of type SensorReadings. We unpack the message
            // and pass each sensor reading to its respective sensor
            // to interpret.

            var messages = TLVPackage.UnpackTLVMessages(message);


            // 1 message is left

            //var leftUltraSonic = this.GetSensorById(2);
            //ProcessMessage(leftUltraSonic, messages[0]);


            // 2 is right
            var rightUltraSonic = this.GetSensorById(3);
            ProcessMessage(rightUltraSonic, messages[1]);







            //var lidar = this.GetSensorById(1);
            //ProcessMessage(lidar, null);

            //var leftUltraSonic = this.GetSensorById(2);
            //ProcessMessage(leftUltraSonic, null);

            //var rightUltraSonic = this.GetSensorById(3);
            //ProcessMessage(rightUltraSonic, null);

            //var backUltraSonic = this.GetSensorById(5);
            //ProcessMessage(backUltraSonic, null);
        }
        Debug.Log("ReadCount: " + readCount);
    }



    private BotReading ProcessMessage(Sensor sensor, Message message)
    {
        if (sensor == null)
            throw new ArgumentNullException("sensor");
        //if (message == null)
        //    throw new ArgumentNullException("message");

        SensorReading sensorReading = sensor.ProcessMessage(message);  
        return new BotReading(this.currentX, this.currentY, sensorReading);
    }

    /// <summary>
    /// Gets a sensor on the bot that has the given ID. 
    /// </summary>
    /// <param name="id">The sensor ID</param>
    /// <returns>The sensor</returns>
    private Sensor GetSensorById(int id)
    {
        return this.sensors.SingleOrDefault(sensor => sensor.id == id);
    }  
}
