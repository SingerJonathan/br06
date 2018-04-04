using System.Collections;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public Animator _animator;
    private CharacterController _characterController;
    private Vector3 _moveDirection;
    private float _dodgeCountdown;
    protected string _enemyColour;

    //Bools for handling changes in movement.
    private bool _mirrorRun;
    private bool _twoHandedRun;
    private bool _knockedBack = false;
    private bool _jumpBack;
    private bool _pullTowards = false;
    private bool _knockedUp = false;

    // Speed variables
    public int PlayerNumber = 1;
    public float RunSpeed = 12f;
    public float DodgeSpeed = 35f;
    public float Gravity = 1000.0f;
    public float KnockbackForce = 1f;
    private float JumpBackForce = 1f;
    private float PullForce = 1f;
    private float KnockupDuration = 1f;
    private bool SlowedDown;
    private float SlowedSpeed;
    private float OldSpeed;
    private float SlowDuration = 1f;

    public float DodgeCountdown
    {
        get
        {
            return _dodgeCountdown;
        }

        set
        {
            _dodgeCountdown = value;
        }
    }

    public CharacterController CharacterController
    {
        get
        {
            return _characterController;
        }

        set
        {
            _characterController = value;
        }
    }

    public bool MirrorRun
    {
        get
        {
            return _mirrorRun;
        }

        set
        {
            _mirrorRun = value;
            _animator.SetBool("mirror", value);
            if (value)
                TwoHandedRun = false;
        }
    }

    public bool TwoHandedRun
    {
        get
        {
            return _twoHandedRun;
        }

        set
        {
            _twoHandedRun = value;
            _animator.SetBool("twohanded", value);
            if (value)
                MirrorRun = false;
        }
    }

    public bool PullTowardsValue
    {
        get
        {
            return _pullTowards;
        }
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _enemyColour = _animator.name.Contains("Red") ? "Blue" : "Red";
        OldSpeed = RunSpeed;
    }

    void Update()
    {
        // Handle input and movement
        if (_dodgeCountdown > 0.0f)
            _dodgeCountdown -= Time.deltaTime;
        else if (_dodgeCountdown <= 0.0f)
            _moveDirection = Vector3.zero;
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge") && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Ability"))
        {
            _moveDirection = new Vector3(-Input.GetAxis("Vertical" + PlayerNumber), 0, Input.GetAxis("Horizontal" + PlayerNumber));
            if (_moveDirection == Vector3.zero)
                _moveDirection = new Vector3(-Input.GetAxis("VerticalAlt" + PlayerNumber), 0, Input.GetAxis("HorizontalAlt" + PlayerNumber));
            if (_animator.GetNextAnimatorStateInfo(0).IsName("Dodge"))
            {
                _moveDirection = transform.forward.normalized * DodgeSpeed;
                _dodgeCountdown = GetComponent<CharacterStatsController>().DodgeCooldown;
            }
            else
                _moveDirection *= RunSpeed;
            float step = (RunSpeed * 2) * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, _moveDirection, step, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        _moveDirection.y -= Gravity;
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Ability"))
            _characterController.Move(_moveDirection * Time.deltaTime);

        // Handle knockback animation
        if(_knockedBack)
        {
            Vector3 enemyPosition = GameObject.FindGameObjectWithTag(_enemyColour).transform.position;
            Vector3 collisionDirection = enemyPosition - transform.position;
            collisionDirection = -collisionDirection.normalized;
            _characterController.Move(collisionDirection * KnockbackForce);
            StartCoroutine("knockbackDistance");
        }

        if (_jumpBack)
        {
            Vector3 moveDirection = -transform.forward.normalized;
            _characterController.Move(moveDirection * JumpBackForce);
            StartCoroutine("jumpBackDistance");
        }

        if(_pullTowards)
        {
            Vector3 enemyPosition = GameObject.FindGameObjectWithTag(_enemyColour).transform.position;
            Vector3 collisionDirection = enemyPosition - transform.position;
            collisionDirection = collisionDirection.normalized;
            _characterController.Move(collisionDirection * KnockbackForce);
            StartCoroutine("pullDistance");
        }

        if (SlowedDown)
            RunSpeed = SlowedSpeed;
        else
            RunSpeed = OldSpeed;
        
        if(_knockedUp)
        {
            Vector3 UpDirection = new Vector3(0,1,0);
            _characterController.Move(UpDirection * KnockbackForce);
            _moveDirection.y -= Gravity;
            StartCoroutine("KnockupDurationCoroutine");
        }

        // Handle animations
        if (Input.GetButton("Dodge" + PlayerNumber) && _dodgeCountdown <= 0.0f)
        {
            _animator.SetTrigger("dodge");
            GameObject.FindGameObjectWithTag("Dodge Sound").GetComponent<AudioSource>().Play();
        }
        if (_moveDirection.x != 0 || _moveDirection.z != 0)
        {
            _animator.SetBool("running", true);
        }
        else
        {
            _animator.SetBool("running", false);
        }
    }

    public void KnockedBack(float _knockbackForce)
    {
        KnockbackForce = _knockbackForce;
        _knockedBack = true;
    }

    public void JumpBack(float _jumpBackForce)
    {
        JumpBackForce = _jumpBackForce;
        _jumpBack = true;
    }

    public void PullTowards(float _pullForce)
    {
        PullForce = _pullForce;
        _pullTowards = true;
    }

    public void KnockupEnable(float _KnockupDuration)
    {
        KnockupDuration = _KnockupDuration;
        _knockedUp = true;
    }

    public void Slow(float _runSpeed, float _slowDuration)
    {
        SlowedDown = true;
        SlowedSpeed = _runSpeed;
        SlowDuration = _slowDuration;
        StartCoroutine(SlowTimer());
    }

    IEnumerator SlowTimer()
    {
        yield return new WaitForSeconds(SlowDuration);
        SlowedDown = false;
    }

    IEnumerator knockbackDistance()
    {
        yield return new WaitForSeconds(KnockbackForce);
        _knockedBack = false;
    }

    IEnumerator jumpBackDistance()
    {
        yield return new WaitForSeconds(JumpBackForce);
        _jumpBack = false;
    }

    IEnumerator pullDistance()
    {
        yield return new WaitForSeconds(PullForce);
        _pullTowards = false;
    }

    IEnumerator KnockupDurationCoroutine()
    {
        yield return new WaitForSeconds(KnockupDuration);
        _knockedUp = false;
    }
}
