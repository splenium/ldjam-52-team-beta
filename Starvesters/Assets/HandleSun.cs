using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSun : MonoBehaviour
{
    public Color SunColor;
    void Update()
    {
        this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_SunCol", SunColor);
    }
}
