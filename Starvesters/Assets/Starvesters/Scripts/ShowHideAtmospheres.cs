using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ShowHideAtmospheres : MonoBehaviour
{
    public bool Visible;
    public Material AtmoshpereMat;
    void Update()
    {
        if (AtmoshpereMat != null)
        AtmoshpereMat.SetFloat("_Visibility", (Visible ? 1.0f : 0.0f));
    }
}
