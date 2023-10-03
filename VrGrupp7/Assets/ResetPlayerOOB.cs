using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerOOB : MonoBehaviour
{
    [SerializeField] float minY = -5;
    [SerializeField] Vector3 respawnPoint;
    bool isWaiting;

    IEnumerator respawn()
    {
        isWaiting = true;
        GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSeconds(1f);
        transform.position = respawnPoint;
        GetComponent<CharacterController>().enabled = true;
        yield return new WaitForSeconds(1f);
        isWaiting = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < minY && !isWaiting)
            StartCoroutine(respawn());
    }
}
