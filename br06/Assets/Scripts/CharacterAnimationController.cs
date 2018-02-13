using System;
using System.Collections;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public Animator _animator;
    private CharacterController _characterController;
    private Vector3 _moveDirection;
    private float _dodgeCountdown;
    private bool _mirrorRun;
    private bool _knockedBack = false;
    protected string _enemyColour;


    // Speed variables
    public int PlayerNumber = 1;
    public float RunSpeed = 12f;
    public float DodgeSpeed = 35f;
    public float Gravity = 1000.0f;
    public float knockbackForce = 1f;

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
        }
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _enemyColour = _animator.name.Contains("Red") ? "Blue" : "Red";
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
            float step = RunSpeed * Time.deltaTime;
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
            _characterController.Move(collisionDirection * knockbackForce);
            StartCoroutine("knockbackDistance");
        }
        
        // Handle animations
        if (Input.GetButton("Dodge" + PlayerNumber) && _dodgeCountdown <= 0.0f)
        {
            _animator.SetTrigger("dodge");
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

    public void KnockedBack()
    {
        _knockedBack = true;
    }

    IEnumerator knockbackDistance()
    {
        yield return new WaitForSeconds(knockbackForce);
        _knockedBack = false;
    }
}
