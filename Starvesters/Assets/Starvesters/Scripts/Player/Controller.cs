using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Controller : MonoBehaviour
{
    public float AccelerationFactor;
    public bool MouseLock;
    public float LookSpeedMouse;

    //private void Update()
    //{
    //    if (!MouseLock)
    //    {
    //        //Cursor.visible = true;
    //        Cursor.lockState = CursorLockMode.None;
    //    }
    //    else
    //    {
    //        //Cursor.visible = false;
    //        Cursor.lockState = CursorLockMode.Locked;
    //    }
    //    var curAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    //    _deltaAxis = _lastAxis - curAxis;
    //    _lastAxis = curAxis;
    //}
    private Vector2 _deltaAxis;
    private Vector2 _lastAxis;
    private Vector2 _lastMousePosition;
    public float DirectionSmooth;
    public float PitchSpeed;
    public HandleAvatar Avatar;
    void Update()
    {
        var rigidBody = this.gameObject.GetComponent<Rigidbody>();
        var forwardAcceleration = this.gameObject.transform.forward * AccelerationFactor*Input.GetAxis("Vertical");
        Avatar.Acceleration = Input.GetAxis("Vertical") * Mathf.Clamp01(rigidBody.velocity.magnitude / 50.0f);

        float rotationX = transform.localEulerAngles.x;
        var curMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        var deltaMousePos = _lastMousePosition - curMousePos;
        deltaMousePos =  new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        float newRotationY = transform.localEulerAngles.y + deltaMousePos.x* LookSpeedMouse;

        float newRotationX = (rotationX - deltaMousePos.y* LookSpeedMouse);
        if (rotationX <= 90.0f && newRotationX >= 0.0f)
            newRotationX = Mathf.Clamp(newRotationX, 0.0f, 90.0f);
        if (rotationX >= 270.0f)
            newRotationX = Mathf.Clamp(newRotationX, 270.0f, 360.0f);

        float pitch = (Input.GetKey(KeyCode.A) ? 1.0f : 0.0f) + (Input.GetKey(KeyCode.E) ? -1.0f : 0.0f);
        //transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, transform.localEulerAngles.z);
        var newQuat = Quaternion.Euler(newRotationX, newRotationY, transform.localEulerAngles.z+ pitch*4.9f);

        Debug.Log(" " + deltaMousePos.x);

        var right = this.gameObject.transform.right * deltaMousePos.x * LookSpeedMouse;
        var up = this.gameObject.transform.up * deltaMousePos.y * LookSpeedMouse;
        var quat = Quaternion.LookRotation(this.gameObject.transform.forward + right + up, this.gameObject.transform.up + this.gameObject.transform.right*LookSpeedMouse * pitch* PitchSpeed);
        rigidBody.MoveRotation(quat);// Quaternion.Slerp(rigidBody.rotation, quat, DirectionSmooth * Time.fixedDeltaTime));
        rigidBody.maxAngularVelocity = 0.0f;
        //rigidBody.MoveRotation(quat);


        rigidBody.AddForce(forwardAcceleration);
        rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, this.gameObject.transform.forward * rigidBody.velocity.magnitude, DirectionSmooth * Time.fixedDeltaTime);
        _lastMousePosition = curMousePos;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(this.gameObject.transform.position, this.gameObject.transform.position + this.gameObject.transform.forward*5.0f);
    }
}
