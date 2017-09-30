using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour {

    static Animator animator;
    public float walkSpeed = 7.5f;
    public float runSpeed = 17.5f;
    public float rotationSpeed = 200.0f;
    public float strafeSpeed = 5.0f;
    private bool running;

    void Start () {
        animator = GetComponent<Animator>();
    }

    void Update() {
        // Handle input and movement
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") >= 0)
            running = true;
        else
            running = false;

        float translation;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        float strafeTranslation = 0;
        if (running)
            translation = Input.GetAxis("Vertical") * runSpeed;
        else
            translation = Input.GetAxis("Vertical") * walkSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        if (!Input.GetKey(KeyCode.LeftShift))
            if (Input.GetKey(KeyCode.Q)) {
                strafeTranslation = -strafeSpeed * Time.deltaTime;
                transform.Translate(strafeTranslation, 0, 0);
            }
            else if (Input.GetKey(KeyCode.E)) {
                strafeTranslation = strafeSpeed * Time.deltaTime;
                transform.Translate(strafeTranslation, 0, 0);
            }

        // Handle animations
        if (translation > 0) {
            animator.SetInteger("walking", 1);
            if (running)
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
