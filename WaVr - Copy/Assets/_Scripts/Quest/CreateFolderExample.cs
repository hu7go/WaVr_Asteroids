using UnityEngine;
using UnityEditor;

public class CreateFolderExample : MonoBehaviour
{
    [MenuItem("GameObject/Create Folder")]
    private void Start()
    {
        print("step one");
        if(!AssetDatabase.IsValidFolder("Assets/HUGOMAPPEN"))
        {
            print("Step Two");
            string guid = AssetDatabase.CreateFolder("Assets", "HUGOMAPPEN");
            string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
        }
    }
}