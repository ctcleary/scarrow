using UnityEngine;
using System.Collections;

public class CombatText : MonoBehaviour {

    private UnityEngine.UI.Text uiText;
    public Color textColor;
    private Material textMaterial;

    Coroutine runningFade;

    // Use this for initialization
    void Start () {
        uiText = GetComponentInParent<UnityEngine.UI.Text>();
        textMaterial = uiText.material;
        textMaterial.color = textColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetText(string text, float opt_fadeDelay = 0)
    {
        if (runningFade != null)
            StopCoroutine(runningFade);
        
        uiText.material.color = textColor;
        uiText.text = text;
        runningFade = StartCoroutine(ReturnToEmptyText(opt_fadeDelay));
    }


    IEnumerator ReturnToEmptyText(float opt_fadeDelay = 0)
    {
        if (opt_fadeDelay > 0)
            yield return new WaitForSeconds(opt_fadeDelay);

        for (float f = 1f; f > 0; f -= 0.05f)
        {
            Color c = uiText.material.color;
            c.a = f;
            uiText.material.color = c;
            yield return new WaitForEndOfFrame();
        }

        uiText.text = "";
        uiText.material.color = textColor;
        runningFade = null;
    }
}
