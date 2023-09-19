using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PourLiquid : MonoBehaviour
{
    private Liquid liquid;

    public LinkedList<Vector3[]> linkedList = new LinkedList<Vector3[]>();
    public Vector3[] splineTrajectory;
    public Vector3[] currentTrajectory;

    float timer = 0;
    [Range(0.5f, 2f)] public float simulationSpeed = 2f;

    [Header("Pour Controls")]
    [SerializeField]
    public Transform pourPosition;
    [SerializeField]
    [Range(1, 10)]
    private float pourStrengthGain = 3;
    [SerializeField]
    [Range(1, 5)]
    private float maxPourStrength = 2;
    private float PourStrength = 2f;
    [Header("Display Controls")]
    [SerializeField]
    [Range(10, 100)]
    public int LinePoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    public float TimeBetweenPoints = 0.05f;

    public int pointCount;

    float pourStrengthLimiter = 1;

    private LayerMask PourCollisionMask;

    private void Awake()
    {

        splineTrajectory = new Vector3[LinePoints];
        currentTrajectory = new Vector3[LinePoints];

    }

    private void Start()
    {

        
    }

    private void Update()
    {
        if (timer > TimeBetweenPoints)
        {
            timer %= TimeBetweenPoints;

            RecordPositions();

            if(liquid != null)
                liquid.UpdateSpline();

            //DrawDebugLines(splineTrajectory, Color.red, TimeBetweenPoints);
        }

        timer += Time.deltaTime * simulationSpeed;
    }

    void RecordPositions()
    {
        DrawProjection();

        Vector3[] copy = (Vector3[])currentTrajectory.Clone();
        linkedList.AddFirst(copy);

        for (int i = 0; i < linkedList.Count && i < LinePoints; i++)
        {
            Vector3[] nthTrajectory = linkedList.ElementAt(i);
            if (nthTrajectory == null)
            {
                return;
            }

            Vector3 point = nthTrajectory[i];
            splineTrajectory[i] = point;
        }

        if (linkedList.Count > LinePoints)
        {
            linkedList.RemoveLast();
        }
    }

    private void DrawProjection()
    {
        float mass = 1;

        Vector3 startPosition = pourPosition.position;
        Vector3 startVelocity = PourStrength * transform.up / mass;
        int i = 0;

        currentTrajectory[i] = startPosition;

        for (float time = TimeBetweenPoints; i < LinePoints - 1; time += TimeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            currentTrajectory[i] = point;

            Vector3 lastPosition = currentTrajectory[i - 1];

            bool collided = Physics.Raycast(lastPosition, point - lastPosition, out RaycastHit hit, (point - lastPosition).magnitude); //Add collision Mask?

            if (collided && hit.collider.gameObject != gameObject)
            {

                currentTrajectory[i] = hit.point;
                i++;

                pointCount = i;

                // Clear Unused LinePoints
                while (LinePoints > i)
                {
                    currentTrajectory[i] = Vector3.zero;
                    i++;
                }

                //DrawDebugLines(currentTrajectory, Color.white, TimeBetweenPoints * 5);

                return;
            }
        }
    }

    public void Pour(Color color)
    {
        liquid = LiquidObjectPool.instance.GetLiquid();
        liquid.pourLiquid = this;

        liquid.StartFlow(color);
    }

    public void Stop()
    {
        liquid.StopFlow();
        LiquidObjectPool.instance.ReturnLiquid(liquid);
        liquid = null;

        pourStrengthLimiter = 1;
    }

    void DrawDebugLines(Vector3[] points, Color color, float duration)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i + 1] == Vector3.zero)
                break;

            Debug.DrawLine(points[i], points[i + 1], color, duration);

        }
    }

    public void SetPourStrength(float tilt)
    {
        pourStrengthLimiter -= Time.deltaTime;
        //Magic numbers
        PourStrength = Mathf.Clamp((tilt - 1) * pourStrengthGain, 0.1f, maxPourStrength - Mathf.Clamp01(pourStrengthLimiter)* (maxPourStrength / 3));
    }
}