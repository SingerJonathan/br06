using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour {

    static Animator animator;
    public float walkSpeed = 10.0f;
    public float rotationSpeed = 200.0f;
    public float strafeSpeed = 7.5f;

    void Start () {
        animator = GetComponent<Animator>();
	}
	
	void Update () {
        //Handle input and movements
        float translation = Input.GetAxis("Vertical") * walkSpeed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        float strafeTranslation = 0;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        if (Input.GetKey(KeyCode.Q)) {
            strafeTranslation = -strafeSpeed * Time.deltaTime;
            transform.Translate(strafeTranslation, 0, 0);
        }
        else if (Input.GetKey(KeyCode.E)) {
            strafeTranslation = strafeSpeed * Time.deltaTime;
            transform.Translate(strafeTranslation, 0, 0);
        }

        //Handle animations
        if (translation > 0)
            animator.SetInteger("walking", 1);
        else if (translation < 0)
            animator.SetInteger("walking", 2);
        else
            animator.SetInteger("walking", 0);

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
