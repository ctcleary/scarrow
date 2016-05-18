using UnityEngine;
using System.Collections;

public class DotSpawner : MonoBehaviour {

    public GameObject dotPrefab;

    private GameObject[] dots = new GameObject[6];

    // Use this for initialization
    void Start() {
        Vector2[] positions = new Vector2[6]
        {
            new Vector3(0,   3,    0),
            new Vector3(3,   1.5f, 0),
            new Vector3(3,  -1.5f, 0),
            new Vector3(0,  -3,    0),
            new Vector3(-3, -1.5f, 0),
            new Vector3(-3,  1.5f, 0)
        };

	    for (int i = 0; i < 6; i++)
        {
            GameObject dotObj = Instantiate(dotPrefab);
            dotObj.name = "Dot" + i;
            dots[i] = dotObj;

            Vector2 position = positions[i];
            dotObj.transform.position = position;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
