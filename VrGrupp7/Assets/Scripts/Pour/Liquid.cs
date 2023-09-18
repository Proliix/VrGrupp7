using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class Liquid : MonoBehaviour
{
    public PourLiquid pourLiquid;

    [SerializeField] Vector3 scale;

    [SerializeField] Spline spline;
    [SerializeField] ExampleContortAlong contortAlong;

    [SerializeField] float speedDelta;
    [SerializeField] float animationSpeed;
    private float localAnimSpeed;

    private Vector3 targetScale;
    private float speedCurveLerp;
    private float length;

    private Vector3 startScale;
    private float meshLength;
    [SerializeField] private float maxSpeed = 10f;
    private bool flowWater = true;

    public void StartFlow()
    {
        StopAllCoroutines();
        flowWater = true;
        Init();
        StartCoroutine(Coroutine_WaterBend());
    }

    public void StopFlow()
    {
        flowWater = false;
    }


    void Init()
    {
        Debug.Log("Init");
        ConfigureSpline();

        contortAlong.Init();

        meshLength = contortAlong.MeshBender.Source.Length;
        meshLength = meshLength == 0 ? 1 : meshLength;

        localAnimSpeed = animationSpeed / scale.x;

        UpdateScale();

        speedCurveLerp = 0;
        length = 0;

        gameObject.SetActive(true);
        //spline.gameObject.SetActive(true);
    }

    public void UpdateScale()
    {

        Debug.Log("Update Scale");

        float scaleFactor = spline.Length * (1f / 3f);

        //Debug.Log("Update Scale");

        Vector3 localScale = scale;
        localScale.x = scale.x * scaleFactor;

        startScale = localScale;
        startScale.x = 0;
        targetScale = localScale;
    }

    IEnumerator Coroutine_WaterBend()
    {
        while (flowWater)
        {
            //Debug.Log("L/meshL: = " + length + " / " + meshLength + " = " + (length / meshLength));
            contortAlong.ScaleMesh(Vector3.Lerp(startScale, targetScale, length / meshLength));

            if (length < meshLength)
            {
                length += Time.deltaTime * localAnimSpeed * speedCurveLerp;
                speedCurveLerp = Mathf.Clamp(speedCurveLerp + speedDelta * Time.deltaTime, 0, maxSpeed);

                //Debug.Log("Scaling");
            }
            else
            {
                length = meshLength;
            }

            yield return null;
        }

        float count = 0;
        speedCurveLerp = 0;

        while (count < spline.Length)
        {

            contortAlong.Contort((count / spline.Length));

            count += Time.deltaTime * localAnimSpeed * speedCurveLerp;
            speedCurveLerp = Mathf.Clamp(speedCurveLerp + speedDelta * Time.deltaTime, 0, maxSpeed);
            //speedCurveLerp += _SpeedDelta * Time.deltaTime;
            yield return null;

        }


        gameObject.SetActive(false);
        //spline.gameObject.SetActive(false);
        //Destroy(gameObject, 2f);
    }
    private void ConfigureSpline()
    {
        //Debug.Log("ConfigureSpline");
        //LineRenderer line = TrajectoryProjector.instance.GetComponent<LineRenderer>();

        Vector3[] points = pourLiquid.splineTrajectory;

        Vector3 targetDirection = (pourLiquid.transform.up);
        transform.forward = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;

        List<SplineNode> nodes = new List<SplineNode>(spline.nodes);
        for (int i = 2; i < nodes.Count; i++)
        {
            spline.RemoveNode(nodes[i]);
        }

        int pointCount = pourLiquid.pointCount;

        for (int i = 0; i < pointCount; i++)
        {


            if (spline.nodes.Count <= i)
            {
                spline.AddNode(new SplineNode(Vector3.zero, Vector3.forward));
            }

            Vector3 pos = points[i];
            Vector3 myAngle = Vector3.zero;

            if (i == pointCount - 1)
                myAngle = -(points[i] - points[i - 1]);
            else
                myAngle = points[i] - points[i + 1];

            Vector3 normal = Quaternion.Euler(myAngle) * myAngle;
            Vector3 direction = pos - (normal / 2f);

            spline.nodes[i].Position = transform.InverseTransformPoint(pos);
            spline.nodes[i].Direction = transform.InverseTransformPoint(direction);

        }
    }

    public void UpdateSpline()
    {
        Vector3[] points = pourLiquid.splineTrajectory;

        //LineRenderer line = TrajectoryProjector.instance.GetComponent<LineRenderer>();

        int maxPoints = pourLiquid.LinePoints;

        for (int i = 0; i < maxPoints; i++)
        {
            Vector3 point = points[i];

            if (point == Vector3.zero)
                break;

            if (spline.nodes.Count <= i)
            {
                spline.AddNode(new SplineNode(Vector3.zero, Vector3.forward));
            }

            if (point == Vector3.zero && i > 2)
            {
                point = points[i - 1];
                point += point - points[i - 2];
            }

            Vector3 myAngle = Vector3.zero;

            if (points[i + 1] == Vector3.zero && i > 0)
                myAngle = -(points[i] - points[i - 1]);
            else
                myAngle = points[i] - points[i + 1];

            Vector3 normal = Quaternion.Euler(myAngle) * myAngle;
            Vector3 direction = point - (normal / 2f);

            spline.nodes[i].Position = transform.InverseTransformPoint(point);
            spline.nodes[i].Direction = transform.InverseTransformPoint(direction);
        }

        for (int i = spline.nodes.Count - 1; i > pourLiquid.pointCount - 1; i--)
        {
            spline.RemoveNode(spline.nodes[i]);
            //Debug.Log("points: " + line.positionCount);
            //Debug.Log("Removing node: " + i);
        }

        UpdateScale();
    }
}