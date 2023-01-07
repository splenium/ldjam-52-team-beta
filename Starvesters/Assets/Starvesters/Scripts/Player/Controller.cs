using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    private Vector2 _moveDirection;
    public Rigidbody body;
    public float SpeedAccerelation = 1;

    public Vector3 SpeedDecay = new Vector3(.1f, .1f, .1f);

    public void MoveDirection(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _moveDirection = new Vector2(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(body.velocity);

        Vector3 applyForce = new Vector3(_moveDirection.x * SpeedAccerelation * Time.deltaTime, 0, _moveDirection.y * SpeedAccerelation * Time.deltaTime);
        applyForce = applyForce + Vector3.Scale(body.velocity * -1, SpeedDecay);
        body.AddRelativeForce(applyForce);
    }
}
