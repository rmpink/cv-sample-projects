using UnityEngine;
using System.Collections;

public class CamBehaviour : MonoBehaviour {

    public float zoomSpeed = 1f;
	public float minDistance = 4f;
	public float maxDistance = 30f;
    private Camera disCam;

    void Start() {
        disCam = GetComponent<Camera>();
    }

	// Update is called once per frame
	void LateUpdate () {

        float scrl = Input.GetAxis("Mouse ScrollWheel");
        if ( (scrl > 0f && disCam.orthographicSize > minDistance) ||
             (scrl < 0f && disCam.orthographicSize < maxDistance) ) {
            disCam.orthographicSize -= zoomSpeed * scrl;
        } else if ( disCam.orthographicSize > maxDistance ) {
            disCam.orthographicSize = maxDistance;
        } else if ( disCam.orthographicSize < minDistance ) {
            disCam.orthographicSize = minDistance;
        }
	}
}
