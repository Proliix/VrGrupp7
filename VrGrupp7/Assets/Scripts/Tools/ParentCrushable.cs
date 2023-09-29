using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentCrushable : MonoBehaviour
{
    Dictionary<int, Crushable> dictionary = new Dictionary<int, Crushable>();

    // Start is called before the first frame update
    void Start()
    {
        Crushable[] crushables = GetComponentsInChildren<Crushable>();

        for (int i = 0; i < crushables.Length; i++)
        {
            int id = crushables[i].GetComponent<Collider>().GetInstanceID();

            dictionary.Add(id, crushables[i]);

            Debug.Log("Added " + crushables[i].name);
        }
    }

    public void CollidedWithCrusher(Collision myCollision, float damage, Vector3 crusherLocation)
    {
        Collider myCollider = myCollision.collider;

        Debug.Log("Collided with " + myCollider.name);

        int id = myCollider.GetInstanceID();

        Crushable crushable = dictionary[id];

        crushable.OnCollision(damage, myCollision.GetContact(0).point, crusherLocation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
