using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {


    public GameObject rulerPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1))    // right click mouse down
        {
            var vector = Input.mousePosition;


            vector = Camera.main.ScreenToWorldPoint(vector);

            vector[1] = 1;  // y

            Debug.Log("Creating ruler at " + vector.ToString());
            var ruler = Instantiate(rulerPrefab,
               vector,
               rulerPrefab.transform.rotation) as GameObject;

            //ruler.transform.position = new Vector3(ruler.transform.position.x,
            //    ruler.transform.position.y + 2,
            //    ruler.transform.position.z);
        }
    }

    
}
