using System.Collections.Generic;
using LOONACIA.Unity.UI;
using UnityEngine;

namespace LOONACIA.Unity.Managers
{
    public class UIManager
    {
        private readonly Stack<UIPopup> _popupStack = new();

        private Transform _root;

        private UIScene _sceneUI;

        private int _order = 1;

        public Transform Root
        {
            get
            {
                Init();
                return _root;
            }
        }

        public void Init()
        {
            if (_root == null)
            {
                _root = new GameObject { name = "@UI_Root" }.transform;
            }
        }

        public void SetCanvas(GameObject gameObject, bool sort = true)
        {
            var canvas = gameObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.GetComponentInChildren<Canvas>();
            }
            
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;

            canvas.sortingOrder = sort ? _order++ : 0;
        }
        
        public T ShowItemUI<T>(string name = null, bool usePool = true)
            where T : UIBase
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject gameObject = ManagerRoot.Resource.Instantiate($"UI/Item/{name}", usePool: usePool);
            var ui = gameObject.GetOrAddComponent<T>();

            gameObject.transform.SetParent(Root.transform);

            return ui;
        }

        public T ShowSceneUI<T>(string name = null)
            where T : UIScene
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject gameObject = ManagerRoot.Resource.Instantiate($"UI/Scene/{name}");
            var sceneUI = gameObject.GetOrAddComponent<T>();
            _sceneUI = sceneUI;

            gameObject.transform.SetParent(Root.transform);

            return sceneUI;
        }

        public T ShowPopupUI<T>(string name = null, bool usePool = true)
            where T : UIPopup
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject gameObject = ManagerRoot.Resource.Instantiate($"UI/Popup/{name}", usePool: usePool);
            var popup = gameObject.GetOrAddComponent<T>();
            _popupStack.Push(popup);

            gameObject.transform.SetParent(Root.transform);

            return popup;
        }

        public bool ClosePopupUI(UIPopup popup)
        {
            if (_popupStack.Count == 0)
            {
                return false;
            }

            if (_popupStack.Peek() != popup)
            {
                return false;
            }

            popup = _popupStack.Pop();
            if (popup != null)
            {
                ManagerRoot.Resource.Release(popup.gameObject);
            }

            _order--;

            return true;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public bool ClosePopupUI()
        {
            if (_popupStack.TryPeek(out var popup))
            {
                popup.Close();
                return true;
            }

            return false;
        }

        public void ClearAllPopup()
        {
            while (_popupStack.Count > 0)
            {
                if (_popupStack.TryPeek(out var popup))
                {
                    ClosePopupUI(popup);
                }
            }
        }
        
        public void Clear(bool destroyAssociatedObject)
        {
            ClearAllPopup();
            for (int index = Root.childCount - 1; index >= 0; index--)
            {
                Transform child = Root.GetChild(index);
                ManagerRoot.Resource.Release(child.gameObject);
            }
            
            _sceneUI = null;

            if (destroyAssociatedObject)
            {
                Object.Destroy(_root.gameObject);
            }
        }

        public UIPopup GetTopPopupUI()
        {
            return _popupStack.Count > 0 ? _popupStack.Peek() : null;
        }
    }
}