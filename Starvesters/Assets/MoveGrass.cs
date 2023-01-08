using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGrass : MonoBehaviour
{
    private float _offset;
    private void Start()
    {
        _offset = this.gameObject.transform.position.x;
    }
    void Update()
    {
        float amp = 4.3f;
        this.gameObject.transform.localRotation = Quaternion.Euler(
            Mathf.Sin(Time.realtimeSinceStartup + _offset)*amp,
            0.0f,
            Mathf.Sin(Time.realtimeSinceStartup * 1.5f + _offset)*amp);
    }
}
