using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LOONACIA.Unity.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        private readonly Dictionary<Type, UnityEngine.Object[]> _objects = new();

        protected virtual void Awake()
        {
            Init();
        }

        public void RegisterEvent(Action<PointerEventData> handler, UIEventType eventType = UIEventType.Click)
        {
            var eventHandlerComponent = gameObject.GetOrAddComponent<UIEventHandler>();
            switch (eventType)
            {
                case UIEventType.Click:
                    eventHandlerComponent.Clicked -= handler;
                    eventHandlerComponent.Clicked += handler;
                    break;
                case UIEventType.PointerEnter:
                    eventHandlerComponent.PointerEntered -= handler;
                    eventHandlerComponent.PointerEntered += handler;
                    break;
                case UIEventType.PointerExit:
                    eventHandlerComponent.PointerExited -= handler;
                    eventHandlerComponent.PointerExited += handler;
                    break;
                default:
                    break;
            }
        }
        
        protected abstract void Init();

        protected void Bind<TObject, TType>()
            where TObject : UnityEngine.Object
            where TType : Enum
        {
            string[] names = Enum.GetNames(typeof(TType));
            var objects = new UnityEngine.Object[names.Length];
            _objects.Add(typeof(TObject), objects);
            for (var index = 0; index < names.Length; index++)
            {
                objects[index] = typeof(TObject) == typeof(GameObject)
                    ? gameObject.FindChild(names[index], true)
                    : gameObject.FindChild<TObject>(names[index], true);

                if (objects[index] == null)
                {
                    UnityEngine.Debug.Log($"Failed to bind: {names[index]}");
                }
            }
        }
        
        protected TObject Get<TObject, TType>(TType key)
            where TObject : UnityEngine.Object
            where TType : Enum
        {
            if (!_objects.TryGetValue(typeof(TObject), out var objects))
            {
                return null;
            }

            return objects[Convert.ToInt32(key)] as TObject;
        }

        protected T Get<T>(int index)
            where T : UnityEngine.Object
        {
            if (!_objects.TryGetValue(typeof(T), out var objects))
            {
                return null;
            }

            return objects[index] as T;
        }
    }
}