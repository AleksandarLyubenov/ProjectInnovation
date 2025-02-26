using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NFCTagUID))]
public class UIDEditor : Editor
{
    private SerializedProperty tagUIDs;

    private void OnEnable()
    {
        tagUIDs = serializedObject.FindProperty("tagUIDs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("NFC Tag UIDs", EditorStyles.boldLabel);

        for (int i = 0; i < tagUIDs.arraySize; i++)
        {
            SerializedProperty tag = tagUIDs.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(tag, new GUIContent($"Tag {i + 1}"));

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                tagUIDs.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add NFC Tag"))
        {
            tagUIDs.InsertArrayElementAtIndex(tagUIDs.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
