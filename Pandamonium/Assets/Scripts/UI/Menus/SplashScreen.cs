using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelManagment
{
    [RequireComponent(typeof (ScreenFader))]
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField]
        private ScreenFader _screenFader;

        [SerializeField]
        private float delay = 1f;

        private void Awake()
        {
            _screenFader = GetComponent<ScreenFader>();
        }

        /*private void Start()
        {
            _screenFader.FadeOn();
        }

        public void fadeAndLoad()
        {
            StartCoroutine(FadeRoutine());
        }

        private IEnumerator FadeRoutine()
        {
            yield return new WaitForSeconds(delay);
            _screenFader.FadeOff();

            yield return new WaitForSeconds(_screenFader.FadeOnDuration);
            Destroy(gameObject);
        }*/
    }
}