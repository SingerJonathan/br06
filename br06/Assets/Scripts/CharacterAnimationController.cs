using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [HideInInspector]
    public Animator _animator;
    private bool _running;
    private CharacterController _characterController;

    // Speed variables
    public int PlayerNumber = 1;
    //public float WalkSpeed = 7.5f;
    public float RunSpeed = 12f;
    //public float TurnSpeed = 200.0f;
    //public float StrafeSpeed = 5.0f;
    public float Gravity = 1000.0f;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Handle input and movement
        Vector3 moveDirection = Vector3.zero;
        if (_characterController.isGrounded)
        {
            moveDirection = new Vector3(-Input.GetAxis("Vertical"+PlayerNumber), 0, Input.GetAxis("Horizontal"+PlayerNumber));
            //if (Input.GetButton("Run"+PlayerNumber))
                moveDirection *= RunSpeed;
            //else
            //    moveDirection *= WalkSpeed;
            // DEVNOTE: Uncomment the following if jumping is added to the game
            /*if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;*/
            float step = RunSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, moveDirection, step, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);

        }
        moveDirection.y -= Gravity * Time.deltaTime;
        _characterController.Move(moveDirection * Time.deltaTime);
        
        // Handle animations
        // DEVNOTE: Rework Animator component due to movement system rework
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            //_animator.SetInteger("walking", 1);
            //if (Input.GetButton("Run" + PlayerNumber))
                _animator.SetBool("running", true);
            //else
            //    _animator.SetBool("running", false);
        }
        else
        {
            //_animator.SetInteger("walking", 0);
            _animator.SetBool("running", false);
        }
    }
}
