using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelIntroText : MonoBehaviour {

    private Text text;

    Color endColor;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        text.enabled = true;

        text.text = "Level " + (GameManager.I.currentLevel + 1).ToString();

        StartCoroutine(Lifetime());
	}
	
	private IEnumerator Lifetime()
    {

        yield return new WaitForSeconds(1);

        endColor = new Color(text.color.r, text.color.g, text.color.b, 0);
        float i = 0;

        while(i < 1)
        {
            i += Time.deltaTime;
            text.color = Color.Lerp(text.color, endColor, i);
            yield return null;
        }

        Destroy(gameObject);
    }
}
