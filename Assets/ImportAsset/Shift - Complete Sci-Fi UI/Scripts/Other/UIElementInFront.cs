using UnityEngine;

namespace Michsky.UI.Shift
{
    public class UIElementInFront : MonoBehaviour
    {
        void Start()
        {
            transform.SetAsLastSibling();
        }
    }
}