using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(0)]
public class RemoveDeviceSimulator : MonoBehaviour
{
    void Awake()
    {
        if (!Application.isEditor)
            Destroy(gameObject);
    }
}
