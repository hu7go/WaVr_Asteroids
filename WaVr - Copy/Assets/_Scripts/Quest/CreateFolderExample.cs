using UnityEngine;
using UnityEditor;

public class CreateFolderExample : MonoBehaviour
{
    [MenuItem("GameObject/Create Folder")]
    private void Start()
    {
        if (AssetDatabase.IsValidFolder("Assets/Scripts"))
        {
            FileUtil.DeleteFileOrDirectory("Assets/Scripts");
        }
        if (AssetDatabase.IsValidFolder("Assets/AsteroidPack"))
        {
            FileUtil.DeleteFileOrDirectory("Assets/AsteroidPack");
        }
    }
}