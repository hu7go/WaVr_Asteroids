using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    int wv = 1;

    void Update()
    {
        if(Input.GetKeyDown("r"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Input.GetKeyDown("t"))
        {
            switch (wv)
            {
                case 1:
                    Manager.Instance.graphicsSettings.worldVersion = Manager.GraphicsSettings.WorldVersion.one;
                    break;
                case 2:
                    Manager.Instance.graphicsSettings.worldVersion = Manager.GraphicsSettings.WorldVersion.two;
                    break;
                case 3:
                    Manager.Instance.graphicsSettings.worldVersion = Manager.GraphicsSettings.WorldVersion.three;
                    wv = 0;
                    break;
            }

            Manager.Instance.SetWorldVersion();

            wv++;
        }
    }
}