using UnityEngine;
using System.Collections;

[RequireComponent (typeof(SoftbotMovement))]
public class SoftbotPlayerMovement : MonoBehaviour {

	private SoftbotMovement softbot;

	// Use this for initialization
	void Start () {
		softbot = GetComponent<SoftbotMovement> ();
	}
	
	// Update is called once per frame
	void Update () {
		bool forward = Input.GetAxis ("Vertical") > 0f ? true : false;
		float rotate = Input.GetAxis ("Horizontal");

		softbot.Move (forward, rotate);
	}
}
