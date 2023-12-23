using System.Runtime.InteropServices;
using UnityEngine;

public static class NativeMethods
{
    private const int SPI_GETSTICKYKEYS = 0x003A;

    private const int SPI_SETSTICKYKEYS = 0x003B;

    private const uint SKF_HOTKEYACTIVE = 0x00000004;
    
    private const uint SKF_CONFIRMHOTKEY = 0x00000008;

    public static uint DisableStickyKeysActive()
    {
        SKEY skey = new() { cbSize = (uint)Marshal.SizeOf(typeof(SKEY)), dwFlags = 0 };
        bool result = SystemParametersInfo(SPI_GETSTICKYKEYS, 0, ref skey, 0);
        if (!result)
        {
            Debug.LogError("Failed to get sticky keys.");
            return 0;
        }

        uint oldFlags = skey.dwFlags;
        skey.dwFlags &= ~(SKF_HOTKEYACTIVE | SKF_CONFIRMHOTKEY);
        result = SystemParametersInfo(SPI_SETSTICKYKEYS, 0, ref skey, 0);
        if (!result)
        {
            Debug.LogError("Failed to set sticky keys.");
            return 0;
        }

        return oldFlags;
    }

    public static void RestoreStickyKeysActive(uint oldFlags)
    {
        SKEY skey = new() { cbSize = (uint)Marshal.SizeOf(typeof(SKEY)), dwFlags = oldFlags };
        bool result = SystemParametersInfo(SPI_SETSTICKYKEYS, 0, ref skey, 0);
        if (!result)
        {
            Debug.LogError("Failed to restore sticky keys.");
        }
    }
    
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool SystemParametersInfo(int uiAction, int uiParam, ref SKEY pvParam, int fWinIni);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct SKEY
    {
        public uint cbSize;
        public uint dwFlags;
    }
}