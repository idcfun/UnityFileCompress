using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
public class RaycastTargetCancel : MonoBehaviour {
    
    [MenuItem("UITools/Raycast Target Cancel")]
    public static void CancelRaycastTarget()
    {
        Debug.Log(Application.persistentDataPath);
        Debug.Log(Application.temporaryCachePath);
        string genPath = Application.dataPath + "/Resources";
        string[] filesPath = Directory.GetFiles(genPath, "*.prefab", SearchOption.AllDirectories);
        for(int i = 0; i <filesPath.Length;i++)
        {
            filesPath[i] = filesPath[i].Substring(filesPath[i].IndexOf("Assets"));
            GameObject _prefab = AssetDatabase.LoadAssetAtPath(filesPath[i], typeof(GameObject)) as GameObject;
            GameObject prefabGameobject = PrefabUtility.InstantiatePrefab(_prefab) as GameObject;
            MaskableGraphic[] maskableGraphic = prefabGameobject.GetComponentsInChildren<MaskableGraphic>();
            for(int j = 0; j < maskableGraphic.Length; j++)
            {
                maskableGraphic[j].raycastTarget = false;
            }
            PrefabUtility.ReplacePrefab(prefabGameobject, _prefab, ReplacePrefabOptions.Default);
            MonoBehaviour.DestroyImmediate(prefabGameobject);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("成功", "HIT_DATA 添加完成！", "确定");
        }
    }
}
