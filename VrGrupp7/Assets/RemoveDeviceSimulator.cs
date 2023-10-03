using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveDeviceSimulator : MonoBehaviour
{
    void Awake()
    {
        if (!Application.isEditor)
            Destroy(gameObject);
    }
}
