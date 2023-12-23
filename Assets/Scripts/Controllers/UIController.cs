using UnityEngine;

public partial class UIController : MonoBehaviour
{
	private void Start()
	{
        InitInput();
	}

    private void Pause()
    {
        m_inputActions.Character.Disable();
        GameManager.Instance.Pause(m_inputActions.Character.Enable);
    }
}
