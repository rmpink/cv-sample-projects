using UnityEngine;
using System.Collections;

public class SoftbotMovement : MonoBehaviour {

	public float forwardSpeed = 1f;
	public float rotateSpeed = 1f;

	private bool fwd = false;
	private float lr = 0f;

	// Update is called once per frame
	void Update () {
		Debug.DrawRay (transform.position, transform.forward);
		transform.Rotate (transform.up, lr * rotateSpeed);

		if (fwd) {
			transform.position = transform.position + transform.forward * forwardSpeed / 100f;
		}
	}

	public void Move( bool forward, float rotateLR ) {
		fwd = forward;
		
		if ( rotateLR < 0f ) {
					lr = -1f;
		} else if ( rotateLR > 0f ) {
			lr = 1f;
		} else {
			lr = 0f;
		}
	}
}
