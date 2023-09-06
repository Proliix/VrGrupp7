using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit;

[CustomEditor(typeof(Socket_with_tag_check))]
public class Socket_with_tag_Editor : XRSocketInteractorEditor
{
    private SerializedProperty targetTag = null;

    protected override void OnEnable() 
    {
        base.OnEnable();
        targetTag = serializedObject.FindProperty("targetTag");
    }

    protected override void DrawProperties()
    {
        base.DrawProperties();
        EditorGUILayout.PropertyField(targetTag);
    }

}
