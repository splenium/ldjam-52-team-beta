using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Controller : MonoBehaviour
{
    /// <summary>
    /// Rotation speed when using a controller.
    /// </summary>
    public float LookSpeedControl = 120f;
    /// <summary>
    /// Rotation speed when using the mouse.
    /// </summary>
    public float LookSpeedMouse = 4.0f;
    /// <summary>
    /// Movement speed.
    /// </summary>
    public float MoveSpeed = 20000;
    /// <summary>
    /// Value added to the speed when incrementing.
    /// </summary>
    public float MoveSpeedIncrement = 2.5f;
    /// <summary>
    /// Scale factor of the turbo mode.
    /// </summary>
    public float m_Turbo = 10.0f;
    public bool mouseFreedom = false;
    public Rigidbody Body;
    public float DecaySpeed = 0.03f;
    public float minimumSpeed = 5.07f;


    private static string kMouseX = "Mouse X";
    private static string kMouseY = "Mouse Y";

    private static string kVertical = "Vertical";
    private static string kHorizontal = "Horizontal";

    float inputRotateAxisX, inputRotateAxisY;
    float inputChangeSpeed;
    float inputVertical, inputHorizontal; 
    bool leftShiftBoost, leftShift, fire1;

    void Start()
    {
        Cursor.visible = false;
    }

    void MouseManager()
    {
        if(mouseFreedom)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void UpdateInputs()
    {
        inputRotateAxisX = 0.0f;
        inputRotateAxisY = 0.0f;
        leftShiftBoost = false;
        fire1 = false;

        if(Input.GetButtonUp("Escape"))
        {
            mouseFreedom = !mouseFreedom;
        }

        if (!mouseFreedom)
        {
            leftShiftBoost = true;
            inputRotateAxisX = Input.GetAxis(kMouseX) * LookSpeedMouse;
            inputRotateAxisY = Input.GetAxis(kMouseY) * LookSpeedMouse;
        }

        leftShift = Input.GetKey(KeyCode.LeftShift);
        //fire1 = Input.GetAxis("Fire1") > 0.0f;

        inputVertical = Input.GetAxis(kVertical);
        inputHorizontal = Input.GetAxis(kHorizontal);
    }

    void Update()
    {
        // If the debug menu is running, we don't want to conflict with its inputs.
        if (DebugManager.instance.displayRuntimeUI)
            return;

        UpdateInputs();
        MouseManager();

        if (inputChangeSpeed != 0.0f)
        {
            MoveSpeed += inputChangeSpeed * MoveSpeedIncrement;
            if (MoveSpeed < MoveSpeedIncrement) MoveSpeed = MoveSpeedIncrement;
        }

        bool moved = inputRotateAxisX != 0.0f || inputRotateAxisY != 0.0f || inputVertical != 0.0f || inputHorizontal != 0.0f;
        if (moved)
        {
            float rotationX = transform.localEulerAngles.x;
            float newRotationY = transform.localEulerAngles.y + inputRotateAxisX;

            float newRotationX = (rotationX - inputRotateAxisY);
            if (rotationX <= 90.0f && newRotationX >= 0.0f)
                newRotationX = Mathf.Clamp(newRotationX, 0.0f, 90.0f);
            if (rotationX >= 270.0f)
                newRotationX = Mathf.Clamp(newRotationX, 270.0f, 360.0f);

            //transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, transform.localEulerAngles.z);
            Body.MoveRotation(Quaternion.Euler(newRotationX, newRotationY, transform.localEulerAngles.z));

            // Speed adjust
            float moveSpeed = Time.deltaTime * MoveSpeed;
            if (leftShiftBoost && leftShift)
                moveSpeed *= m_Turbo;
            //transform.position += transform.forward * moveSpeed * inputVertical;
            //transform.position += transform.right * moveSpeed * inputHorizontal;

            Vector3 applyForce = new Vector3(moveSpeed * inputHorizontal * Time.deltaTime, 0, moveSpeed * inputVertical * Time.deltaTime);


            // Apply forces
            if(applyForce.magnitude < 0.1)
            {
                //Body.vety = Vector3.Lerp(Body.velocity, Body.velocity.normalized * minimumSpeed, Time.deltaTime / DecaySpeed);
                Body.AddForce(-this.gameObject.transform.forward * DecaySpeed);
            }
            Body.AddRelativeForce(applyForce);
        }
    }
}
