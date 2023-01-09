using System.Collections;
using UnityEngine;


public class TreeCollectible : MonoBehaviour
{
    public float ReadyDelay = 30f;
    public Color Color;
    public float _timeGain;

    public Transform ParentSeeds;
    public AudioSource Audio;

    private bool _isReady;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" && _isReady)
        {
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
        _isReady = true;
        _applyColorToChildren(Color);
        Audio.pitch += Random.Range(-1.0f, 1.0f) * 0.2f;
    }
}
