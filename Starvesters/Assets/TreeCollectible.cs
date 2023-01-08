using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TreeCollectible : MonoBehaviour
{
    public static float ReadyDelay = 10.0f;
    public Material Ready;
    public Material UnReady;
    public Color Color;
    public Transform ParentSeeds;
    public AudioSource Audio;

    private bool _isReady;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" && _isReady)
        {
            // TODO call collect beuhbeuh
            GameManager.Instance.LightIsHarvest(this.gameObject);

            // TODO particle + sound
            _isReady = false;
            _applyColorToChildren(Color.black);
            Audio.Play();
            StartCoroutine(_makeItReady());
        }

    }

    IEnumerator _makeItReady()
    {
        yield return new WaitForSeconds(ReadyDelay);
        _applyColorToChildren(Color);
        _isReady = true;
    }
    private void _applyColorToChildren(Color col)
    {
        foreach(Transform t in ParentSeeds)
        {
            Material toEdit = t.gameObject.GetComponent<MeshRenderer>().material;
            if(Color.Equals(Color.black))
            {
                toEdit.DisableKeyword("_EMISSION");
            } 
            else
            {
                toEdit.EnableKeyword("_EMISSION");
            }
            toEdit.SetColor("_EmissionColor", col);
        }
    }

    private void Start()
    {
        //Ready.EnableKeyword("_EMISSION");
        //Ready.SetColor("_EmissionColor", Color);
        _isReady = true;
        _applyColorToChildren(Color);
        Audio.pitch += UnityEngine.Random.Range(-1.0f, 1.0f) * 0.2f;
    }
    //void Update()
    //{
    //    Ready.SetColor("_EmissionColor", Color);
    //}
}
