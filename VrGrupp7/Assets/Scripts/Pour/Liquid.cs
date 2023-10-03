using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class Liquid : MonoBehaviour
{
    public PourLiquid pourLiquid;

    [SerializeField] Vector3 scale;

    [SerializeField] Material material;
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

    public ParticleSystem ps_waterSplash;
    Vector3 impactPos;
    Vector3 impactUp;

    public void StartFlow(Color color)
    {
        Debug.Log(gameObject.name + ": Starting Flow");
        StopAllCoroutines();
        flowWater = true;

        SetColor(color);
        Init();
        StartCoroutine(Coroutine_WaterFlow());
    }

    public void StopFlow()
    {
        Debug.Log(gameObject.name + ": Stopping flow");
        flowWater = false;
    }


    void Init()
    {
        //Debug.Log("Init");
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
        //Scale the mesh to the length of the spline
        float scaleFactor = spline.Length * (1f / meshLength);

        //Debug.Log("Update Scale");

        Vector3 localScale = scale;
        localScale.x = scale.x * scaleFactor;

        startScale = localScale;
        startScale.x = 0;
        targetScale = localScale;
    }

    IEnumerator Coroutine_WaterFlow()
    {
        //Debug.Log(gameObject.name + ": Flow Started");
        while (flowWater)
        {
            //Moves the mesh along the spline by scaling it, targetScale updates depending on the length of the spline
            contortAlong.ScaleMesh(Vector3.Lerp(startScale, targetScale, length / meshLength));

            //Increments the lerp value if we haven't reached a lerp value of 1;
            if (length < meshLength)
            {
                length += Time.deltaTime * localAnimSpeed * speedCurveLerp;
                speedCurveLerp = Mathf.Clamp(speedCurveLerp + speedDelta * Time.deltaTime, 0, maxSpeed);

                //Debug.Log("Scaling");

                //if (ps_waterSplash.isPlaying)
                //    ps_waterSplash.Stop();

            }
            else
            {
                //if(ps_waterSplash.isStopped)
                //    ps_waterSplash.Play();

                length = meshLength;
            }

            ps_waterSplash.transform.position = impactPos;
            ps_waterSplash.transform.up = impactUp;

            yield return null;
        }

        float count = 0;
        speedCurveLerp = 0;

        //if (ps_waterSplash.isStopped)
        //    ps_waterSplash.Play();

        while (count < spline.Length)
        {

            contortAlong.Contort((count / spline.Length));

            count += Time.deltaTime * localAnimSpeed * speedCurveLerp;
            speedCurveLerp = Mathf.Clamp(speedCurveLerp + speedDelta * Time.deltaTime, 0, maxSpeed);
            //speedCurveLerp += _SpeedDelta * Time.deltaTime;
            yield return null;

        }

        Debug.Log(gameObject.name + ": Flow Stopped");
        pourLiquid?.ReturnLiquid();
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

        UpdateSpline();
    }

    public void UpdateSpline()
    {
        if (!flowWater)
        {
            return;
        }

        Vector3[] points = pourLiquid.splineTrajectory;

        //If the spline trajectory is the shortest possible, containing only a start point and end point, we process it manually
        if (points[2] == Vector3.zero)
        {
            Vector3 point = points[0];
            Vector3 myAngle = points[0] - points[1];

            Vector3 normal = Quaternion.Euler(myAngle) * myAngle;
            Vector3 direction = point - (normal / 3f);

            spline.nodes[0].Position = transform.InverseTransformPoint(points[0]);
            spline.nodes[0].Direction = transform.InverseTransformPoint(direction);

            spline.nodes[1].Position = transform.InverseTransformPoint(points[1]);
            spline.nodes[1].Direction = transform.InverseTransformPoint(direction);

            RemoveUnusedSplines(2);

            impactPos = spline.nodes[1].Position;
            impactUp = spline.nodes[1].Direction;

            return;
        }

        int maxPoints = pourLiquid.LinePoints;
        int nodeCount = 0;

        for (int i = 0; i < maxPoints; i += 2, nodeCount++)
        {
            Vector3 point = points[i];

            //Logic for detecting when we reached the end of the spline trajectory
            if (point == Vector3.zero)
            {
                //If we stopped on an uneven point (we have i += 2) we register the middle step as a node instead
                if (points[i - 1] != Vector3.zero)
                {
                    impactPos = points[i - 1];
                    impactUp = points[i - 2] - points[i - 1];
                    i--;
                    maxPoints = 0;
                }
                //We have stopped at correct point and just need to get the impact position
                else
                {
                    impactPos = points[i - 2];
                    impactUp = points[i - 3] - points[i - 2];
                    break;
                }

            }

            if (spline.nodes.Count <= nodeCount)
            {
                spline.AddNode(new SplineNode(Vector3.zero, Vector3.forward));
            }

            if (point == Vector3.zero && i > 2)
            {
                point = points[i - 1];
                point += point - points[i - 2];
            }

            Vector3 myAngle;

            if (points[i + 1] == Vector3.zero && i > 0)
                myAngle = -(points[i] - points[i - 1]);
            else
                myAngle = points[i] - points[i + 1];

            Vector3 normal = Quaternion.Euler(myAngle) * myAngle;
            Vector3 direction = point - (normal / 2f);


            spline.nodes[nodeCount].Position = transform.InverseTransformPoint(point);
            spline.nodes[nodeCount].Direction = transform.InverseTransformPoint(direction);
        }

        RemoveUnusedSplines(nodeCount);

        UpdateScale();
    }

    void RemoveUnusedSplines(int nodeCount)
    {
        for (int i = spline.nodes.Count - 1; i > nodeCount - 1; i--)
        {
            spline.RemoveNode(spline.nodes[i]);
            //Debug.Log("points: " + line.positionCount);
            //Debug.Log("Removing node: " + i);
        }
    }

    public void SetColor(Color color)
    {
        Material newMaterial = new Material(material);

        newMaterial.SetColor("_Color", color);

        contortAlong.material = newMaterial;

        if (ps_waterSplash != null)
        {
            float particleColorGradient = 0.2f;
            //Create a light/dark gradient from our gameobject color
            var gradient = new ParticleSystem.MinMaxGradient(color * (1 - particleColorGradient), color * (1 + particleColorGradient));
            var main = ps_waterSplash.main;
            //Set particle color to gradient;
            main.startColor = gradient;

        }
    }
}
