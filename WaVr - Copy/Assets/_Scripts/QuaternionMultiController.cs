using UnityEngine;

public class QuaternionMultiController : MonoBehaviour
{
    public GameObject Alpha;
    public GameObject Beta;
    public GameObject Result_A_B;
    public GameObject Result_B_A;

    void Start()
    {
        Alpha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Alpha.transform.position = new Vector3(-3.0f, 0.0f, 0.0f);
        Alpha.transform.rotation = Quaternion.identity;
        Alpha.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        Alpha.name = "Alpha";

        Beta = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Beta.transform.position = new Vector3(-1.0f, 0.0f, 0.0f);
        Beta.transform.rotation = Quaternion.identity;
        Beta.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        Beta.name = "Beta";

        Result_A_B = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Result_A_B.transform.position = new Vector3(1.0f, 0.0f, 0.0f);
        Result_A_B.transform.rotation = Quaternion.identity;
        Result_A_B.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        Result_A_B.name = "Result_A_B";

        Result_B_A = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Result_B_A.transform.position = new Vector3(3.0f, 0.0f, 0.0f);
        Result_B_A.transform.rotation = Quaternion.identity;
        Result_B_A.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        Result_B_A.name = "Result_B_A";
    }

    void Update()
    {
        Quaternion alphaQuaternion = Alpha.transform.rotation;
        Quaternion betaQuaternion = Beta.transform.rotation;

        Result_A_B.transform.rotation = alphaQuaternion * betaQuaternion;
        Result_B_A.transform.rotation = betaQuaternion * alphaQuaternion;
    }

    void OnGUI()
    {
        Quaternion alphaQuaternion = Alpha.transform.rotation;
        Quaternion betaQuaternion = Beta.transform.rotation;

        Quaternion abQuaternion = Result_A_B.transform.rotation;
        Quaternion baQuaternion = Result_B_A.transform.rotation;

        int row = 0;
        int pad = 20;

        GUI.Label(new Rect(10, row += pad, 1000, 20), alphaQuaternion.ToString());
        GUI.Label(new Rect(10, row += pad, 1000, 20), betaQuaternion.ToString());
        GUI.Label(new Rect(10, row += pad, 1000, 20), abQuaternion.ToString());
        GUI.Label(new Rect(10, row += pad, 1000, 20), baQuaternion.ToString());
    }
}