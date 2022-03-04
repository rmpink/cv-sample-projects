using UnityEngine;
using System.Collections;

public class HoverableObject : MonoBehaviour {

	private bool hovering = false;
	private bool active = false;

	private EnvironmentBuilder environmentBuilder;
	private Renderer r;
	private Quaternion initialRotation;

	// Use this for initialization
	void Start () {
		r = GetComponent<MeshRenderer> ();
		environmentBuilder = GameObject.Find ("Panel").GetComponent<EnvironmentBuilder> ();
		initialRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
	}
	
	// Update is called once per frame
	void Update () {
		if (hovering) {
			transform.Rotate(-0.1f,-0.2f,-0.15f);
		}
	}

	void OnMouseEnter() {
		if (!hovering && r != null && r.material != null) {
			r.material.color += new Color (0.1f, 0.1f, 0.1f);
			hovering = true;
		}
	}

	void OnMouseExit() {
		if (hovering && r != null && r.material != null) {
			r.material.color -= new Color (0.1f, 0.1f, 0.1f);
			hovering = false;
			transform.rotation = new Quaternion (initialRotation.x, initialRotation.y, initialRotation.z, initialRotation.w);
		}
	}

	void OnMouseDown() {
		if (!active && r != null && r.material != null) {
			active = true;
			transform.localScale *= 1.2f;
			r.material.color += new Color (0.0f, 0.0f, 0.2f);
			environmentBuilder.setActive (gameObject.name);
		}
	}

	public void setInactive() {
		if (active && r != null && r.material != null) {
			active = false;
			transform.localScale /= 1.2f;
			r.material.color -= new Color (0.0f, 0.0f, 0.2f);
		}
	}
}
