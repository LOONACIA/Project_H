using System;

public class MenuInfo
{
    public MenuInfo(string resourceKey, Action onClick)
    {
        ResourceKey = resourceKey;
        OnClick = onClick;
    }
    
    public string ResourceKey { get; }
    
    public Action OnClick { get; set; }
}
