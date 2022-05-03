using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerenceAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private MyCharacterController char_cont;
    private float maxSpeed = 7.0F;




    // Start is called before the first frame update
    void Start()
    {
        char_cont = this.GetComponent<MyCharacterController>();
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!char_cont.onSpline) 
        {
            animator.SetFloat("speed", rb.velocity.magnitude / maxSpeed);
        }
    }
}
