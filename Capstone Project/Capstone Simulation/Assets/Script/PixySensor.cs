using UnityEngine;
using System.Collections;
using Assets;
using System;
using TLVPackaging;

public class PixySensor : Sensor {

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override SensorReading ProcessMessage(Message message)
    {
        throw new NotImplementedException();
    }
}
