using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Mortar : MonoBehaviour
{
    [SerializeField] private GameObject dustPrefab;
    [SerializeField] private Transform dustSpawnpoint;
    [SerializeField] private GameObject heldObject;

    private GameObject currentDust;
    private XRSocketInteractor socket;

    private Vector3 heldObjectOriginalScale;
    private Vector3 dustOriginalScale;
    private float lerpScale = 0;

    private Pestle pestle;
    private Crushable crushable;

    void Start()
    {
        dustOriginalScale = dustPrefab.transform.localScale;
        socket = GetComponentInChildren<XRSocketInteractor>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Pestle pestle))
        {
            this.pestle = pestle;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Pestle>() == null) { return; }
        if(heldObject == null) { return; }
        if(crushable == null && !heldObject.TryGetComponent(out crushable)) { return; }

        float damage = pestle.GetDamage(dustSpawnpoint.position);
        float percentageLost = damage / crushable.startHealth;

        foreach (BaseAttribute attribute in heldObject.GetComponents<BaseAttribute>())
        {
            BaseAttribute dustAttribute = (BaseAttribute)currentDust.GetComponent(attribute.GetType());
            if(dustAttribute == null)
            {
                dustAttribute = attribute.AddToOther(currentDust.transform);
            }

            dustAttribute.mass += attribute.mass * percentageLost;
        }

        crushable.LoseHealth(damage);

        lerpScale += percentageLost;
        IncreaseDustSize();
    }

    public void SocketCheck()
    {
        IXRSelectInteractable objName = socket.GetOldestInteractableSelected();
        heldObject = objName.transform.gameObject;
        heldObject.transform.localScale = heldObjectOriginalScale;

        SpawnDust();
    }

    public void SocketClear()
    {
        if(crushable != null && crushable.currentHealth <= 0)
        {
            ReleaseDust();
        }

        heldObject = null;
        crushable = null;
        Debug.Log("Socket Cleared");
    }

    void SpawnDust()
    {
        if(currentDust != null) { return; }

        currentDust = Instantiate(dustPrefab, dustSpawnpoint);
        currentDust.transform.localScale = Vector3.zero;

        foreach (Collider col in currentDust.GetComponents<Collider>())
            col.enabled = false;

        currentDust.GetComponent<Rigidbody>().isKinematic = true;

        Color color = heldObject.GetComponent<Renderer>().material.color;
        currentDust.GetComponentInChildren<Renderer>().material.SetColor("_Color", color);

        lerpScale = 0;

        if  (heldObject.TryGetComponent(out Torchable heldTorchable) &&
            currentDust.TryGetComponent(out Torchable dustTorchable))
        {
            dustTorchable.maxTorchedColor = heldTorchable.maxTorchedColor;
        }
    }

    void IncreaseDustSize()
    {
        if (currentDust == null) { return; }

        float xScale = Mathf.Clamp(lerpScale, 0, dustOriginalScale.x);
        float zScale = Mathf.Clamp(lerpScale, 0, dustOriginalScale.z);

        float yScale = Mathf.Lerp(0, dustOriginalScale.y, lerpScale);
        Vector3 newScale = new Vector3(xScale, yScale, zScale);

        currentDust.transform.localScale = newScale;

        if(lerpScale >= 1)
        {
            ReleaseDust();
            SpawnDust();
        }
    }

    void ReleaseDust()
    {
        currentDust.transform.parent = null;
        currentDust.transform.position += (currentDust.transform.up * 0.15f);

        foreach (Collider col in currentDust.GetComponents<Collider>())
            col.enabled = true;

        currentDust.GetComponent<Rigidbody>().isKinematic = false;
        currentDust.GetComponent<AddGrab>().Add();
        currentDust = null;
    }
}
