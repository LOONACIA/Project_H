using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInfo
{
    public MenuInfo(string text, Action onClick)
    {
        Text = text;
        OnClick = onClick;
    }
    
    public string Text { get; }
    
    public Action OnClick { get; }
}
