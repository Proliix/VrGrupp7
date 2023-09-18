using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
public class WaterBendingControll1 : MonoBehaviour
{
    [SerializeField] Vector3 _Scale;

    [SerializeField] Spline _Spline;
    [SerializeField] ExampleContortAlong _ContortAlong;

    [SerializeField] float _SpeedDelta;
    [SerializeField] float _AnimSpeed;
    private float localAnimSpeed;

    private Vector3 targetScale;
    private float speedCurveLerp;
    private float length;

    private Vector3 startScale;
    private float meshLength;
    [SerializeField] private float maxSpeed = 10f;
    bool flowWater = true;

    private void Start()
    {
        //Time.timeScale = 0.05f;
        TrajectoryProjector.instance.WaterBending = this;
        StartFlow();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartFlow();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            StopFlow();
        }
    }

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

        LineRenderer line = TrajectoryProjector.instance.GetComponent<LineRenderer>();

        _ContortAlong.Init();

        meshLength = _ContortAlong.MeshBender.Source.Length;
        meshLength = meshLength == 0 ? 1 : meshLength;

        localAnimSpeed = _AnimSpeed / _Scale.x;

        UpdateScale();

        speedCurveLerp = 0;
        length = 0;

        _Spline.gameObject.SetActive(true);
    }

    public void UpdateScale()
    {

        Debug.Log("Update Scale");

        float scaleFactor = _Spline.Length * (1f/3f);

        //Debug.Log("Update Scale");

        Vector3 localScale = _Scale;
        localScale.x = _Scale.x * scaleFactor;

        startScale = localScale;
        startScale.x = 0;
        targetScale = localScale;
    }

    IEnumerator Coroutine_WaterBend()
    {
        while(flowWater)
        {
            //Debug.Log("L/meshL: = " + length + " / " + meshLength + " = " + (length / meshLength));
            _ContortAlong.ScaleMesh(Vector3.Lerp(startScale, targetScale, length / meshLength));

            if (length < meshLength)
            {
                length += Time.deltaTime * localAnimSpeed * speedCurveLerp;
                speedCurveLerp = Mathf.Clamp(speedCurveLerp + _SpeedDelta * Time.deltaTime, 0, maxSpeed);

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

        while (count < _Spline.Length)
        {

            _ContortAlong.Contort((count / _Spline.Length));

            count += Time.deltaTime * localAnimSpeed * speedCurveLerp;
            speedCurveLerp =  Mathf.Clamp(speedCurveLerp + _SpeedDelta * Time.deltaTime, 0, maxSpeed);
            //speedCurveLerp += _SpeedDelta * Time.deltaTime;
            yield return null;
        }
        _Spline.gameObject.SetActive(false);
        //Destroy(gameObject, 2f);
    }
    private void ConfigureSpline()
    {
        //Debug.Log("ConfigureSpline");
        LineRenderer line = TrajectoryProjector.instance.GetComponent<LineRenderer>();

        Vector3[] points = TrajectoryProjector.instance.splineTrajectory;

        Vector3 targetDirection = (TrajectoryProjector.instance.transform.up);
        transform.forward = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;

        List<SplineNode> nodes = new List<SplineNode>(_Spline.nodes);
        for (int i = 2; i < nodes.Count; i++)
        {
            _Spline.RemoveNode(nodes[i]);
        }

        int _PointCount = line.positionCount;

        for (int i = 0; i < _PointCount; i++)
        {


            if (_Spline.nodes.Count <= i)
            {
                _Spline.AddNode(new SplineNode(Vector3.zero, Vector3.forward));
            }

            Vector3 pos = points[i];
            Vector3 myAngle = Vector3.zero;

            if (i == _PointCount - 1)
                myAngle = -(points[i] - points[i - 1]);
            else
                myAngle = points[i] - points[i + 1];

            Vector3 normal = Quaternion.Euler(myAngle) * myAngle;
            Vector3 direction = pos - (normal / 2f);

            _Spline.nodes[i].Position = transform.InverseTransformPoint(pos);
            _Spline.nodes[i].Direction = transform.InverseTransformPoint(direction);

        }
    }

    public void UpdateSpline()
    {
        Vector3[] points = TrajectoryProjector.instance.splineTrajectory;

        LineRenderer line = TrajectoryProjector.instance.GetComponent<LineRenderer>();

        int maxPoints = 25;

        for (int i = 0; i < maxPoints; i++)
        {
            Vector3 point = points[i];

            if (point == Vector3.zero)
                break;

            if (_Spline.nodes.Count <= i)
            {
                _Spline.AddNode(new SplineNode(Vector3.zero, Vector3.forward));
            }

            if(point == Vector3.zero && i > 2)
            {
                point = points[i - 1];
                point += point - points[i - 2];
            }

            Vector3 myAngle = Vector3.zero;

            if (points[i+1] == Vector3.zero && i > 0)
                myAngle = -(points[i] - points[i - 1]);
            else
                myAngle = points[i] - points[i + 1];

            Vector3 normal = Quaternion.Euler(myAngle) * myAngle;
            Vector3 direction = point - (normal / 2f);

            _Spline.nodes[i].Position = transform.InverseTransformPoint(point);
            _Spline.nodes[i].Direction = transform.InverseTransformPoint(direction);
        }

        for (int i = _Spline.nodes.Count - 1; i > line.positionCount - 1; i--)
        {
            _Spline.RemoveNode(_Spline.nodes[i]);
            //Debug.Log("points: " + line.positionCount);
            //Debug.Log("Removing node: " + i);
        }

        UpdateScale();
    }
}
