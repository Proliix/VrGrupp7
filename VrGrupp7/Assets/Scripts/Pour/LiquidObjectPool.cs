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

            newLiquid.name = i + " - " + liquid.name;

            newLiquid.SetActive(false);
            freeLiquids.Enqueue(newLiquid);
        }
    }

    private void Start()
    {
        //Time.timeScale = 0.1f;
    }

    public void ReturnLiquid(Liquid liquid)
    {
        freeLiquids.Enqueue(liquid.gameObject);
    }

    public Liquid GetLiquid()
    {
        if (freeLiquids.Count == 0)
        {
            return null;
        }

        //Debug.Log(freeLiquids.Count);
        return freeLiquids.Dequeue().GetComponent<Liquid>();
    }
}
