using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionFader : ScreenFader
{
    [SerializeField]
    private float _TransitionLifetime = 1f;

    [SerializeField]
    private float _delay = 0.3f;

    protected void Awake()
    {
        _TransitionLifetime = Mathf.Clamp(_TransitionLifetime, FadeOnDuration + FadeOffDuration + _delay, 10f);
    }

    private IEnumerator PlayRoutine()
    {
        SetAlpha(_clearAlpha);

        yield return new WaitForSeconds(_delay);

        FadeOn();
        float onTime = _TransitionLifetime - (FadeOffDuration + _delay);

        yield return new WaitForSeconds(onTime);

        FadeOff();
        Object.Destroy(gameObject, FadeOffDuration);
    }

    public void Play()
    {
        StartCoroutine(PlayRoutine());
    }

    public static void PlayTransition (TransitionFader transitionPrefab)
    {
        if (transitionPrefab != null)
        {
            TransitionFader instance = Object.Instantiate(transitionPrefab, Vector3.zero, Quaternion.identity);
            instance.Play();
        }
    }
}
