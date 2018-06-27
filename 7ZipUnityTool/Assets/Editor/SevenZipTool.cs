using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SevenZipTool : EditorWindow {

    [MenuItem("Tools/Compress")]
    public static void Compress()
    {
        EditorWindow.CreateInstance<SevenZipTool>().Show();
    }

    private string url = "Please Input File Path Which Need Compress";
    private void OnGUI()
    {
        url = EditorGUILayout.TextField(url);
        if(GUILayout.Button("Compress"))
        {
            SevenZipTask.StartCompress(new string[] { url}, "");
        }
    }
}
