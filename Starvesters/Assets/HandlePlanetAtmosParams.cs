using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlePlanetAtmosParams : MonoBehaviour
{
    public Material AtmosphereMaterial;

    public GameObject PlanetA;
    public float PlanetASize;
    public Color PlanetAColorA;
    public Color PlanetAColorB;

    public GameObject PlanetB;
    public float PlanetBSize;
    public Color PlanetBColorA;
    public Color PlanetBColorB;

    public GameObject PlanetC;
    public float PlanetCSize;
    public Color PlanetCColorA;
    public Color PlanetCColorB;


    // Update is called once per frame
    void Update()
    {
        AtmosphereMaterial.SetVector("_PlanetAPos", PlanetA.transform.position);
        AtmosphereMaterial.SetFloat("PlanetASize", PlanetASize);
        AtmosphereMaterial.SetColor("_PlanetAColA", PlanetAColorA);
        AtmosphereMaterial.SetColor("_PlanetAColB", PlanetAColorB);

        AtmosphereMaterial.SetVector("_PlanetBPos", PlanetA.transform.position);
        AtmosphereMaterial.SetFloat("PlanetBSize", PlanetASize);
        AtmosphereMaterial.SetColor("_PlanetBColA", PlanetAColorA);
        AtmosphereMaterial.SetColor("_PlanetBColB", PlanetAColorB);

        AtmosphereMaterial.SetVector("_PlanetCPos", PlanetC.transform.position);
        AtmosphereMaterial.SetFloat("PlanetCSize", PlanetCSize);
        AtmosphereMaterial.SetColor("_PlanetCColA", PlanetCColorA);
        AtmosphereMaterial.SetColor("_PlanetCColB", PlanetCColorB);
    }
}
