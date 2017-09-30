using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour {

    static Animator animator;
    public float speed = 2.0f;
    public float rotationSpeed = 75.0f;
    
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	void Update () {
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        if (translation > 0)
            animator.SetInteger("walking", 1);
        else if (translation < 0)
            animator.SetInteger("walking", 2);
        else
            animator.SetInteger("walking", 0);
    }
}
