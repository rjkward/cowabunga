using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OK_CharacterController : MonoBehaviour {

    // Variables
    [SerializeField]
    public enum PlayerSelect {one,two,three,four,five,six,seven,eight,nine,ten };
    public PlayerSelect Player;

    [SerializeField]
    private float moveDel = 0.2f;
    [SerializeField]
    private float forVel = 12;
    [SerializeField]
    private float rotaVel = 100;

    Quaternion tarRota;
    Rigidbody PC_Rigid;
    float turnInput;

    private bool isbounce = false;
    private bool isfalling = false;

    public Quaternion TargetRotation
    {
        get { return tarRota; }
    }

    private void Start()
    {
        tarRota = transform.rotation;
        PC_Rigid = GetComponent<Rigidbody>();
        InvokeRepeating("GroundCheck", 1f, 1f);


        turnInput = 0;
    }

    private void GetInput()
    {

        turnInput = Input.GetAxis("Horizontal");
    }

    private void Update()
    {
        GetInput();
        if (isfalling == false)
        {
            Turn();
        }

    }

    private void FixedUpdate()
    {
        MoveForward();
    }

    void MoveForward()
    {
        if (isfalling == false)
        {
            if (isbounce == true)
            {
                PC_Rigid.velocity = Vector3.zero;
            }
            else
            {
                PC_Rigid.velocity = transform.forward * forVel * Time.deltaTime;

            }
        }
        else
        {
            transform.rotation = Quaternion.AngleAxis(rotaVel * Time.deltaTime * 100f,Vector3.up);

        }

    }

    void Turn()
    {
        if (Mathf.Abs(turnInput) > moveDel)
        {
            tarRota *= Quaternion.AngleAxis(rotaVel * turnInput * Time.deltaTime, Vector3.up);
        }
        transform.rotation = tarRota;
    }

    void PlayerControls()
    {

    }

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
}

