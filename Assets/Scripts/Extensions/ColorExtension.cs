using UnityEngine;

public static class ColorExtension
{
	public static Color SetAlpha(this Color color, float alpha)
    {
        return new(color.r, color.g, color.b, alpha);
    }
}
