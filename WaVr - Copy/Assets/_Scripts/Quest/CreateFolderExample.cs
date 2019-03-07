using UnityEngine;
using UnityEditor;

public class CreateFolderExample : MonoBehaviour
{
    [MenuItem("GameObject/Create Folder")]
    private void Start()
    {
        if (!AssetDatabase.IsValidFolder("Assets/HUGOMAPPEN"))
        {
            string guid = AssetDatabase.CreateFolder("Assets", "HUGOMAPPEN");
            string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
        }
    }
}