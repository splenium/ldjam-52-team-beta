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
            _applyMatToChildren(UnReady);
            Audio.Play();
            StartCoroutine(_makeItReady());
        }

    }

    IEnumerator _makeItReady()
    {
        yield return new WaitForSeconds(ReadyDelay);
        _applyMatToChildren(Ready);
        _isReady = true;
    }
    private void _applyMatToChildren(Material mat)
    {
        foreach(Transform t in ParentSeeds)
        {
            t.gameObject.GetComponent<MeshRenderer>().material = mat;
        }
    }
    private void Start()
    {
        _isReady = true;
        _applyMatToChildren(Ready);
        Audio.pitch += UnityEngine.Random.Range(-1.0f, 1.0f) * 0.2f;
    }
    void Update()
    {
        Ready.SetColor("_EmissionColor", Color);
    }
}
