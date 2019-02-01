using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour // 
{

    public float popupSpeed = 1.5f;

    public Vector2 initScale;

    public virtual void Start()
    {

        initScale = transform.localScale;
        StartCoroutine(Opening());

    }

    public virtual void Close()
    {
        StartCoroutine(Closing());
    }

    public virtual void OnBackPressed()
    {
        PopupManager.I.CloseMenu();
        //FindObjectOfType<PopupManager>().CloseMenu();
    }

    private IEnumerator Opening()
    {

        float i = 0f;

        while(i < 1f)
        {
            i += Time.deltaTime * popupSpeed;

            transform.localScale = Vector2.Lerp(Vector2.zero, initScale, Mathf.SmoothStep(0.0f, 1.0f, i));

            yield return null;
        }

        OnPoppedUp();

    }

    public virtual void OnPoppedUp()
    {

    }

    private IEnumerator Closing()
    {
        float i = 0f;

        while (i < 1f)
        {
            i += Time.deltaTime * popupSpeed;

            transform.localScale = Vector2.Lerp(initScale, Vector2.zero, Mathf.SmoothStep(0.0f, 1.0f, i));

            yield return null;
        }

        Destroy(gameObject);
    }
}
