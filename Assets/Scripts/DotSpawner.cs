using UnityEngine;
using System.Collections;

public class DotSpawner : MonoBehaviour {

    public CombatPhaseManager combatPhaseManager;
    public Dot dotPrefab;
    
    public enum Shape { HEXAGON, PENTAGON };
    public Shape shape;

    private bool spawned = false;

    private Vector2[] positions;
    private Dot[] dots;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void SpawnDotShape()
    {
        if (spawned)
        {
            return;
        }

        positions = GetPositions(shape);
        dots = new Dot[positions.Length];

        //Debug.Log(positions.Length);
        for (int i = 0; i < positions.Length; i++)
        {
            Dot dotObj = Instantiate(dotPrefab);
            dotObj.name = "Dot" + i;
            dots[i] = dotObj;

            Vector2 position = positions[i];
            dotObj.transform.position = position;
            dotObj.transform.SetParent(this.transform);
        }
        spawned = true;
    }

    private Vector2[] GetPositions(Shape shape)
    {
        switch (shape)
        {
            case Shape.HEXAGON:
                return GetHexagon();
            case Shape.PENTAGON:
                return GetPentagon();
            default:
                Debug.Log("Bad shape request.");
                return GetHexagon();
        }
    }

 
    public Dot[] GetDots()
    {
        return dots;
    }

    private Vector2[] GetHexagon()
    {
        return new Vector2[6]
        {
            new Vector3(0,   3,    0),
            new Vector3(3,   1.5f, 0),
            new Vector3(3,  -1.5f, 0),
            new Vector3(0,  -3,    0),
            new Vector3(-3, -1.5f, 0),
            new Vector3(-3,  1.5f, 0)
        };
    }

    private Vector2[] GetPentagon()
    {
        return new Vector2[5]
        {
            new Vector3(0,   3,    0),
            new Vector3(3,   0.25f, 0),
            new Vector3(2,  -3,    0),
            new Vector3(-2, -3,    0),
            new Vector3(-3,   0.25f, 0)
        };
    }
}
