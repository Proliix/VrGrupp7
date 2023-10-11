using UnityEngine;
using UnityEngine.Events;

public class Torchable : MonoBehaviour
{
    public UnityEvent onBreak;
    private bool isBroken = false;

    [Header("Defaults to this gameobject")]
    [SerializeField] public GameObject torchedObject;

    [Header("Heat Controls")]
    [SerializeField] private float heatGainSpeed = 0.5f;
    [SerializeField] private float heatLossSpeed = 0.2f;
    [SerializeField] private float maxTemp = 1.5f;
    [SerializeField] private float breakTemp = 1.5f;

    [Header("Color Change Controls (Default color is red)")]
    [SerializeField] public Color maxTorchedColor;
    [SerializeField] private float saturationModifier = 0.2f;
    [SerializeField] private float valueModifier = 1;

    private Material material;
    private Color original;

    private float temp01 = 0;
    private bool insideFire = false;

    // Start is called before the first frame update
    void Start()
    {
        if (torchedObject == null)
            torchedObject = gameObject;

        if (maxTorchedColor == Color.clear)
            maxTorchedColor = Color.red;

        material = torchedObject.GetComponent<Renderer>().material;
        original = material.color;

        //Debug.Log(gameObject.name +  " has Mat: " + material.name);
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        temp01 = insideFire ?
            temp01 + Time.deltaTime * heatGainSpeed :
            temp01 - Time.deltaTime * heatLossSpeed;

        temp01 = Mathf.Clamp(temp01, 0, maxTemp);

        if (temp01 >= breakTemp && !isBroken)
        {
            isBroken = true;
            onBreak.Invoke();
        }

        if (temp01 <= 0)
        {
            temp01 = 0;
            material.color = original;
            this.enabled = false;
        }

        Color.RGBToHSV(original, out float hue, out float saturation, out float value);
        Color.RGBToHSV(maxTorchedColor, out float maxH, out float MaxS, out float MaxV);

        hue = Mathf.Lerp(hue, maxH, temp01);
        saturation = Mathf.Lerp(saturation, MaxS, temp01);
        value = Mathf.Lerp(value, MaxV, temp01);
        float alpha = Mathf.Lerp(original.a, (original.a / 4f), temp01);

        Color newColor = Color.HSVToRGB(hue, saturation - (temp01 * saturationModifier), value * (1 + temp01) * valueModifier);
        newColor.a = alpha;

        material.color = newColor;
        //Debug.Log(material.name + " has temp: " + temp01 + " and color " + material.color + ". Target color: " + newColor);
    }

    public void OnTorchEnter()
    {
        this.enabled = true;
        insideFire = true;
    }

    public void OnTorchExit()
    {
        insideFire = false;
    }
    public float GetTemperature()
    {
        return temp01;
    }
}
