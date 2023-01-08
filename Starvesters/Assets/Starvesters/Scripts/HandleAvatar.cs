using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleAvatar : MonoBehaviour
{
    public GameObject MainCube;
    public Vector3 RotateSpeed;
    [Range(0.0f, 1.0f)]
    public float Acceleration;
    public Material ThrusterMat;
    public AudioSource ThrusterAudio;


    public GameObject GemA;
    public GameObject GemB;
    public GameObject GemC;
    private float _accTime;
    void Update()
    {
        float factor = Mathf.Lerp(1.0f, 15.0f, Acceleration);
        MainCube.transform.Rotate(RotateSpeed * Time.deltaTime* factor);
        float dist = Mathf.Lerp(2.0f, 0.8f, Acceleration);
        var newPos = new Vector3(Mathf.Sin(_accTime), Mathf.Cos(_accTime), 0.0f);
        newPos = newPos * dist;
        GemA.transform.Rotate(-RotateSpeed * Time.deltaTime * 2.0f* factor, Space.Self);
        GemA.transform.localPosition = newPos;

        newPos = new Vector3(Mathf.Sin(_accTime+5.0f), Mathf.Cos(-_accTime), 0.0f);
        newPos = newPos * -dist;
        GemB.transform.Rotate(-RotateSpeed * Time.deltaTime * 2.0f * factor, Space.Self);
        GemB.transform.localPosition = newPos;

        newPos = new Vector3(Mathf.Sin(-_accTime + 5.0f), 0.0f, Mathf.Cos(-_accTime*0.8f+1.0f));
        newPos = newPos * -dist;
        GemC.transform.Rotate(Vector3.Scale(RotateSpeed, new Vector3(-1.0f, 1.20f, 0.8f)) * Time.deltaTime * 1.5f * factor, Space.Self);
        GemC.transform.localPosition = newPos;
        _accTime += Time.deltaTime * factor;

        ThrusterMat.SetFloat("_Acceleration", Acceleration);

        ThrusterAudio.volume = Mathf.Lerp(0.01f, 0.05f, Acceleration);
        ThrusterAudio.pitch = Mathf.Lerp(0.8f, 1.5f, Acceleration);
    }
}
