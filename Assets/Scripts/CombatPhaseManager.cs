using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * This object will act as the mediator for information pass-through between
 * various combat related game objects.
 *
 */

public enum Phase { NONE, COUNTDOWN, SIMON, PLAYER, RESOLVE_PLAYER, ENEMY };

public class CombatPhaseManager : MonoBehaviour {

    public Simon simon;
    public DotSpawner dotSpawner;

    //temp
    public CombatText combatText;

    public Phase currentPhase = Phase.NONE;
    //private int countdownCurrent = 0;

    private List<Dot[]> sequence;
    
	// Use this for initialization
	void Start ()
    {
        dotSpawner.SpawnDotShape();
        SetPhase(Phase.COUNTDOWN);
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void SetPhase(Phase newPhase)
    {
        switch (newPhase)
        {
            case Phase.COUNTDOWN:
                SetCountdownPhase();
                return;
            case Phase.SIMON:
                SetSimonPhase();
                break;
            case Phase.PLAYER:
            default:
                return;
        }
    }

    public List<Dot[]> GetOrders()
    {
        return sequence;
    }

    public Phase CurrentPhase
    {
        get { return currentPhase; }
        private set { currentPhase = value; }
    }

    private IEnumerator Countdown(int sec, Phase goToPhase)
    {
        int countdownCt = 3;
        while (countdownCt > 0)
        {
            SetCombatText(""+countdownCt, 0.35f);
            yield return new WaitForSeconds(1f);
            countdownCt--;
        }
        SetCombatText("Go!", 0.15f);
        yield return new WaitForSeconds(0.35f);
        SetPhase(goToPhase);
    }

    public void SetCombatText(string text, float opt_fadeDelay = 0)
    {
        combatText.SetText(text, opt_fadeDelay);
    }


    private void SetCountdownPhase()
    {
        currentPhase = Phase.COUNTDOWN;
        StartCoroutine(Countdown(3, Phase.SIMON));
    }

    public void SetSimonPhase()
    {
        simon.GiveOrders(SetPlayerPhase);
    }

    public void SetPlayerPhase(List<Dot[]> sequence)
    {
        this.sequence = sequence;
        currentPhase = Phase.PLAYER;
    }

    public void SetResolvePlayerPhase(bool[] slices)
    {
        Debug.Log("slices " + slices[0] + "/" + slices[1] + "/" + slices[2]);
        currentPhase = Phase.RESOLVE_PLAYER;

        int hits = 0;
        for (int i = 0; i < slices.Length; i++)
        {
            if (slices[i])
                hits++;
        }

        bool perfectAttack = (slices.Length == hits);
        string attackDescriptor = "";

        if (perfectAttack)
            attackDescriptor = "Perfect!";
        else if (hits > 1)
            attackDescriptor = "Good!";
        else if (hits == 1)
            attackDescriptor = "Poor";
        else
            attackDescriptor = "Fail!";

        SetCombatText(attackDescriptor, 0.35f);
        StartCoroutine(DelayAction(SetCountdownPhase, 1.35f));
    }


    IEnumerator DelayAction(Action cb, float delaySec)
    {
        yield return new WaitForSeconds(delaySec);
        cb();
    }
}
