using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
	public void OnStartGameClicked()
    {
        SceneManagerEx.LoadScene("MainBuild");
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }
}
