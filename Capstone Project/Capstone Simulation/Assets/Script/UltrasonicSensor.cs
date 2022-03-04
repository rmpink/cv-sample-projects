using UnityEngine;
using Assets;
using System;
using TLVPackaging;
using System.Collections.Generic;
using System.Net;

public class UltrasonicSensor : Sensor {

    public float effectualAngle = 15f;
	public bool showRay = true;

	public Transform rayOrigin;
    private LineRenderer myLR;


    // Use this for initialization
    void Start () {
        myLR = GetComponentInChildren<LineRenderer>();
        myLR.SetWidth( Mathf.Abs((2 * minRange) / Mathf.Tan( effectualAngle/2 )), Mathf.Abs((2*maxRange) / Mathf.Tan(effectualAngle/2)) );
        RaycastHit hit = new RaycastHit();
        Ray r = new Ray(rayOrigin.transform.position, rayOrigin.transform.forward);
        float rangeRead = maxRange;

        if ( Physics.Raycast( r, out hit, maxRange ) ) {
            DrawObjectAt( hit.distance );
            rangeRead = hit.distance;
        }

        myLR.SetPosition( 0, rayOrigin.transform.position + rayOrigin.transform.forward * minRange);
        myLR.SetPosition( 1, rayOrigin.transform.position + rayOrigin.transform.forward * rangeRead);
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit = new RaycastHit();
        Ray r = new Ray(rayOrigin.transform.position, rayOrigin.transform.forward);
        float rangeRead = maxRange;

        if ( Physics.Raycast( r, out hit, maxRange ) ) {
            DrawObjectAt( hit.distance );
            rangeRead = hit.distance;
        }

        myLR.SetPosition( 0, rayOrigin.transform.position + rayOrigin.transform.forward * minRange);
        myLR.SetPosition( 1, rayOrigin.transform.position + rayOrigin.transform.forward * rangeRead);
    }



    public override SensorReading ProcessMessage(Message message)
    {

        // get the reading. NetworkToHostOrder changes the endien-ness.
        var reading = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(message.MessageData, 0));



       Debug.Log("ID: " + this.id + " reading: " + reading);
        if(reading != -1)   // -1 means no reading, so do nothing
        {
            // reading is in centimeters. DrawObjectAt takes meters.
      //      DrawObjectAt((float)reading / 100);
        }

        // message will just contain an int
        
        //bool somethingDetected = true;

        //if (somethingDetected)
        //{
        //    DrawObjectAt(2f); // 1.5 meters
        //}

        //// bearing will always be 0

        return null;

    }
}
