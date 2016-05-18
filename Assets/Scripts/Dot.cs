using UnityEngine;
using System.Collections;

public class Dot : MonoBehaviour {

    public Sprite inactiveDot;
    public Sprite activeDot;

    private SpriteRenderer spriteRenderer;
    private bool isActive;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (isActive && spriteRenderer.sprite != activeDot)
        {
            spriteRenderer.sprite = activeDot;
        }
        else if (!isActive && spriteRenderer.sprite != inactiveDot)
        {
            spriteRenderer.sprite = inactiveDot;
        }
	}

    // TODO do this better later
    public bool IsActive {
        get { return isActive; }
        set { isActive = value; }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        IsActive = true;
        StartCoroutine(ReturnToInactive());
    }
    
    IEnumerator ReturnToInactive()
    {
        yield return new WaitForSeconds(0.3f);
        IsActive = false;
    }
}
