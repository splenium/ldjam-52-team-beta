using UnityEngine;

public class HandleSun : MonoBehaviour
{
    public Color SunColor;
    public Light LightSource;
    void Update()
    {
        this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_SunCol", SunColor);
        LightSource.color = SunColor;
    }
}
