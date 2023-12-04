using DG.Tweening;
using LOONACIA.Unity.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using static Unity.VisualScripting.Metadata;

public class UIObjects : UIScene
{
    #region PublicVariables
    public Transform layout;
    public TMP_Text objectText;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    private void Start()
    {
        Transform[] myChildren = this.GetComponentsInChildren<Transform>();

        foreach (Transform child in myChildren)
        {
            if (child.name == "Layout")
                layout = child;

            if (child.name == "ObjectText")
                objectText = child.GetComponent<TMP_Text>();
        }
   
    }

    public void UpdateObjectText(string _text)
    {   
        objectText.text = _text;

        objectText.transform.localScale = Vector3.one * 2f;

        objectText.transform.DOScale(1, 1f);
    }
    #endregion

    #region PrivateMethod
    #endregion
}
