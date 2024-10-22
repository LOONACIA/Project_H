using System;
using System.Collections;
using UnityEngine;

namespace LOONACIA.Unity.Coroutines
{
    public class CoroutineEx
    {
        private readonly MonoBehaviour _root;

        private Coroutine _handler;

        private Action _callback;

        private CoroutineEx(MonoBehaviour root)
        {
            _root = root;
        }

        public bool IsRunning { get; private set; }

        public static CoroutineEx Create(MonoBehaviour root, IEnumerator routine)
        {
            CoroutineEx coroutineEx = new(root);
            coroutineEx.Start(routine);
            return coroutineEx;
        }
        
        public static CoroutineEx Create(MonoBehaviour root, IEnumerator routine, Action callback)
        {
            CoroutineEx coroutineEx = new(root);
            coroutineEx._callback = callback;
            coroutineEx.Start(routine);
            return coroutineEx;
        }

        public void Abort()
        {
            if (_handler == null || _root == null)
            {
                return;
            }

            _root.StopCoroutine(_handler);
            _callback?.Invoke();
            _handler = null;
            IsRunning = false;
        }

        private void Start(IEnumerator routine)
        {
            _root.StartCoroutine(Wrap(routine));
        }

        private IEnumerator Wrap(IEnumerator routine)
        {
            IsRunning = true;
            _handler = _root.StartCoroutine(routine);
            yield return _handler;
            _handler = null;
            IsRunning = false;
            _callback?.Invoke();
        }
    }
}