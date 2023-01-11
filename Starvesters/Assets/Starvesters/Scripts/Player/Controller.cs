using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;
using static UnityEngine.UI.Image;

public class Controller : MonoBehaviour
{
    public float AccelerationFactor;
    public float PlanetAccelerationFactor;
    public float LookSpeedMouse;

    public float DirectionSmooth;
    public float PitchSpeed;
    public float Decay = 1f;
    public float MinimumSpeed = 1f;
    public float BoostFactor = 3f;
    public HandleAvatar Avatar;
    public bool ZeroIsEvil = true;
    public GameObject[] Planets;

    private KeyMap KeyboardLayout;
    private KeyMap qwerty;
    private KeyMap azerty;
    public bool isQwerty = true;
    private enum Axes { Horizontal, Vertical };
    class KeyMap
    {
        public KeyCode boost;
        public KeyCode forward;
        public KeyCode backward;
        public KeyCode pitchPositive;
        public KeyCode pitchNegative;

        public KeyMap(KeyCode boost, KeyCode forward, KeyCode backward, KeyCode pitchPositive, KeyCode pitchNegative)
        {
            this.boost = boost;
            this.forward = forward;
            this.backward = backward;
            this.pitchPositive = pitchPositive;
            this.pitchNegative = pitchNegative;
        }
    }

    void Start()
    {
        // Look the sun first
        transform.LookAt(Vector3.zero);

        qwerty = new KeyMap(KeyCode.LeftShift, KeyCode.W, KeyCode.S, KeyCode.D, KeyCode.A);
        azerty = new KeyMap(KeyCode.LeftShift, KeyCode.Z, KeyCode.S, KeyCode.Q, KeyCode.D);
        SetKeyboardLayout();
    }

    void SetKeyboardLayout()
    {
        KeyboardLayout = isQwerty ? qwerty : azerty;
    }

    float GetAxis(Axes axe)
    {
        switch (axe)
        {
            case Axes.Vertical:
                return (Input.GetKey(KeyboardLayout.forward) ? 1f : 0f) + (Input.GetKey(KeyboardLayout.backward) ? -1f : 0f);
            case Axes.Horizontal:
                return (Input.GetKey(KeyboardLayout.pitchPositive) ? 1f : 0f) + (Input.GetKey(KeyboardLayout.pitchNegative) ? -1f : 0f);
            default:
                throw new ArgumentException("Parameter value incorrect", nameof(axe));
        }
    }

    GameObject getNearestPlanet()
    {
        GameObject nearest = Planets[0];
        float distance = Vector3.Distance(nearest.transform.position, transform.position);
        foreach(var planet in Planets)
        {
            float curDistance = Vector3.Distance(planet.transform.position, transform.position);
            if(curDistance < distance)
            {
                distance= curDistance;
                nearest = planet;
            }
        }
        return nearest;
    }

    void Update()
    {
        if(Input.GetKeyUp("k"))
        {
            isQwerty = !isQwerty;
            SetKeyboardLayout();
        }

        float boostFactor = 1f;
        Avatar.ThrusterMatBoost.SetFloat("_Acceleration", 0.0f);
        Avatar.Boost = 0.0f;
        if (Input.GetKey(KeyboardLayout.boost))
        {
            boostFactor = BoostFactor;
            Avatar.ThrusterMatBoost.SetFloat("_Acceleration", 1.0f);
            Avatar.Boost = 1.0f;
        }
        
        var nearestPlanet = getNearestPlanet();
        float planetDistance = Vector3.Distance(nearestPlanet.transform.position, transform.position);
        float planetSize = nearestPlanet.GetComponent<Planet>().shapeSettings.planetSize*2.5f;
        float acceleration = Mathf.Lerp(AccelerationFactor, PlanetAccelerationFactor, 1.0f- Mathf.Clamp01(planetDistance / planetSize));

        var rigidBody = this.gameObject.GetComponent<Rigidbody>();
        var forwardAcceleration = this.gameObject.transform.forward * acceleration * boostFactor * GetAxis(Axes.Vertical);
        Avatar.Acceleration = GetAxis(Axes.Vertical) * Mathf.Clamp01(rigidBody.velocity.magnitude / 350.0f);

        Vector2 deltaMousePos;
        if (GameManager.Instance.MouseLock)
        {
            deltaMousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        } else
        {
            deltaMousePos = Vector2.zero;
        }

        float pitch = GetAxis(Axes.Horizontal);

        var right = this.gameObject.transform.right * deltaMousePos.x * LookSpeedMouse;
        var up = this.gameObject.transform.up * deltaMousePos.y * LookSpeedMouse;
        var quat = Quaternion.LookRotation(this.gameObject.transform.forward + right + up, this.gameObject.transform.up + this.gameObject.transform.right*LookSpeedMouse * pitch * PitchSpeed * Time.deltaTime);
        rigidBody.MoveRotation(quat);
        rigidBody.maxAngularVelocity = 0.0f;

        if(ZeroIsEvil) rigidBody.AddForce(this.gameObject.transform.position.normalized*(1.0f-Mathf.Clamp01(this.gameObject.transform.position.magnitude/300.0f))*100.0f);
        rigidBody.AddForce(forwardAcceleration);
        if(forwardAcceleration.magnitude < 0.1f && rigidBody.velocity.magnitude > MinimumSpeed)
        {
            float decayWay = Mathf.Clamp01(Vector3.Dot(rigidBody.velocity.normalized, this.gameObject.transform.forward));
            rigidBody.AddForce(-transform.forward * rigidBody.velocity.magnitude * Decay * decayWay);
        }
        rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, this.gameObject.transform.forward * rigidBody.velocity.magnitude, DirectionSmooth * Time.fixedDeltaTime);
    }
}
