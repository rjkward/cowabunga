using UnityEngine;

public class Rob_CharacterController : MonoBehaviour
{
    [SerializeField]
    private float _forVel = 10;
    [SerializeField]
    private float _rotaVel = 10f;
    
    [SerializeField]
    private float _maxBounceTime = 0.1f;
    [SerializeField]
    private float _bounceMultiplier = 5f;

    CharacterController _characterController;
    private float _turnInput;
    private float hitTime;
    public bool IsHit { get; private set; }
    private Vector3 _bounceDir;

    [SerializeField]
    private GameObject[] _skins;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void SetInput(float horizontal)
    {
        _turnInput = horizontal;
    }

    private void FixedUpdate()
    {
        Turn();
        MoveForward();
    }

    // Moves the cow forward
    void MoveForward()
    {
        if (IsHit == false)
        {
            _characterController.SimpleMove(Vector3.Lerp(_characterController.velocity, transform.forward * _forVel, 0.6f));
        
        }
        else
        {
            _characterController.SimpleMove(_bounceDir * _forVel * _bounceMultiplier);
            hitTime -= Time.fixedDeltaTime;
            if (hitTime <= 0f)
            {
                IsHit = false;
            }
        }
    }

    // Turns the cow
    void Turn()
    {
        transform.Rotate(Vector3.up, _rotaVel * _turnInput);
    }

    private const string CowTag = "Cow";

    // If you bumped into a cow then you both get knocked back
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag(CowTag))
        {
            if (IsHit)
            {
                return;
            }
            
            var otherCc = hit.gameObject.GetComponent<Rob_CharacterController>();
            if (otherCc.IsHit)
            {
                return;
            }
            
            Transform other = hit.transform;
            Vector3 toOther = (other.position - transform.position).normalized;
            
            if (Vector3.Angle(transform.forward, toOther) < 45f)
            {
                otherCc.Bounce(toOther);
            }

            if (Vector3.Angle(other.forward, -toOther) < 45f)
            {
                Bounce(-toOther);
            }
        }
    }


    // If cows have bumped into each other but one has boosted then the boosted cow is fine
    public void Bounce(Vector3 bounceDir)
    {
        _bounceDir = bounceDir;
        hitTime = _maxBounceTime;
        IsHit = true;
    }

    public void PickRandomSkin()
    {
        for (int i = 0; i < _skins.Length; i++)
        {
            _skins[i].SetActive(false);
        }

        int newSkin = Random.Range(0, _skins.Length);
        _skins[newSkin].SetActive(true);
    }
}