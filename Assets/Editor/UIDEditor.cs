using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NFCTagUID))]
public class UIDEditor : Editor
{
    private SerializedProperty tagData;

    private void OnEnable()
    {
        tagData = serializedObject.FindProperty("tagData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("NFC Tag Data", EditorStyles.boldLabel);

        for (int i = 0; i < tagData.arraySize; i++)
        {
            SerializedProperty tag = tagData.GetArrayElementAtIndex(i);
            SerializedProperty uid = tag.FindPropertyRelative("uid");
            SerializedProperty itemName = tag.FindPropertyRelative("itemName");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Tag {i + 1}", GUILayout.Width(60));
            EditorGUILayout.PropertyField(uid, GUIContent.none);
            EditorGUILayout.PropertyField(itemName, GUIContent.none);
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                tagData.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                break;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add NFC Tag"))
        {
            tagData.InsertArrayElementAtIndex(tagData.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}