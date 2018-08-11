using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OK_CharacterController : MonoBehaviour
{

    // Variables
    [SerializeField]
    public enum PlayerSelect { one, two, three, four, five, six, seven, eight, nine, ten };
    public PlayerSelect Player;

    [SerializeField]
    private float moveDel = 0.2f;
    [SerializeField]
    private float forVel = 12;
    [SerializeField]
    private float rotaVel = 100;
    [SerializeField]
    private float BOOOST = 1000000;
    [SerializeField]
    private float bounce_time = 1;

    Quaternion tarRota;
    Rigidbody PC_Rigid;
    float turnInput;
    float time;

    Vector3 v3_Velocity;

    private bool isbounce = false;
    private bool isfalling = false;
    private bool isboost = false;
    private bool isbooston = false;

    public Quaternion TargetRotation
    {
        get { return tarRota; }
    }

    private void Start()
    {
        tarRota = transform.rotation;
        PC_Rigid = GetComponent<Rigidbody>();
        InvokeRepeating("GroundCheck", 1f, 0.2f);


        turnInput = 0;
        time = 3;
    }

    // Replace with keys with multiple players.
    private void GetInput()
    {
        turnInput = Input.GetAxis("Horizontal");
    }


    private void Update()
    {
        GetInput();

        v3_Velocity = PC_Rigid.velocity;

        if (isfalling == false)
        {
            Turn();
        }

        // Sets boost time
        if (isboost == true)
            time -= 1 * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        MoveForward();
        Boost();
    }

    // Moves the cow forward
    void MoveForward()
    {
        if (isfalling == false)
        {
            if (isbooston == false && isbounce == false)
            {
                PC_Rigid.velocity = transform.forward * forVel * Time.deltaTime;
            }
        }
        else
        {
            transform.rotation *= Quaternion.AngleAxis(rotaVel * Time.deltaTime * 5f, Vector3.up);
        }
    }

    // Turns the cow
    void Turn()
    {
        if (Mathf.Abs(turnInput) > moveDel)
        {
            if (isbooston == false)
                tarRota *= Quaternion.AngleAxis(rotaVel * turnInput * Time.deltaTime, Vector3.up);
        }
        transform.rotation = tarRota;
    }

    // Checks to see if the cow as fallen off the edge
    void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 20f))
        {

        }
        else
        {
            PC_Rigid.useGravity = true;
            isfalling = true;
        }

        if (isfalling == true)
            transform.localScale += new Vector3(-0.1F, -0.1f, -0.1f);
    }

    // Boost control
    void Boost()
    {
        if (isboost == false && Input.GetKey("space"))
        {
            PC_Rigid.AddForce(transform.forward * BOOOST * 100);
            isboost = true;
            isbooston = true;
        }

        if (time <= 0)
        {
            time = 3;
            isboost = false;
        }

        if (time <= 2.5f)
        {
            isbooston = false;
        }
    }

    // If you bumped into a cow then you both get knocked back
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Cow")
        {
            hit.gameObject.SendMessage("Bounce");
            hit.rigidbody.AddForce(transform.forward * v3_Velocity.x);
        }
    }

    // If cows have bumped into each other but one has boosted then the boosted cow is fine
    void Bounce()
    {
        if (isbooston == false)
            isbounce = true;
    }
}