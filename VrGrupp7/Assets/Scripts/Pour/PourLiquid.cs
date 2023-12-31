using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PourLiquid : MonoBehaviour
{
    private Liquid liquid;

    public LinkedList<Vector3[]> trajectories = new LinkedList<Vector3[]>();
    public Vector3[] splineTrajectory;
    public Vector3[] currentTrajectory;

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

    //This can be used to allow liquid to pour through objects;
    [SerializeField] private LayerMask PourCollisionMask;

    [Header("Display Controls")]
    [SerializeField]
    [Range(10, 100)]
    public int LinePoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    public float TimeBetweenPoints = 0.05f;

    public int pointCount;

    private float pourStrengthLimiter = 1;
    private float liquidLost = 0;
    private bool isPouring;
    private IEnumerator couroutine_Flowing;

    [Header("If used as dispenser")]
    [SerializeField] private LiquidDispenser liquidDispenser;

    private void Awake()
    {
        splineTrajectory = new Vector3[LinePoints];
        currentTrajectory = new Vector3[LinePoints];

        if (transform.parent != null)
        {
            var dispenser = transform.parent.GetComponentInChildren<LiquidDispenser>();

            if (dispenser != null)
            {
                liquidDispenser = dispenser;
            }
        }
    }

    IEnumerator Couroutine_StartFlow(Color color)
    {
        //Wait for a liquid from the object pool
        while (liquid == null)
        {
            liquid = LiquidObjectPool.instance.GetLiquid();


            if (liquid == null)
                yield return new WaitForSeconds(TimeBetweenPoints);
            else
                Debug.Log(transform.name + " took " + liquid.transform.name + " from object pool");
        }

        //set the liquids pourLiquid reference to this script
        liquid.pourLiquid = this;

        isPouring = true;

        //Record a trajectory for the liquid to follow
        RecordPositions();
        //Temporarly assign the current trajectory to the splineTrajectory to let the liquid have a trajectory to follow
        splineTrajectory = currentTrajectory;
        //Start the flow
        liquid.StartFlow(color);

        while (isPouring)
        {
            yield return new WaitForSeconds(TimeBetweenPoints / simulationSpeed);

            RecordPositions();

            //Sometimes is called after we cancel the couroutine
            if (liquid != null)
                liquid.UpdateSpline();
        }
    }

    void RecordPositions()
    {
        DrawProjection();

        Vector3[] copy = (Vector3[])currentTrajectory.Clone();
        trajectories.AddFirst(copy);

        for (int i = 0; i < trajectories.Count && i < LinePoints; i++)
        {
            Vector3[] nthTrajectory = trajectories.ElementAt(i);
            if (nthTrajectory == null)
            {
                return;
            }

            Vector3 point = nthTrajectory[i];
            splineTrajectory[i] = point;
        }

        if (trajectories.Count > LinePoints)
        {
            trajectories.RemoveLast();
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

            bool collided = Physics.Raycast(lastPosition, point - lastPosition, out RaycastHit hit, (point - lastPosition).magnitude, PourCollisionMask); //Add collision Mask?

            if (collided && hit.collider.gameObject != gameObject)
            {
                currentTrajectory[i] = hit.point;
                i++;
                pointCount = i;

                TryTransferLiquid(hit.collider.gameObject, time);

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
        //Debug.Log("PourLiquid: Pour");
        couroutine_Flowing = Couroutine_StartFlow(color);
        StartCoroutine(couroutine_Flowing);
    }

    public void Stop()
    {
        isPouring = false;
        StopCoroutine(couroutine_Flowing);

        if (liquid == null)
            return;
        //Debug.Log("LiquidPour: Stopping " + liquid.transform.name);

        liquid.StopFlow();
    }

    public void ReturnLiquid()
    {
        LiquidObjectPool.instance.ReturnLiquid(liquid);
        liquid = null;
        if (this == null) { return; }
        CancelInvoke();

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
        PourStrength = Mathf.Clamp((tilt - 1) * pourStrengthGain, 0.1f, maxPourStrength - Mathf.Clamp01(pourStrengthLimiter) * (maxPourStrength / 3));
    }

    public void UpdateLiquidLost(float lost)
    {
        liquidLost += lost;
    }

    void TryTransferLiquid(GameObject hitObject, float delay)
    {
        LiquidCatcher liquidCatcher = hitObject.GetComponent<LiquidCatcher>();

        if (liquidCatcher != null && liquidDispenser != null)
        {
            if (liquidCatcher.GetVolume() < 1)
            {
                StartCoroutine(liquidCatcher.Couroutine_AddFromDispenser(liquidDispenser.currentAttribute, liquidLost, delay));
                Invoke(nameof(StopParticle), delay);
                liquidLost = 0;
                return;
            }
        }
        else if (liquidCatcher != null)
        {
            if (liquidCatcher.GetVolume() < 1)
            {
                StartCoroutine(liquidCatcher.Couroutine_TransferLiquid(gameObject, liquidLost, delay));
                Invoke(nameof(StopParticle), delay);
                liquidLost = 0;
                return;
            }
        }
        else if (hitObject.TryGetComponent(out CanHaveAttributes canHaveAttributes))
        {
            StartCoroutine(Couroutine_TransferAttributes(canHaveAttributes, liquidLost, delay));
            Invoke(nameof(PlayParticle), delay);
            liquidLost = 0;
            return;
        }

        Invoke(nameof(PlayParticle), delay);
        BaseAttribute[] attributes = GetComponents<BaseAttribute>();

        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].LoseMass(liquidLost);
        }

        Debug.Log("Tried to tranfer to " + hitObject.name);

        liquidLost = 0;
    }

    IEnumerator Couroutine_TransferAttributes(CanHaveAttributes canHaveAttributes, float liquidLost, float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("Transferred Attributes to: " + canHaveAttributes.gameObject.name);
        canHaveAttributes.AddAttributes(gameObject, liquidLost);
    }

    void PlayParticle()
    {
        liquid.ps_waterSplash.Play();
    }

    void StopParticle()
    {
        liquid.ps_waterSplash.Stop();
    }

    private void OnDisable()
    {
        if (isPouring)
        {
            Debug.Log("PourLiquid Disabled: Stopping pour on " + transform.name);
            Stop();
        }
    }
}