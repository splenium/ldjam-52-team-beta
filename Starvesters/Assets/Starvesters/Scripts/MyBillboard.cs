using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBillboard : MonoBehaviour
{

    void Update()
    {
        var dir = Camera.main.transform.position.normalized;
        var right = Vector3.Cross(dir, Vector3.up).normalized;
        var up = Vector3.Cross(right, dir).normalized;
        this.gameObject.transform.rotation = Quaternion.LookRotation(-dir, up);
    }
}
