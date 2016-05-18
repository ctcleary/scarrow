using UnityEngine;
using System.Collections;

public class TouchScript : MonoBehaviour {
    
    public UnityEngine.UI.Text hitText;
    public Color hitTextColor;
    private Material hitTextMaterial;

    private Dot[] dots = new Dot[6];

    private bool isTouched = false;

    private Vector3 nullVec3 = new Vector3();
    private Vector3 touchStart;
    private Vector3 touchEnd;

    private LineRenderer liner;

	// Use this for initialization
	void Start () {
        gameObject.AddComponent<LineRenderer>();

        liner = GetComponent<LineRenderer>();
        liner.SetVertexCount(2);
        liner.SetWidth(0.05f, 0.2f);
        liner.useLightProbes = false;
        liner.receiveShadows = false;
        liner.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        liner.SetColors(Color.grey, Color.white);
        //liner.material = new Material(Shader.Find("Standard"));
        liner.material = new Material(Shader.Find("Particles/Additive"));


        hitTextMaterial = hitText.material;
        hitTextMaterial.color = hitTextColor;
    }

    // Update is called once per frame
    void Update () {
        if (dots[0] == null)
        {
            // This is terrible, do it better after prototype.
            for (int i = 0; i < 6; i++)
            {
                dots[i] = GameObject.Find("Dot" + i).GetComponent<Dot>();
            }
        }

        isTouched = IsTouched();

        if (touchStart != nullVec3 && touchEnd != nullVec3)
        {
            liner.SetPosition(1, touchEnd);

            RaycastHit2D[] lineCasts = Physics2D.LinecastAll(touchStart, touchEnd);

            if (lineCasts.Length > 0)
            {
                foreach (RaycastHit2D rayHit in lineCasts)
                {
                    rayHit.collider.BroadcastMessage("OnTriggerEnter2D", rayHit.collider);
                }
            }

            if (lineCasts.Length == 2) {
                Debug.Log("Slice!");
                hitText.text = "Slice!";
                StartCoroutine(ReturnToEmptyText());
            }
            else
            {
                Debug.Log("Miss!");
                hitText.text = "Miss!";
                StartCoroutine(ReturnToEmptyText());
            }

        }

        if (!isTouched)
        {
            //foreach (Dot dot in dots)
            //    dot.IsActive = false;

            touchStart = nullVec3;
            touchEnd = nullVec3;
            return;
        }

        

        Touch touch = Input.GetTouch(0);
        
        Vector3 screenTouchPosition = touch.position;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenTouchPosition);
        worldPosition.z = 0;
        //sampleDot.transform.position = worldPosition;

        if (touchStart == nullVec3)
        {
            //Debug.Log("set touchstart");
            touchStart = worldPosition;
            liner.SetPosition(0, touchStart);
        }

        liner.SetPosition(1, worldPosition);

        if (touch.phase == TouchPhase.Ended)
        {
            //Debug.Log("set touchend");
            touchEnd = worldPosition;
        }

        //foreach (Dot dot in dots)
        //    dot.IsActive = true;
	}

    private bool IsTouched()
    {
        int fingerCount = 0;
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Canceled)
            {
                fingerCount++;
            }
        }
        return fingerCount > 0;
    }

    IEnumerator ReturnToEmptyText()
    {
        for (float f = 1f; f > 0; f -= 0.05f)
        {
            Color c = hitText.material.color;
            c.a = f;
            hitText.material.color = c;
            yield return new WaitForEndOfFrame();
        }

        hitText.text = "";
        hitText.material.color = hitTextColor;

        //Color currColor = hitText.material.color;
        //currColor.a -= 0.1f;
        //hitText.material.color = currColor;
        //Debug.Log(currColor.a);
        //if (currColor.a > 0)
        //{
        //    yield return new WaitForEndOfFrame();
        //}

        //yield return new WaitForSeconds(0.3f);
        //hitText.text = "";
        //hitText.material.color = hitTextColor;
    }

    //void OnGUI()
    //{

    //    if (touchStart != nullVec3 && touchEnd != nullVec3)
    //        GUI.Label(new Rect(200, 100, 500, 100), "Line: (" + touchStart.x + "," + touchStart.y + " : " + touchEnd.x + ", " + touchEnd.y + ")");

    //    foreach (Touch touch in Input.touches)
    //    {
    //        string msg = "";
    //        msg += "ID: " + touch.fingerId + "\n";
    //        msg += "Phase: " + touch.phase.ToString() + "\n";
    //        msg += "TapCount: " + touch.tapCount + "\n";
    //        msg += "Pos: (" + touch.position.x + ", " + touch.position.y + ")\n";

    //        int num = touch.fingerId;
    //        GUI.Label(new Rect(0 + 130 * num, 0, 120, 100), msg);
    //    }
    //}
}
