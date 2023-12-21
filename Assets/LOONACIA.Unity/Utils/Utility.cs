using System.Collections;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace LOONACIA.Unity
{
    public static class Utility
    {
        public static CoroutineEx Lerp(float from, float to, float duration, System.Action<float> action, System.Action callback = null, bool ignoreTimeScale = false)
        {
            return CoroutineEx.Create(ManagerRoot.Instance, LerpCoroutine(from, to, duration, action, callback, ignoreTimeScale));
        }

        public static CoroutineEx Lerp(Vector3 from, Vector3 to, float duration, System.Action<Vector3> action, System.Action callback = null, bool ignoreTimeScale = false)
        {
            return CoroutineEx.Create(ManagerRoot.Instance, LerpCoroutine(from, to, duration, action, callback, ignoreTimeScale));
        }
        
        public static CoroutineEx Lerp(Color from, Color to, float duration, System.Action<Color> action, System.Action callback = null, bool ignoreTimeScale = false)
        {
            return CoroutineEx.Create(ManagerRoot.Instance, LerpCoroutine(from, to, duration, action, callback, ignoreTimeScale));
        }

        private static IEnumerator LerpCoroutine(float from, float to, float duration, System.Action<float> action, System.Action callback, bool ignoreTimeScale)
        {
            float time = 0;
            while (time < duration)
            {
                float delta = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                time += delta;
                float value = Mathf.Lerp(from, to, time / duration);
                action?.Invoke(value);
                yield return null;
            }
            action?.Invoke(to);

            callback?.Invoke();
        }

        private static IEnumerator LerpCoroutine(Vector3 from, Vector3 to, float duration, System.Action<Vector3> action, System.Action callback, bool ignoreTimeScale)
        {
            float time = 0;
            while (time < duration)
            {
                float delta = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                time += delta;
                var value = Vector3.Lerp(from, to, time / duration);
                action?.Invoke(value);
                yield return null;
            }
            action?.Invoke(to);

            callback?.Invoke();
        }
        
        private static IEnumerator LerpCoroutine(Color from, Color to, float duration, System.Action<Color> action, System.Action callback, bool ignoreTimeScale)
        {
            float time = 0;
            while (time < duration)
            {
                float delta = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                time += delta;
                var value = Color.Lerp(from, to, time / duration);
                action?.Invoke(value);
                yield return null;
            }
            action?.Invoke(to);

            callback?.Invoke();
        }
    }
}