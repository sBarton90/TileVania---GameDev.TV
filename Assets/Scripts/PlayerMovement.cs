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
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    Vector2 moveInput;
    float startingGravityScale;
    bool isAlive = true;

    void Start() {
        myRigidbody = GetComponent<Rigidbody2D>(); // cache a ref to our own rigidbody.
        myAnimator = GetComponent<Animator>(); // cache a ref to our own animator.
        myBodyCollider = GetComponent<CapsuleCollider2D>(); // cache a ref to our collider.
        myFeetCollider = GetComponent<BoxCollider2D>();
        startingGravityScale = myRigidbody.gravityScale;
    }

    void FixedUpdate() {
        if (!isAlive) {
            return;
        }
        MovePlayer();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value) { // when move key is pressed {
        if (!isAlive) {
            return;
        }
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
        if (!isAlive) {
            return;
        }

        if (value.isPressed && myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
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
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) { //if not touching the climbing layer
            myRigidbody.gravityScale = startingGravityScale; //gravity is normal, exit this method
            return;
        }
        // otherwise, touching is true, so run this code
        Vector2 climbVelocty = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed); //set a new vec2 for climbing speed
        myRigidbody.velocity = climbVelocty; // apply vec2 to rigidbody
        myRigidbody.gravityScale = 0f; //gravity is 0 so we don't fall off ladder

        //when climbing ladder, use climbing animation
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    void Die() {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies"))) {
            isAlive = false;
        } 
    }
}
