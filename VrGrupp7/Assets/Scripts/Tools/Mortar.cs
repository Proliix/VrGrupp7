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

    // Start is called before the first frame update
    void Start()
    {
        dustOriginalScale = dustPrefab.transform.localScale;
        socket = GetComponentInChildren<XRSocketInteractor>();

        SpawnDust();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Pestle pestle))
            this.pestle = pestle;

        if(heldObject == null) { return; }

        if      (heldObject.TryGetComponent(out Crushable crushable) && 
            other.transform.TryGetComponent(out Crusher crusher))
        {
            this.crushable = crushable;
            float damage = crusher.GetDamage();
            Vector3 crusherLocation = crusher.transform.position;
            Vector3 hitLocation = dustSpawnpoint.transform.position;

            crushable.OnCollision(damage, hitLocation, crusherLocation);


        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Pestle>() == null) { return; }

        float damage = pestle.GetDamage(dustSpawnpoint.position);

        //Debug.Log(damage);

        lerpScale += damage / crushable.startHealth;
        IncreaseDustSize();
        DecreaseCrushableSize();
    }

    public void SocketCheck()
    {

        IXRSelectInteractable objName = socket.GetOldestInteractableSelected();

        //Debug.Log(objName.transform.name + " in socket of " + transform.name);

        heldObject = objName.transform.gameObject;

        heldObject.transform.localScale = heldObjectOriginalScale;
    }

    public void SocketClear()
    {
        heldObject = null;
    }

    void SpawnDust()
    {
        if(currentDust != null) { return; }

        currentDust = Instantiate(dustPrefab, dustSpawnpoint);
        currentDust.transform.localScale = Vector3.zero;

        currentDust.GetComponent<Collider>().enabled = false;
        currentDust.GetComponent<Rigidbody>().isKinematic = true;

        lerpScale = 0;
    }

    void IncreaseDustSize()
    {
        currentDust.transform.localScale = Vector3.Lerp(Vector3.zero, dustOriginalScale, lerpScale);

        if(lerpScale >= 1)
        {
            ReleaseDust();
            SpawnDust();
        }
    }

    void DecreaseCrushableSize()
    {
        if(heldObject == null) { return; }

        heldObject.transform.localScale = Vector3.Lerp(Vector3.zero, heldObjectOriginalScale, 1 - (lerpScale/2));
    }

    void ReleaseDust()
    {
        currentDust.transform.parent = null;
        currentDust.transform.position += (currentDust.transform.up * 0.15f);

        currentDust.GetComponent<Collider>().enabled = true;
        currentDust.GetComponent<Rigidbody>().isKinematic = false;
        currentDust.GetComponent<AddGrab>().Add();

        currentDust = null;
    }
}