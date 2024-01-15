using Michsky.UI.Reach;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionChanger : MonoBehaviour
{
    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private ResolutionData resolutionData;

    private List<KeyValuePair<int,int>> tempList = new();

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        Dropdown dropdownUI = GetComponent<Dropdown>();
        dropdownUI.items.Clear();

        //현재 해상도값이 데이터에 있는지 확인, 존재하지 않으면 추가함
        resolutionData.AddResolutionItem(Screen.width, Screen.height);

        //해상도 목록을 불러옴
        resolutionData.GetItemsAsPairNonAlloc(ref tempList);
        dropdownUI.selectedItemIndex = resolutionData.GetIndex(Screen.width, Screen.height);

        foreach(var pair in tempList)
        {
            Dropdown.Item item = new()
            {
                itemName = pair.Key + "x" + pair.Value,
                itemIcon = icon
            };
            item.onItemSelection.AddListener(() => { SetResolution(pair.Key, pair.Value); });
            dropdownUI.items.Add(item);
        }

        dropdownUI.Initialize();
    }
}
