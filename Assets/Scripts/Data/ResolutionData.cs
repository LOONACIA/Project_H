using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ResolutionData), menuName = "Data/" + nameof(ResolutionData))]
public class ResolutionData : ScriptableObject
{
    [SerializeField]
    private List<Item> items;

    /// <summary>
    /// 해상도를 데이터에 추가합니다.
    /// </summary>
    /// <param name="width">추가할 해상도의 가로 길이</param>
    /// <param name="height">추가할 해상도의 세로 길이</param>
    /// <returns></returns>
    public bool AddResolutionItem(int width, int height)
    {
        Item curRes = new(width, height);
        if (!items.Contains(curRes))
        {
            items.Add(curRes);
            items.Sort();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 해상도를 "WIDTHxHEIGHT" 문자열 포맷으로 받아옵니다.
    /// </summary>
    /// <param name="stringItems">값을 받아올 문자열 리스트</param>
    public void GetItemsAsPairNonAlloc(ref List<KeyValuePair<int, int>> pairItems)
    {
        pairItems.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            Item item = items[i];
            pairItems.Add(new(item.Width,item.Height));
        }
    }

    /// <summary>
    /// 해당 해상도의 인덱스를 받아옵니다. 없다면 0을 반환합니다.
    /// </summary>
    /// <param name="width">현재 해상도의 가로 길이</param>
    /// <param name="height">현재 해상도의 세로 길이</param>
    /// <returns>현재 해상도의 인덱스, 해당 해상도가 없다면 0 반환</returns>
    public int GetIndex(int width, int height)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Width == width && items[i].Height == height)
            {
                return i;
            }
        }
        return 0;
    }

    [Serializable]
    private class Item : IComparable<Item>
    {
        public Item(int width, int height)
        {
            Width = width;
            Height = height;
        }

        [field: SerializeField]
        public int Width { get; set; }

        [field: SerializeField]
        public int Height { get; set; }

        public static bool operator ==(Item left, Item right)
        {
            return left.Width==right.Width && left.Height == right.Height;
        }

        public static bool operator !=(Item left, Item right)
        {
            return !(left == right);
        }

        public override bool Equals(object other)
        {
            return (other is Item) && this == (other as Item);
        }

        public override string ToString()
        {
            return Width + "x" + Height;
        }

        public int CompareTo(Item other)
        {
            return ToString().CompareTo(other.ToString());
        }        
    }
}
