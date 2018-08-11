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

    public Quaternion TargetRotation
    {
        get { return tarRota; }
    }

    private void Start()
    {
        tarRota = transform.rotation;
        PC_Rigid = GetComponent<Rigidbody>();


        turnInput = 0;
    }

    private void GetInput()
    {

        turnInput = Input.GetAxis("Horizontal");
    }

    private void Update()
    {
        GetInput();
        Turn();
    }

    private void FixedUpdate()
    {
        MoveForward();
    }

    void MoveForward()
    {
        if(isbounce == true)
        {
            PC_Rigid.velocity = Vector3.zero;
        }
        else
        {
            PC_Rigid.velocity = transform.forward * forVel * Time.deltaTime;

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
}
