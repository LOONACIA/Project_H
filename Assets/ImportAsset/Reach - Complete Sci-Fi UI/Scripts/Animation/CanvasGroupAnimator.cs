using System.Collections;
using UnityEngine;

namespace Michsky.UI.Reach
{
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("Reach UI/Animation/Canvas Group Animator")]
    public class CanvasGroupAnimator : MonoBehaviour
    {
        [Header("Settings")]
        [Range(0.5f, 10)] public float fadeSpeed = 4f;
        [SerializeField] private StartBehaviour startBehaviour;

        // Helpers
        CanvasGroup cg;

        public enum StartBehaviour { Default, FadeIn, FadeOut }

        void Start()
        {
            if (cg == null) { cg = GetComponent<CanvasGroup>(); }
            if (startBehaviour == StartBehaviour.FadeIn) { FadeIn(); }
            else if (startBehaviour == StartBehaviour.FadeOut) { FadeOut(); }
        }

        public void FadeIn()
        {
            gameObject.SetActive(true);

            StopCoroutine("FadeInHelper");
            StartCoroutine("FadeInHelper");
        }

        public void FadeOut()
        {
            gameObject.SetActive(true);

            StopCoroutine("FadeOutHelper");
            StartCoroutine("FadeOutHelper");
        }

        IEnumerator FadeInHelper()
        {
            StopCoroutine("FadeOutHelper");

            cg.alpha = 0;
            cg.interactable = true;
            cg.blocksRaycasts = true;

            while (cg.alpha < 0.99)
            {
                cg.alpha += fadeSpeed * Time.unscaledDeltaTime;
                yield return null;
            }

            cg.alpha = 1;
        }

        IEnumerator FadeOutHelper()
        {
            StopCoroutine("FadeInHelper");

            cg.alpha = 1;

            while (cg.alpha > 0.01)
            {
                cg.alpha -= fadeSpeed * Time.unscaledDeltaTime;
                yield return null;
            }

            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }
}