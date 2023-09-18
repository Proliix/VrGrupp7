using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidObjectPool : MonoBehaviour
{

    public static LiquidObjectPool instance;

    [SerializeField] private int numOfLiquids = 3;
    [SerializeField] private GameObject liquid;

    private Queue<GameObject> freeLiquids = new Queue<GameObject>();
    private void Awake()
    {
        instance = this;

        for (int i = 0; i < numOfLiquids; i++)
        {
            GameObject newLiquid = Instantiate(liquid, transform);

            freeLiquids.Enqueue(newLiquid);
        }
    }

    private void Start()
    {

    }

    public void ReturnLiquid(Liquid liquid)
    {
        freeLiquids.Enqueue(liquid.gameObject);
    }

    public Liquid GetLiquid()
    {
        Debug.Log(freeLiquids.Count);
        return freeLiquids.Dequeue().GetComponent<Liquid>();
    }
}
