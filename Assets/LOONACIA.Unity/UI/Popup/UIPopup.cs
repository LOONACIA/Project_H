using LOONACIA.Unity.Managers;

namespace LOONACIA.Unity.UI
{
    public class UIPopup : UIBase
    {
        private bool _isInitialized;
        
        protected virtual void OnEnable()
        {
            ManagerRoot.UI.SetCanvas(gameObject, true);
        }

        protected virtual void OnDisable()
        {
        }

        public virtual void Close()
        {
            ManagerRoot.UI.ClosePopupUI(this);
        }
        
        protected override void Init()
        {
        }
    }
}