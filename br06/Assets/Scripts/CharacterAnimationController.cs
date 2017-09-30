using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour {

    private Animator animator;
    private bool running;

    // Speed variables
    public float walkSpeed = 7.5f;
    public float runSpeed = 17.5f;
    public float turnSpeed = 200.0f;
    public float strafeSpeed = 5.0f;

    // Key bindings
    public KeyCode walkKey = KeyCode.W;
    public KeyCode walkBackKey = KeyCode.S;
    public KeyCode turnLeftKey = KeyCode.A;
    public KeyCode turnRightKey = KeyCode.D;
    public KeyCode strafeLeftKey = KeyCode.Q;
    public KeyCode strafeRightKey = KeyCode.E;
    public KeyCode runKey = KeyCode.LeftShift;

    void Start () {
        animator = gameObject.GetComponent<Animator>();
    }

    void Update() {
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

        if (Input.GetKey(walkKey))
        {
            if (Input.GetKey(runKey))
                translation = runSpeed * Time.deltaTime;
            else
                translation = walkSpeed * Time.deltaTime;
            transform.Translate(0, 0, translation);
        }
        else if (Input.GetKey(walkBackKey)) {
            translation = -walkSpeed * Time.deltaTime;
            transform.Translate(0, 0, translation);
        }

        if (Input.GetKey(turnLeftKey)) {
            rotation = -turnSpeed * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }
        else if (Input.GetKey(turnRightKey)) {
            rotation = turnSpeed * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }

        if (!Input.GetKey(runKey))
            if (Input.GetKey(strafeLeftKey)) {
                strafeTranslation = -strafeSpeed * Time.deltaTime;
                transform.Translate(strafeTranslation, 0, 0);
            }
            else if (Input.GetKey(strafeRightKey)) {
                strafeTranslation = strafeSpeed * Time.deltaTime;
                transform.Translate(strafeTranslation, 0, 0);
            }

        // Handle animations
        if (translation > 0) {
            animator.SetInteger("walking", 1);
            if (Input.GetKey(runKey))
                animator.SetBool("running", true);
            else
                animator.SetBool("running", false);
        }
        else if (translation < 0) {
            animator.SetInteger("walking", 2);
            animator.SetBool("running", false);
        }
        else {
            animator.SetInteger("walking", 0);
            animator.SetBool("running", false);
        }

        if (rotation > 0)
            animator.SetInteger("turning", 1);
        else if (rotation < 0)
            animator.SetInteger("turning", 2);
        else
            animator.SetInteger("turning", 0);

        if (strafeTranslation > 0)
            animator.SetInteger("strafing", 1);
        else if (strafeTranslation < 0)
            animator.SetInteger("strafing", 2);
        else
            animator.SetInteger("strafing", 0);
    }
}
