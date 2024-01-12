using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EndingScene : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> credits = new List<GameObject>();

    private float waitingTime = 3f;

    public void GoToMain()
    {
        //GameManager.Sound.ResetBGMTime();
        SceneManagerEx.LoadScene("TitleScene");
    }

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        GameManager.Sound.Play(GameManager.Sound.ObjectDataSounds.TitleSceneBGM);
        StartCoroutine(CreditCoroutine());
    }

    private IEnumerator CreditCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        for(int i=0;i<credits.Count-1; i++)
        {
            credits[i].SetActive(true);
            yield return new WaitForSeconds(waitingTime);
            credits[i].SetActive(false);
        }


        credits[credits.Count-1].SetActive(true);
    }

}
