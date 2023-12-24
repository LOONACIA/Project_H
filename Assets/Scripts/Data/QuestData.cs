using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(QuestData), menuName = "Data/" + nameof(QuestData))]
public class QuestData : ScriptableObject
{
    [SerializeField]
	private TextAsset m_questData;
}
