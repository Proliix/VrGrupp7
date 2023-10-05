using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PotionColors
{
    public static Color waterSide = new Color(0, 1, 1);
    public static Color waterTop = new Color(0.5f, 1, 1);
    private static float waterWeight = 0.1f;

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
    public static PotionColor GetMixedColor(BaseAttribute baseAttribute)
    {
        Color sideColor = waterSide * waterWeight;
        Color topColor = waterTop * waterWeight;

        float weight = waterWeight;

        var color = baseAttribute.GetColor();
        sideColor += color.GetSideColor();
        topColor += color.GetTopColor();
        weight += color.GetWeight();

        sideColor /= weight;
        topColor /= weight;

        sideColor.a = 1;
        topColor.a = 1;

        return new PotionColor(sideColor, topColor);
    }

    public static PotionColor GetMixedColor(BaseAttribute[] baseAttributes)
    {
        Color sideColor = waterSide * waterWeight;
        Color topColor = waterTop * waterWeight;

        float weight = waterWeight;

        foreach (var baseAttribute in baseAttributes)
        {
            var color = baseAttribute.GetColor();
            sideColor += color.GetSideColor();
            topColor += color.GetTopColor();
            weight += color.GetWeight();
            //Debug.Log(baseAttribute.name + "Color: " + color.GetSideColor());
        }

        sideColor /= weight;
        topColor /= weight;

        sideColor.a = 1;
        topColor.a = 1;

        return new PotionColor(sideColor, topColor);
    }

    public static PotionColor GetColor(IAttribute attribute)
    {
        string type = attribute.GetType().Name;


        switch (type)
        {
            case "CustomGravity":
                { return new PotionColor(GravitySide, GravityTop); }
            case "Bouncy": 
                { return new PotionColor(BouncySide, BouncyTop); }
            case "Duplicator":
                { return new PotionColor(CloningSide, CloningTop); }
            case "Transparency": 
                { return new PotionColor(TransparencySide, TransparencyTop); }
            case "Explosive": 
                { return new PotionColor(ExplosiveSide, ExplosiveTop); }

            default:
                {
                    Debug.LogWarning("Unknown Color: " + type +  ", click me and fix");
                    return new PotionColor(Color.grey, Color.black);
                }
        }
    }
}
public class PotionColor
{
    private Color sideColor;
    private Color topColor;

    private float weight = 1f;

    public PotionColor(Color sideColor, Color topColor)
    {
        this.sideColor = sideColor;
        this.topColor = topColor;
        weight = 1f;
    }

    public void SetWeight(float weight)
    {
        this.weight = weight;
    }

    public float GetWeight()
    {
        return weight;
    }

    public Color GetTopColor()
    {
        return topColor * weight;
    }

    public Color GetSideColor()
    {
        return sideColor * weight;
    }
}
