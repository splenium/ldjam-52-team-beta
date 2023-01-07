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

    //[ReadOnly]
    [SerializeField]
    private Vector3 velocity;



    public void MoveDirection(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _moveDirection = new Vector2(0,0);
        velocity = body.velocity;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(body.velocity);
        Vector3 applyForce = new Vector3(_moveDirection.x * SpeedAccerelation * Time.deltaTime, body.velocity.y, _moveDirection.y * SpeedAccerelation * Time.deltaTime);
        body.AddRelativeForce(applyForce);
    }
}
