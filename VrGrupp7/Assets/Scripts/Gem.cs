using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private float attributeMass = 100;

    void Start()
    {
        if (AddRandomAttribute(out IAttribute attribute))
        {
            var baseAttribute = (BaseAttribute)attribute;
            baseAttribute.mass = attributeMass;

            if (TryGetComponent(out Torchable torchable))
            {
                var colors = PotionColors.GetColor(attribute);
                torchable.maxTorchedColor = colors.GetSideColor();
            }
        }
        else
        {
            Debug.LogWarning("No GameController was found, no attribute added to Gem: " + name);
        }
    }

    private bool AddRandomAttribute(out IAttribute attribute)
    {
        attribute = null;
        GameObject gameController = GameObject.FindWithTag("GameController");

        if(gameController == null) { return false; }

        IAttribute[] attributes = gameController.GetComponents<IAttribute>();

        if(attributes.Length == 0) { return false; }

        int random = Random.Range(0, attributes.Length);

        attribute = (IAttribute)gameObject.AddComponent(attributes[random].GetType());

        return true;
    }
}
