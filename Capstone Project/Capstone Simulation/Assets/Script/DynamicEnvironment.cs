using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicEnvironment : MonoBehaviour {

	private EnvironmentBuilder environmentBuilder;
	private bool clickSpamFlag;
	public float clickDelay = 0.2f;
	public Material objectMaterial;

	void Start () {
		environmentBuilder = GameObject.Find ("Panel").GetComponent<EnvironmentBuilder> ();
	}

	void OnMouseDown() {
		if (!clickSpamFlag && environmentBuilder != null && environmentBuilder.active != null) {
			RaycastHit hit = new RaycastHit();
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast(ray, out hit)) {
				GameObject go = Instantiate<GameObject>(environmentBuilder.active.gameObject);
				go.GetComponent<HoverableObject>().enabled = false;
				go.GetComponent<Renderer>().material = objectMaterial;
				go.transform.position = hit.point;
				go.transform.localScale = new Vector3(2f,2f,2f);
                go.layer = LayerMask.NameToLayer( "EnvironmentVisible" );

                if ( clickDelay > 0f ) {
                    clickSpamFlag = true;
                    StartCoroutine( DelayClick() );
                }

				if ( go.name.Contains( "EnvArrow" ) ) {
					go.transform.localScale = new Vector3(0.34f, 1f, 0.4f);
					go.transform.RotateAround( go.transform.position, Vector3.right, -90f);
                    go.transform.Translate( new Vector3( 0f, -1f, 0f ) );
				} else if ( go.name.Contains( "EnvCircle" ) ) {
                    go.transform.localScale = new Vector3( 2f, 1f, 2f );
                    go.transform.RotateAround( go.transform.position, Vector3.right, -90f );
                } else if ( go.name.Contains( "EnvTriangle" ) ) {
                    go.transform.localScale = new Vector3( 1f, 1f,1f );
                    go.transform.RotateAround( go.transform.position, Vector3.right, -90f );
                    go.transform.RotateAround( go.transform.position, Vector3.up, 180f );
                    go.transform.Translate( Vector3.down );
                } else if ( go.name.Contains( "EnvBall" ) ) {
                    go.transform.Translate( Vector3.up * 4f );
                    go.transform.localScale = new Vector3( 2f, 2f, 2f );
                    go.GetComponent<Rigidbody>().useGravity = true;
                }

				go.transform.SetParent (transform);
			}
		}
	}

    void OnMouseDrag() {
        OnMouseDown();
    }

	IEnumerator DelayClick () {
		yield return new WaitForSeconds( clickDelay );
		clickSpamFlag = false;
	}

	public void ClearAll() {
		foreach (Transform t in transform) {
			if ( t.name.Contains("Env") ) {
				Destroy( t.gameObject );
			}
		}
	}

    public void SetSpamMode( bool val ) {
        if ( val ) {
            clickDelay = 0f;
        } else {
            clickDelay = 0.2f;
        }
    }
}
