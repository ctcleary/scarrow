using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Simon : MonoBehaviour {

    public CombatPhaseManager combatPhaseManager;
    public DotSpawner dotSpawner;
    
    private LineRenderer simonLiner;

    private bool isSimonPhase = false;
    private List<Dot[]> sequence;

    private Dot[] dots;

    private int segmentShown = -1;
    private Action<List<Dot[]>> ordersGivenCallback = null;

    // Use this for initialization
    void Start () {

        gameObject.AddComponent<LineRenderer>();

        simonLiner = GetComponent<LineRenderer>();
        simonLiner.SetVertexCount(2);
        simonLiner.SetWidth(0.1f, 0.1f);
        simonLiner.useLightProbes = false;
        simonLiner.receiveShadows = false;
        simonLiner.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        simonLiner.SetColors(Color.red, Color.red);
        simonLiner.material = new Material(Shader.Find("Particles/Additive"));

    }
	
	// Update is called once per frame
	void Update () {
        if (!isSimonPhase)
        {
            dots = null;
            sequence = null;
            segmentShown = -1;
            return;
        }

        if (sequence == null) {
            sequence = CreateSequence();
            StartCoroutine(ShowSequence(sequence));
        }
	}

    public void GiveOrders(Action<List<Dot[]>> callback)
    {
        isSimonPhase = true;
        ordersGivenCallback = callback;
    }
    
    public void RepeatOrders(List<Dot[]> sequence, Action<List<Dot[]>> callback)
    {
        this.sequence = sequence;
        StartCoroutine(ShowSequence(sequence));
    }

    public List<Dot[]> GetOrders()
    {
        return sequence;
    }

    private IEnumerator ShowSequence(List<Dot[]> sequence)
    {
        segmentShown++;
        while (segmentShown < sequence.Count) {
            Dot[] dotPair = sequence[segmentShown];
            dotPair[0].IsActive = true;
            dotPair[1].IsActive = true;
            simonLiner.SetPosition(0, dotPair[0].transform.position);
            simonLiner.SetPosition(1, dotPair[1].transform.position);
            yield return new WaitForSeconds(0.6f);

            dotPair[0].IsActive = false;
            dotPair[1].IsActive = false;
            simonLiner.SetPosition(0, new Vector3());
            simonLiner.SetPosition(1, new Vector3());
            segmentShown++;
            yield return new WaitForSeconds(0.1f);
        }
        isSimonPhase = false;
        ordersGivenCallback(sequence);
    }

    private List<Dot[]> CreateSequence(int sequenceCount = 3)
    {
        //Dictionary<Dot, Dot> result = new Dictionary<Dot, Dot>();

        List<Dot[]> result = new List<Dot[]>(sequenceCount);

        dots = dotSpawner.GetDots();
        int len = dots.Length;

        Dot dotA;
        Dot dotB;

        for (int i = 0; i < sequenceCount; i++)
        {
            int[] pair = GetValidPair(len);
            dotA = dots[pair[0]];
            dotB = dots[pair[1]];
            //Debug.Log("A," + pair[0] + " : B," + pair[1]);
            result.Add(new Dot[2] { dotA, dotB });
        }

        return result;
    }

    private int[] GetValidPair(int dotCount)
    {
        int randA = UnityEngine.Random.Range(0, dotCount);
        int randB = GetValidSecond(randA, dotCount); ;
        return new int[2] { randA, randB };
    }
    
    private int GetValidSecond(int randA, int dotCount)
    {
        int randB = UnityEngine.Random.Range(0, dotCount);
        if (IsValidPair(randA, randB, dotCount))
        {
            return randB;
        }
        return GetValidSecond(randA, dotCount);
    }

    private bool IsValidPair(int randA, int randB, int count)
    {
        if (randA == randB)
            return false;

        bool isAdjacent = (randA - 1 == randB || randA + 1 == randB);
        if (isAdjacent)
            return false;

        bool isWraparoundAdjacent = ((randA == 0 && randB == count - 1) || (randA == count - 1 && randB == 0));
        if (isWraparoundAdjacent)
            return false;

        return true;
    }



}
