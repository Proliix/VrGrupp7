using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PotionColors
{
    public static Color GravitySide = new Color(1, 0, 0);
    public static Color GravityTop = new Color(1, 0.5037735f, 0.5037735f);
    public static Color BouncySide = new Color(0.3209854f, 1, 0);
    public static Color BouncyTop = new Color(0.6886027f, 1, 0.5019608f);
    public static Color CloningSide = new Color(1, 0, 0.9955863f);
    public static Color CloningTop = new Color(1, 0.5019608f, 0.995156f);
    public static Color TransparencySide = new Color(0, 0.3494084f, 1);
    public static Color TransparencyTop = new Color(0.5019608f, 0.6689681f, 1);
    public static Color ExplosiveSide = new Color(1, 0.4757203f, 0);
    public static Color ExplosiveTop = new Color(1, 0.6951825f, 0.5019608f);
    public static Color CombineColors(Color[] aColors)
    {
        Color result = new Color(0, 0, 0, 0);
        foreach (Color c in aColors)
        {
            result += c;
        }
        result /= aColors.Length;
        return result;
    }
}
