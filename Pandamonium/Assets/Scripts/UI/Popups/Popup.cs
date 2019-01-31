using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour // 
{

    public float popupSpeed = 1.5f;

    public virtual void Open()
    {

        StartCoroutine(Opening());

    }

    private IEnumerator Opening()
    {

        float i = 0f;

        while(i < 1f)
        {
            i += Time.deltaTime * popupSpeed;

            transform.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, i);

            yield return null;
        }

    }
}
