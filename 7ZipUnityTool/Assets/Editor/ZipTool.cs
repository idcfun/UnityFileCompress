using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class ZipTool : EditorWindow {

    [MenuItem("Tools/ZipTool")]
    public static void Compress()
    {
        CreateInstance<ZipTool>().Show();
    }

    private string url = "Please Input File Path Which Need Compress";
    private void OnGUI()
    {
        if (GUILayout.Button("Compress"))
        {
            url = EditorUtility.OpenFolderPanel("test", "", "");
            string[] files = Directory.GetFiles(url, "*.cs", SearchOption.AllDirectories);
            ZipTask.StartCompress(url, files, Application.streamingAssetsPath + "/" + Path.GetFileName(url) + ".zip");
            AssetDatabase.Refresh();
        }

        if(GUILayout.Button("Uncompress"))
        {
            url = EditorUtility.OpenFilePanel("test", "", "");
            ZipTask.StartDecompress(url);
            AssetDatabase.Refresh();
        }

        this.Repaint();
    }
}
