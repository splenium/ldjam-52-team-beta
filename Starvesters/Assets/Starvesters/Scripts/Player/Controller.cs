using UnityEngine;
using UnityEngine.Rendering;

public class Controller : MonoBehaviour
{
    public float AccelerationFactor;
    public float PlanetAccelerationFactor;
    public bool MouseLock;
    public float LookSpeedMouse;

    public float DirectionSmooth;
    public float PitchSpeed;
    public float Decay = 1f;
    public float MinimumSpeed = 1f;
    public float BoostFactor = 3f;
    public HandleAvatar Avatar;
    public bool ZeroIsEvil = true;
    public GameObject[] Planets;

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
        if(Input.GetKeyUp("escape"))
        {
            MouseLock = !MouseLock;
            if (!MouseLock)
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

        float boostFactor = 1f;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            boostFactor = BoostFactor;
        }

        var nearestPlanet = getNearestPlanet();
        float planetDistance = Vector3.Distance(nearestPlanet.transform.position, transform.position);
        float planetSize = nearestPlanet.GetComponent<Planet>().shapeSettings.planetSize*2.5f;
        float acceleration = Mathf.Lerp(AccelerationFactor, PlanetAccelerationFactor, 1.0f- Mathf.Clamp01(planetDistance / planetSize));

        var rigidBody = this.gameObject.GetComponent<Rigidbody>();
        var forwardAcceleration = this.gameObject.transform.forward * acceleration * boostFactor * Input.GetAxis("Vertical");
        Avatar.Acceleration = Input.GetAxis("Vertical") * Mathf.Clamp01(rigidBody.velocity.magnitude / 50.0f);

        Vector2 deltaMousePos;
        if (!MouseLock)
        {
            deltaMousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        } else
        {
            deltaMousePos = Vector2.zero;
        }

        float pitch = Input.GetAxis("Horizontal");

        var right = this.gameObject.transform.right * deltaMousePos.x * LookSpeedMouse;
        var up = this.gameObject.transform.up * deltaMousePos.y * LookSpeedMouse;
        var quat = Quaternion.LookRotation(this.gameObject.transform.forward + right + up, this.gameObject.transform.up + this.gameObject.transform.right*LookSpeedMouse * pitch* PitchSpeed);
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
