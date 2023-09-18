using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrajectoryProjector : MonoBehaviour
{
    public static TrajectoryProjector instance;

    public WaterBendingControll1 WaterBending;

    public LinkedList<Vector3[]> linkedList = new LinkedList<Vector3[]>();
    public Vector3[] splineTrajectory;
    public Vector3[] currentTrajectory;

    float timer = 0;
    [Range(0.5f, 2f)]public float simulationSpeed = 1f;


    private LineRenderer LineRenderer;
    [SerializeField]
    public Transform ReleasePosition;
    [Header("Grenade Controls")]
    [SerializeField]
    [Range(1, 100)]
    private float ThrowStrength = 10f;
    [Header("Display Controls")]
    [SerializeField]
    [Range(10, 100)]
    private int LinePoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    public float TimeBetweenPoints = 0.1f;

    int lastPointCount;

    private Transform InitialParent;
    private Vector3 InitialLocalPosition;
    private Quaternion InitialRotation;

    private bool IsGrenadeThrowAvailable = true;
    private LayerMask GrenadeCollisionMask;

    private void Awake()
    {
        instance = this;
        //InitialParent = Grenade.transform.parent;
        //InitialRotation = Grenade.transform.localRotation;
        //InitialLocalPosition = Grenade.transform.localPosition;
        //Grenade.freezeRotation = true;

        //int grenadeLayer = Grenade.gameObject.layer;

        LineRenderer = GetComponent<LineRenderer>();


        splineTrajectory = new Vector3[LinePoints];
        currentTrajectory = new Vector3[LinePoints];

        lastPointCount = LinePoints;
    }

    private void Update()
    {
        if(timer > TimeBetweenPoints)
        {
            timer %= TimeBetweenPoints;

            RecordPositions();
            WaterBending.UpdateSpline();
        }

        timer += Time.deltaTime * simulationSpeed;

        //DrawProjection();
    }

    void RecordPositions()
    {
        DrawProjection();

        Vector3[] copy = (Vector3[])currentTrajectory.Clone();
        linkedList.AddFirst(copy);

        //Debug.Log("Recording position: " + linkedList.Count);
        
        for (int i = 0; i < linkedList.Count && i < LinePoints; i++)
        {
            Vector3[] nthTrajectory = linkedList.ElementAt(i);
            if(nthTrajectory == null)
            {
                return;
            }

            Vector3 point = nthTrajectory[i];
            splineTrajectory[i] = point;
        }

        if(linkedList.Count > LinePoints)
        {
            linkedList.RemoveLast();
        }
    }

    private void DrawProjection()
    {

        float mass = 1;

        LineRenderer.enabled = true;
        LineRenderer.positionCount = Mathf.CeilToInt(LinePoints / TimeBetweenPoints) + 1;
        Vector3 startPosition = ReleasePosition.position;
        Vector3 startVelocity = ThrowStrength * transform.up / mass;
        int i = 0;
        LineRenderer.SetPosition(i, startPosition);
        currentTrajectory[i] = startPosition;

        for (float time = TimeBetweenPoints; time < LinePoints; time += TimeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            currentTrajectory[i] = point;
            LineRenderer.SetPosition(i, point);

            Vector3 lastPosition = LineRenderer.GetPosition(i - 1);

            if (Physics.Raycast(lastPosition,
                (point - lastPosition).normalized,
                out RaycastHit hit,
                (point - lastPosition).magnitude)) // Add collision mask?
            {
                LineRenderer.SetPosition(i, hit.point);
                LineRenderer.positionCount = i + 1;
                currentTrajectory[i] = point;

                i++;
                while(LinePoints > i)
                {
                    currentTrajectory[i] = Vector3.zero;
                    i++;
                }

                return;
            }
        }


    }
}