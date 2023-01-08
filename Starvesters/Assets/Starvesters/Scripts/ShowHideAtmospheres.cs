using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideAtmospheres : MonoBehaviour
{
    public bool Visible;
    public Material AtmoshpereMat;
    void Start()
    {
        AtmoshpereMat.SetFloat("_Visibility", (Visible ? 1.0f : 0.0f));
    }
}
