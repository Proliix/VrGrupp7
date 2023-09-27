using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JobManager))]
public class JobManagerEditor : Editor
{

    bool foldOut = true;
    public override void OnInspectorGUI()
    {
        JobManager manager = (JobManager)target;
        List<WantedAttribute> attributes = manager.wantedAtributes;

        if (attributes.Count > 0)
        {
            foldOut = EditorGUILayout.Foldout(foldOut, "Wanted Attributes");
            if (foldOut)
            {
                for (int i = 0; i < attributes.Count; i++)
                {
                    EditorGUILayout.LabelField("Type: " + attributes[i].Attribute.GetName());
                    EditorGUILayout.LabelField("Wants " + (attributes[i].potency * 100) + "% " + "potency" + " with a moe at: " + attributes[i].marginOfError + "%");
                    Rect rect = EditorGUILayout.GetControlRect(false, 2);
                    rect.height = 1.5f;
                    EditorGUI.DrawRect(rect, new Color(0.33f, 0.33f, 0.33f, 1));
                    EditorGUILayout.Space();
                }
            }
        }
        base.OnInspectorGUI();
    }
}
