using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;


[CustomEditor(typeof(ListLevel))]
public class LevelDataEditor : Editor
{
    private ReorderableList list;

    const float height = 100;

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("names"));

        list.elementHeightCallback =  (x) => { return height; } ;

        list.drawElementCallback =
        (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 150, EditorGUIUtility.singleLineHeight),
                element, GUIContent.none);


            string path = element.stringValue.Replace(".txt", "");

            Texture texture = Resources.Load<Texture>(path);
      
            if (texture)
                EditorGUI.DrawPreviewTexture(
                    new Rect(rect.x + 160, rect.y, height-5, height-5),//EditorGUIUtility.singleLineHeight),
                texture);


        };


        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Levels names");
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

}
