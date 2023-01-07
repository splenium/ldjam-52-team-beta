using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Controller : MonoBehaviour
{
    /// <summary>
    /// Rotation speed when using a controller.
    /// </summary>
    public float m_LookSpeedController = 120f;
    /// <summary>
    /// Rotation speed when using the mouse.
    /// </summary>
    public float m_LookSpeedMouse = 4.0f;
    /// <summary>
    /// Movement speed.
    /// </summary>
    public float m_MoveSpeed = 10.0f;
    /// <summary>
    /// Value added to the speed when incrementing.
    /// </summary>
    public float m_MoveSpeedIncrement = 2.5f;
    /// <summary>
    /// Scale factor of the turbo mode.
    /// </summary>
    public float m_Turbo = 10.0f;
    public bool mouseFreedom = false;


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
            inputRotateAxisX = Input.GetAxis(kMouseX) * m_LookSpeedMouse;
            inputRotateAxisY = Input.GetAxis(kMouseY) * m_LookSpeedMouse;
        }

        leftShift = Input.GetKey(KeyCode.LeftShift);
        fire1 = Input.GetAxis("Fire1") > 0.0f;

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
            m_MoveSpeed += inputChangeSpeed * m_MoveSpeedIncrement;
            if (m_MoveSpeed < m_MoveSpeedIncrement) m_MoveSpeed = m_MoveSpeedIncrement;
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

            transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, transform.localEulerAngles.z);

            // Speed adjust
            float moveSpeed = Time.deltaTime * m_MoveSpeed;
            if (fire1 || leftShiftBoost && leftShift)
                moveSpeed *= m_Turbo;
            transform.position += transform.forward * moveSpeed * inputVertical;
            transform.position += transform.right * moveSpeed * inputHorizontal;
        }
    }
}
