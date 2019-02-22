using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBender : MonoBehaviour
{
    [HideInInspector] public Vector3 targetRange;
    public List<Transform> points;
    public int vertexCount = 12;
    private Vector3 target;
    public Transform ogPos;

    [HideInInspector] public LineRenderer render;
    private Vector3 tangentLineVertex1;
    private Vector3 tangentLineVertex2;
    private Vector3 bezierPoint;
    private bool snapped = false;
    private float percent;

    private TeleportRotation teleport;

    private Vector3 point;

    void Start()
    {
        render = GetComponent<LineRenderer>();
        teleport = GetComponent<TeleportRotation>();

        targetRange.z = teleport.master.GetMaxLenght();

        percent = .9f;

        Vector3 tmp = points[2].position;
        tmp.z = teleport.master.GetMaxLenght();
        points[2].position = tmp;

        Vector3 tmp2 = ogPos.position;
        tmp2.z = teleport.master.GetMaxLenght();
        ogPos.position = tmp2;
    }

    private void Update()
    {
        if (teleport.renderLine)
        {
            if (snapped == false)
            {
                points[1].position = transform.position;
            }
            else
            {
                points[1].position = point;
                points[2].position = target;
            }

            List<Vector3> pointList = new List<Vector3>();

            for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
            {
                tangentLineVertex1 = Vector3.Lerp(points[0].position, points[1].position, ratio);
                tangentLineVertex2 = Vector3.Lerp(points[1].position, points[2].position, ratio);
                bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);

                pointList.Add(bezierPoint);
            }
            render.positionCount = pointList.Count;
            render.SetPositions(pointList.ToArray());
        }
    }

    public void ChangeLineVersion()
    {
        switch (teleport.lineVersion)
        {
            case TeleportRotation.LineVersion.nothing:
                render.material.SetColor("_BaseColor", Color.cyan);
                break;
            case TeleportRotation.LineVersion.outOfRange:
                render.material.SetColor("_BaseColor", Color.red);
                break;
            case TeleportRotation.LineVersion.hit:
                render.material.SetColor("_BaseColor", Color.green);
                break;
        }
    }

    public void SetLineEnd(Vector3 newTarget, Vector3 newPoint)
    {
        target = newTarget;
        point = newPoint;
        if (Manager.Instance.bendLine && Manager.Instance.enums.pointerState == Manager.Enums.PointerState.Teleport)
            snapped = true;
    }

    public void SetEnd (Vector3 newTarget)
    {
        points[2].position = newTarget;
    }

    public void ResetLineEnd()
    {
        snapped = false;
    }

    public void StraightRenderer ()
    {
        snapped = false;
        points[2].position = ogPos.position;
    }

    public void TargetToggle ()
    {
        snapped = !snapped;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(points[0].position, points[1].position);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(points[1].position, points[2].position);

        Gizmos.color = Color.red;
        for (float ratio = 0.5f / vertexCount; ratio < 1; ratio += 1.0f / vertexCount)
        {
            Gizmos.DrawLine(Vector3.Lerp(points[0].position, points[1].position, ratio), Vector3.Lerp(points[1].position, points[2].position, ratio));
        }
    }
}
