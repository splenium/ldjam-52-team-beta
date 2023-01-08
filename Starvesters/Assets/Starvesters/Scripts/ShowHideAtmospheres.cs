using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ShowHideAtmospheres : MonoBehaviour
{
    public bool Visible;
    public Material AtmoshpereMat;
    public Material UIMat;
    void Update()
    {
        if (AtmoshpereMat != null)
            AtmoshpereMat.SetFloat("_Visibility", (Visible ? 1.0f : 0.0f));
        if (UIMat != null)
            UIMat.SetFloat("_Visibility", (Visible ? 1.0f : 0.0f));
    }
}
