using UnityEngine;
using System.Collections;
using Assets;
using System;
using TLVPackaging;

public enum Spin {
    Clockwise,
    CounterClockwise,
    Stationary
};

public class LidarSensor : Sensor {
    public bool showRay = true;
    public Spin spin = Spin.Clockwise;
    public float spinSpeed = 10f;

    public Transform rayOrigin;
    private LineRenderer myLR;


    // Use this for initialization
    void Start () {
        myLR = GetComponentInChildren<LineRenderer>();
        myLR.SetWidth( 0.1f, 0.1f );
        RaycastHit hit = new RaycastHit();
        Ray r = new Ray(rayOrigin.transform.position, rayOrigin.transform.forward);
        float rangeRead = maxRange;

        if ( Physics.Raycast(r, out hit, maxRange ) ) {
            DrawObjectAt( hit.distance );
            rangeRead = hit.distance;
        }

        myLR.SetPosition( 0, rayOrigin.transform.position + rayOrigin.transform.forward * minRange );
        myLR.SetPosition( 1, rayOrigin.transform.position + rayOrigin.transform.forward * rangeRead );
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

        myLR.SetPosition( 0, rayOrigin.transform.position + rayOrigin.transform.forward * minRange );
        myLR.SetPosition( 1, rayOrigin.transform.position + rayOrigin.transform.forward * rangeRead );

        switch ( spin ) {
        case Spin.Clockwise:
            transform.Rotate( transform.up, spinSpeed * Time.deltaTime );
            break;
        case Spin.CounterClockwise:
            transform.Rotate( transform.up, -spinSpeed * Time.deltaTime );
            break;
        }
    }

    public override SensorReading ProcessMessage(Message message)
    {

        // parse blah blah blah
        // message will contain serializeed struct containing bearing and reading

        // draw dummy point


        // value is in centimeters.


        bool somethingDetected = true;

        if(somethingDetected)
        {
            DrawObjectAt(1.5f); // 1.5 meters
        }

        // return reading and bearing
        return null;
    }



}
