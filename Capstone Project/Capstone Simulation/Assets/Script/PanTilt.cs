using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets;

public class PanTilt : MonoBehaviour {

	public enum PanTiltMode {
		SweepHorizontal,
		Stationary,
		LookAt
	}

    public PanTiltMode panTiltMode = PanTiltMode.Stationary;
	public float panTiltSpeed = 4f;
	public float panRange = 170f;
	public float tiltRange = 170f;
	public float maxAngleFocus = 10f;
	public GameObject lookTarget;

	private Transform botTransform;

	private bool panRight = false;
	private Vector3 lookAt;

	// Use this for initialization
	void Start () {
		lookAt = transform.forward;
		botTransform = transform.parent;
	}
	
	// Update is called once per frame
	void Update () {

		if (panTiltMode == PanTiltMode.SweepHorizontal) {
			SweepHorizontal ();
		} else if (panTiltMode == PanTiltMode.LookAt) {
			LookAt();
		}

		if (Input.GetMouseButton (0)) {
			lookAt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			panTiltMode = PanTiltMode.LookAt;
			SpawnTarget();
		}
	}

	void SweepHorizontal() {
		if (panRight && Vector3.Angle (botTransform.forward, transform.forward) < panRange / 2) {
			transform.Rotate (transform.up, panTiltSpeed);
		} else if (!panRight && Vector3.Angle (botTransform.forward, transform.forward) < panRange / 2) {
			transform.Rotate (transform.up, -panTiltSpeed);
		} else if (panRight) {
			transform.Rotate (transform.up, -panTiltSpeed);
			panRight = false;
		} else if (!panRight) {
			transform.Rotate (transform.up, panTiltSpeed);
			panRight = true;
		}
	}

	void LookAt () {
		Vector3 lookDir = (lookAt - transform.position).normalized;
		float proj = Vector3.Dot (lookDir, transform.right);

		if (proj < 0f && Vector3.Angle (transform.forward, lookDir) > maxAngleFocus) {
			transform.Rotate (transform.up, -panTiltSpeed);
		} else if (proj > 0f && Vector3.Angle (transform.forward, lookDir) > maxAngleFocus) {
			transform.Rotate (transform.up, panTiltSpeed);
		}
	}

	void SpawnTarget() {
		GameObject target = GameObject.FindGameObjectWithTag ("Target");
		target.transform.position = new Vector3(lookAt.x, 0.5f, lookAt.z);
	}
}
