using UnityEngine;
using System;
using System.Collections;

public class TouchScript : MonoBehaviour {

    public CombatPhaseManager combatPhaseManager;

    private bool isTouched = false;

    private Vector3 nullVec3 = new Vector3();
    private Vector3 touchStart;
    private Vector3 touchEnd;

    private LineRenderer liner;
    private Phase prevPhase = Phase.NONE;

    private int slashCount = 0;
    private bool[] slices;
    

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
        liner.material = new Material(Shader.Find("Particles/Additive"));
        
    }

    // Update is called once per frame
    void Update () {
        if (combatPhaseManager.CurrentPhase != Phase.PLAYER)
        {
            prevPhase = combatPhaseManager.CurrentPhase;
            return;
        }

        if (prevPhase != Phase.PLAYER) // show once
        {
            slices = new bool[combatPhaseManager.GetOrders().Capacity];
            combatPhaseManager.SetCombatText("Attack!", 0.5f);
            prevPhase = Phase.PLAYER;
        }
        
        isTouched = IsTouched();
        if (!isTouched)
        {
            touchStart = nullVec3;
            touchEnd = nullVec3;
            return;
        }
              

        Touch touch = Input.GetTouch(0);
        
        Vector3 screenTouchPosition = touch.position;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenTouchPosition);
        worldPosition.z = 0;

        if (touchStart == nullVec3)
        {
            touchStart = worldPosition;
        }

        if (touch.phase == TouchPhase.Ended)
        {
            touchEnd = worldPosition;

            bool didHit = ResolveSlice(touchStart, touchEnd);

            slices[slashCount] = didHit;
            slashCount++;

            bool finished = slashCount == combatPhaseManager.GetOrders().Capacity;
            if (finished)
            {
                StartCoroutine(DelayAction(ClearLine, 1));
                StartCoroutine(DelayAction(EndPlayerPhase, 1));
            }
        }
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

    private bool ResolveSlice(Vector3 touchStart, Vector3 touchEnd)
    {
        liner.SetPosition(0, touchStart);
        liner.SetPosition(1, touchEnd);

        RaycastHit2D[] lineCastHits = Physics2D.LinecastAll(touchStart, touchEnd);

        if (lineCastHits.Length > 0)
        {
            foreach (RaycastHit2D rayHit in lineCastHits)
            {
                // Manually trigger collisions with any hit dots.
                rayHit.collider.BroadcastMessage("OnTriggerEnter2D", rayHit.collider);
            }
        }

        bool didHit = (lineCastHits.Length >= 2 && IsProperSlice(lineCastHits, slashCount));

        string hitText = "";
        if (didHit)
            hitText = "Slice!";
        else if (lineCastHits.Length == 2)
            hitText = "Fumble!";
        else
            hitText = "Miss!";

        combatPhaseManager.SetCombatText(hitText, 0.5f);
        return didHit;
    }

    private bool IsProperSlice(RaycastHit2D[] hits, int slashCount)
    {
        if (hits.Length < 2)
        {
            Debug.Log("Not enough hits");
            return false;
        }
        
        Dot[] targetPair = combatPhaseManager.GetOrders()[slashCount];

        GameObject hitA = hits[0].collider.gameObject;
        GameObject hitB = hits[1].collider.gameObject;
        GameObject targetA = targetPair[0].gameObject;
        GameObject targetB = targetPair[1].gameObject;

        //Debug.Log("IDs :" +
        //    hitA.GetInstanceID() + "/" +
        //    hitB.GetInstanceID() + "/" +
        //    targetA.GetInstanceID() + "/" +
        //    targetB.GetInstanceID());

        bool dotAValid = hitA.Equals(targetA) || hitA.Equals(targetB);
        bool dotBValid = hitB.Equals(targetA) || hitB.Equals(targetB);

        //Debug.Log("dotAValid " + dotAValid);
        //Debug.Log("dotBValid " + dotBValid);

        return (dotAValid && dotBValid);
    }

    private void ClearLine()
    {
        liner.SetPosition(0, nullVec3);
        liner.SetPosition(1, nullVec3);
    }

    private void Reset()
    {
        slashCount = 0;
        slices = null;
        ClearLine();
        touchStart = nullVec3;
        touchEnd = nullVec3;
    }

    private void EndPlayerPhase()
    {
        combatPhaseManager.SetResolvePlayerPhase(slices);
        Reset();
    }

    IEnumerator DelayAction(Action cb, int delaySec)
    {
        yield return new WaitForSeconds(delaySec);
        cb();
    }
}
