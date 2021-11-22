using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbSpeed = 5f;

    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myCapsuleCollider;
    Vector2 moveInput;

    void Start() {
        myRigidbody = GetComponent<Rigidbody2D>(); // cache a ref to our own rigidbody.
        myAnimator = GetComponent<Animator>(); // cache a ref to our own animator.
        myCapsuleCollider = GetComponent<CapsuleCollider2D>(); // cache a ref to our collider.
    }

    void FixedUpdate() {
        MovePlayer();
        FlipSprite();
        ClimbLadder();
    }

    void OnMove(InputValue value) { // when move key is pressed {
        moveInput = value.Get<Vector2>(); // set the value of keypress into moveInput var.
    }

    void MovePlayer() {
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon; 
        // ^^ true/false if our velocity is greater than 0

        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed); 
        // ^^ Set the boolean of our animator to the t/f value of our boolean being greater than 0
    }

    void OnJump(InputValue value) {
        if (value.isPressed && myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void FlipSprite() {

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed) {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void ClimbLadder() {
        if (myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
            Vector2 climbingVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
            myRigidbody.velocity = climbingVelocity;
        }
        
    }
}
