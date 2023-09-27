using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LiquidEffect))]
public class LiquidEditor : Editor
{
    private void OnEnable()
    {
        LiquidEffect effect = (LiquidEffect)target;
        effect.isSelected = true;
    }

    private void OnDisable()
    {
        LiquidEffect effect = (LiquidEffect)target;
        effect.isSelected = false;
    }
}
