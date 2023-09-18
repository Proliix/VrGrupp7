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


    [SerializeField]
    public Transform pourPosition;
    [SerializeField]
    [Range(1, 100)]
    private float PourStrength = 2f;
    [Header("Display Controls")]
    [SerializeField]
    [Range(10, 100)]
    public int LinePoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    public float TimeBetweenPoints = 0.05f;

    public int pointCount;

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

            if (Physics.Raycast(lastPosition,
                (point - lastPosition).normalized,
                out RaycastHit hit,
                (point - lastPosition).magnitude)) // Add collision mask?
            {

                currentTrajectory[i] = point;
                i++;

                pointCount = i;

                // Clear Unused LinePoints
                while (LinePoints > i)
                {
                    currentTrajectory[i] = Vector3.zero;
                    i++;
                }

                return;
            }
        }
    }

    public void Pour()
    {
        liquid = LiquidObjectPool.instance.GetLiquid();
        liquid.pourLiquid = this;

        liquid.StartFlow();
    }

    public void Stop()
    {
        liquid.StopFlow();
        LiquidObjectPool.instance.ReturnLiquid(liquid);
        liquid = null;
    }
}