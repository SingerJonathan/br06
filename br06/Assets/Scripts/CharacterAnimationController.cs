using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator _animator;
    private bool _running;

    // Speed variables
    public float WalkSpeed = 7.5f;
    public float RunSpeed = 17.5f;
    public float TurnSpeed = 200.0f;
    public float StrafeSpeed = 5.0f;

    // Key bindings
    public KeyCode WalkKey = KeyCode.W;
    public KeyCode WalkBackKey = KeyCode.S;
    public KeyCode TurnLeftKey = KeyCode.A;
    public KeyCode TurnRightKey = KeyCode.D;
    public KeyCode StrafeLeftKey = KeyCode.Q;
    public KeyCode StrafeRightKey = KeyCode.E;
    public KeyCode RunKey = KeyCode.LeftShift;

    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        /*if (Input.GetKey(runKey) && Input.GetAxis("Vertical") >= 0)
            running = true;
        else
            running = false;

        float translation;
        float rotation = Input.GetAxis("Horizontal") * turnSpeed;
        float strafeTranslation = 0;
        if (running)
            translation = Input.GetAxis("Vertical") * runSpeed;
        else
            translation = Input.GetAxis("Vertical") * walkSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);*/

        // Handle input and movement
        float translation = 0.0f;
        float rotation = 0.0f;
        float strafeTranslation = 0.0f;

        if (Input.GetKey(WalkKey))
        {
            if (Input.GetKey(RunKey))
                translation = RunSpeed * Time.deltaTime;
            else
                translation = WalkSpeed * Time.deltaTime;
            transform.Translate(0, 0, translation);
        }
        else if (Input.GetKey(WalkBackKey))
        {
            translation = -WalkSpeed * Time.deltaTime;
            transform.Translate(0, 0, translation);
        }

        if (Input.GetKey(TurnLeftKey))
        {
            rotation = -TurnSpeed * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }
        else if (Input.GetKey(TurnRightKey))
        {
            rotation = TurnSpeed * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }

        if (!Input.GetKey(RunKey))
            if (Input.GetKey(StrafeLeftKey))
            {
                strafeTranslation = -StrafeSpeed * Time.deltaTime;
                transform.Translate(strafeTranslation, 0, 0);
            }
            else if (Input.GetKey(StrafeRightKey)) {
                strafeTranslation = StrafeSpeed * Time.deltaTime;
                transform.Translate(strafeTranslation, 0, 0);
            }

        // Handle animations
        if (translation > 0)
        {
            _animator.SetInteger("walking", 1);
            if (Input.GetKey(RunKey))
                _animator.SetBool("running", true);
            else
                _animator.SetBool("running", false);
        }
        else if (translation < 0)
        {
            _animator.SetInteger("walking", 2);
            _animator.SetBool("running", false);
        }
        else
        {
            _animator.SetInteger("walking", 0);
            _animator.SetBool("running", false);
        }

        if (rotation > 0)
            _animator.SetInteger("turning", 1);
        else if (rotation < 0)
            _animator.SetInteger("turning", 2);
        else
            _animator.SetInteger("turning", 0);

        if (strafeTranslation > 0)
            _animator.SetInteger("strafing", 1);
        else if (strafeTranslation < 0)
            _animator.SetInteger("strafing", 2);
        else
            _animator.SetInteger("strafing", 0);
    }
}
