using System.Runtime.InteropServices;
using UnityEngine;

public static class NativeMethods
{
    private const int SPI_GETSTICKYKEYS = 0x003A;

    private const int SPI_SETSTICKYKEYS = 0x003B;

    private const uint SKF_AVAILABLE = 0x00000002;

    private const uint SKF_HOTKEYACTIVE = 0x00000004;

    public static uint DisableStickyKeys()
    {
        var skey = new SKEY { cbSize = (uint)Marshal.SizeOf(typeof(SKEY)), dwFlags = 0 };
        var result = SystemParametersInfo(SPI_GETSTICKYKEYS, 0, ref skey, 0);
        if (!result)
        {
            Debug.LogError("Failed to get sticky keys.");
            return 0;
        }

        var oldFlags = skey.dwFlags;
        skey.dwFlags &= ~(SKF_AVAILABLE | SKF_HOTKEYACTIVE);
        result = SystemParametersInfo(SPI_SETSTICKYKEYS, 0, ref skey, 0);
        if (!result)
        {
            Debug.LogError("Failed to set sticky keys.");
            return 0;
        }

        return oldFlags;
    }

    public static void RestoreStickyKeys(uint oldFlags)
    {
        var skey = new SKEY { cbSize = (uint)Marshal.SizeOf(typeof(SKEY)), dwFlags = oldFlags };
        var result = SystemParametersInfo(SPI_SETSTICKYKEYS, 0, ref skey, 0);
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