using UnityEngine;

public class Rob_CharacterController : MonoBehaviour
{
    [SerializeField]
    private float forVel = 10;
    [SerializeField]
    private float rotaVel = 10f;
    
    [SerializeField]
    private float _maxBounceTime = 0.15f;
    [SerializeField]
    private float _bounceMultiplier = 5f;

    CharacterController _characterController;
    public float turnInput;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Replace with keys with multiple players.
    private void GetInput()
    {
        turnInput = Input.GetAxis("Horizontal");
    }


    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        Turn();
        MoveForward();
        // Boost();
    }

    // Moves the cow forward
    void MoveForward()
    {
        if (isHit == false)
        {
            _characterController.SimpleMove(Vector3.Lerp(_characterController.velocity, transform.forward * forVel, 0.6f));
        
        }
        else
        {
            _characterController.SimpleMove(_bounceDir * forVel * _bounceMultiplier);
            hitTime -= Time.fixedDeltaTime;
            if (hitTime <= 0f)
            {
                isHit = false;
            }
        }
    }

    // Turns the cow
    void Turn()
    {
        transform.Rotate(Vector3.up, rotaVel * turnInput);
    }

    private const string CowTag = "Cow";

    // If you bumped into a cow then you both get knocked back
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag(CowTag))
        {
            Transform other = hit.transform;
            Vector3 toOther = (other.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, toOther) < 45f)
            {
                var otherCc = hit.gameObject.GetComponent<Rob_CharacterController>();
                otherCc.Bounce(toOther);
            }

            if (Vector3.Angle(other.forward, -toOther) < 45f)
            {
                Bounce(-toOther);
            }
            
            // Debug.Log("hit");
            // hit.gameObject.SendMessage("Bounce");
        }
    }

    private float hitTime;
    private bool isHit;
    private Vector3 _bounceDir;

    // If cows have bumped into each other but one has boosted then the boosted cow is fine
    public void Bounce(Vector3 bounceDir)
    {
        _bounceDir = bounceDir;
        hitTime = _maxBounceTime;
        isHit = true;
    }
}