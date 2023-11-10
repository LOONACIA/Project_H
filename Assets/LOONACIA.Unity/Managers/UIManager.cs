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

        private int _order = 0;

        private Transform Root
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
            var canvas = gameObject.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;

            canvas.sortingOrder = sort ? _order++ : 0;
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

        public void ClosePopupUI(UIPopup popup)
        {
            if (_popupStack.Count == 0)
            {
                return;
            }

            if (_popupStack.Peek() != popup)
            {
                Debug.Log($"Can't close popup: {popup.name}");
                return;
            }

            popup = _popupStack.Pop();
            ManagerRoot.Resource.Release(popup.gameObject);

            _order--;
        }

        public void ClosePopupUI()
        {
            if (_popupStack.TryPeek(out var popup))
            {
                ClosePopupUI(popup);
            }
        }

        public void ClearAllPopup()
        {
            while (_popupStack.Count > 0)
            {
                ClosePopupUI();
            }
        }
        
        public void Clear(bool destroyAssociatedObject)
        {
            ClearAllPopup();
            _sceneUI = null;

            if (destroyAssociatedObject)
            {
                Object.Destroy(_root.gameObject);
            }
        }
    }
}