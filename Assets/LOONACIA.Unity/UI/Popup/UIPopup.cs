using LOONACIA.Unity.Managers;

namespace LOONACIA.Unity.UI
{
    public class UIPopup : UIBase
    {
        protected override void Init()
        {
            ManagerRoot.UI.SetCanvas(gameObject, true);
        }

        public virtual void Close()
        {
            ManagerRoot.UI.ClosePopupUI(this);
        }
    }
}