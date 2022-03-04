using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// This script is to be added to the objects we dynamically create from the sensor readings
/// </summary>
public class SpawnObject : MonoBehaviour {

    public int LifetimeInSeconds = 5;
    public int paintSize = 10;
    public Color paintColor = Color.gray;
    private bool hovering = false;
    private int paintEvery = 20;
    private int targetCount = 0;

    private Color startcolor;
    void OnMouseEnter()
    {
        hovering = true;
        startcolor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.yellow;
    }
    void OnMouseExit()
    {
        hovering = false;
        GetComponent<Renderer>().material.color = startcolor;
    }

    // Use this for initialization
    void Start () {
        startcolor = GetComponent<Renderer>().material.color;
    }
	
	// Update is called once per frame
	void Update () {
        if ( !hovering ) {
            transform.localScale *= 0.998f;
            startcolor = GetComponent<Renderer>().material.color;
            Color.Lerp( startcolor, new Color( startcolor.grayscale, startcolor.grayscale, startcolor.grayscale ), Time.deltaTime );
        }
	}

    void Awake()
    {
        RaycastHit hit = new RaycastHit();
        Ray r = new Ray( transform.position, Vector3.down );

        if ( Physics.Raycast( r, out hit ) ) {
            if ( hit.collider.gameObject.name == "Floor" ) {
                if ( ++targetCount%paintEvery == 0 ) {
                    Debug.Log( "Paint da floor! :: " + hit.textureCoord );
                    GameObject floor = hit.collider.gameObject;
                    Renderer rend = floor.GetComponent<Renderer>();
                    Texture2D tex = new Texture2D(2000,2000);
                    rend.material.SetTexture( "Albedo", tex );
                    Vector2 pixelUV = hit.textureCoord;
                    Color[] c = Enumerable.Repeat(paintColor, paintSize*paintSize).ToArray();

                    //tex.SetPixels( Mathf.FloorToInt(pixelUV.x - paintSize/2), Mathf.FloorToInt( pixelUV.y - paintSize/2), paintSize, paintSize, c );
                }
            }
        }

        Destroy( this.gameObject, this.LifetimeInSeconds );
    }



}
