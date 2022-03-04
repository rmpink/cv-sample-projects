using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvironmentBuilder : MonoBehaviour {

	public List<Transform> environmentObjects;
	public Transform floor = null;
	public Transform active = null;

	// Use this for initialization
	void Start () {
		foreach (Transform e in environmentObjects) {
			if ( e.gameObject.GetComponent<HoverableObject>() == null ) {
				e.gameObject.AddComponent<HoverableObject>();
			}
		}
	}
    
	public void setActive( string obj ) {
		foreach (Transform t in environmentObjects) {
			if ( t.name == obj ) {
				active = t;
			} else {
				t.gameObject.GetComponent<HoverableObject>().setInactive();
			}
		}
	}

    public void SetShowSensorFOV( bool val ) {
        Camera.main.cullingMask ^= (1 << LayerMask.NameToLayer( "RayVisible" ));
    }

    public void SetShowGeometry( bool val ) {
        Camera.main.cullingMask ^= (1 << LayerMask.NameToLayer( "EnvironmentVisible" ));
    }
}
