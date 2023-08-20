using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    // Adjust the players movement speeds
    [Header("Movement")]
    [SerializeField] float runVelocity = 5f;
    [SerializeField] float jumpVelocity = 5f;
    [SerializeField] float freezeOnAwake = 0.3f;
    private Vector2 moveInput;
    private bool playerHasUpwardMovement;
    private bool frozen = true;

    // Contains the individual colliders to be used for the player
    [Header("Collider Components")]
    [SerializeField] BoxCollider2D playerBody;
    [SerializeField] BoxCollider2D playerFeet;
    [SerializeField] BoxCollider2D playerFront;

    // Options for dashing
    [Header("Dashing")]
    [SerializeField] float dashVelocity = 40f;
    [SerializeField] float dashTime = 0.1f;
    [SerializeField] float dashCooldown = 0.5f;
    private Vector2 dashDirection;
    private bool isDashing;
    private bool canDash = true;

    // Adjust wall slide and the jump
    [Header("Wall Jumping")]
    [SerializeField] float wallSlideSpeed = 1f;
    [SerializeField] float wallJumpVelocity = 5f;
    [SerializeField] float wallJumpTime = 0.2f;
    private bool isWallSliding;
    private bool isWallJumping;
    private float flipX;


    // referenced components
    private Rigidbody2D rigidbodyPlayer;
    private Animator animatorPlayer;
    private TrailRenderer trailRendererPlayer;
    private PlayerCollisions playerCollisions;

    // other variables
    private float originalGravity;
    private bool onCooldown;

    void Awake()
    {
        rigidbodyPlayer = GetComponent<Rigidbody2D>();
        animatorPlayer = GetComponent<Animator>();
        trailRendererPlayer = GetComponent<TrailRenderer>();
        playerCollisions = GetComponent<PlayerCollisions>();
        originalGravity = rigidbodyPlayer.gravityScale;

        Invoke("UnfreezePlayer", freezeOnAwake);
    }

    // NOTE: do not put FlipPlayer() before Run(), or it will break
    void Update()
    {

        if(! playerCollisions.GetIsAlive() || frozen)
        {
            return;
        }

        // state updates
        UpdateOnGround();
        Fall();

        // actions
        Run();
        WallSlide();
        WallJump();
        FlipPlayer();
    }


    // -----Controls-----

    // method for move input in InputSystem
    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        // disables slower diagonal movement
        moveInput.x = Mathf.Round(moveInput.x);
        moveInput.y = Mathf.Round(moveInput.y);
    }

    // method for jump input in InputSystem
    private void OnJump(InputValue value)
    {
        if(PlayerOnGround() && value.isPressed)
        {
            rigidbodyPlayer.velocity += new Vector2(0f, jumpVelocity);
        }
        else if(isWallSliding && value.isPressed)
        {
            isWallJumping = true;
            flipX = -transform.localScale.x;
            Invoke("StopWallJump", wallJumpTime);
        }
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }

    // actual wall jump mechanic, invoked in OnJump()
    private void WallJump()
    {
        if(! isWallJumping)
        {
            return;
        }

        rigidbodyPlayer.velocity = new Vector2(flipX * runVelocity, wallJumpVelocity);
    }

    // method for dash input in InputSystem
    private void OnDash(InputValue Value)
    {
        if(! canDash)
        {
            return;
        }
        StartCoroutine(Dash());
    }

    // actual dash mechanic
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        onCooldown = true;
        rigidbodyPlayer.gravityScale = 0f;

        animatorPlayer.SetBool("isDashing", true);

        // default to dashing forwards, if the player is standing still
        if(moveInput == Vector2.zero)
        {
            rigidbodyPlayer.velocity = new Vector2(moveInput.x * dashVelocity, 0f);
        }
        // diagonal dash
        else if(Math.Abs(moveInput.x) > float.Epsilon && Math.Abs(moveInput.y) > float.Epsilon)
        {
            rigidbodyPlayer.velocity = new Vector2(moveInput.x * dashVelocity * 0.6f, moveInput.y * dashVelocity * 0.6f);
        }
        // vertical and horizontal dash
        else
        {
            rigidbodyPlayer.velocity = new Vector2(moveInput.x * dashVelocity, moveInput.y * dashVelocity * 0.7f);
        }

        trailRendererPlayer.emitting = true;

        yield return new WaitForSeconds(dashTime);
        rigidbodyPlayer.gravityScale = originalGravity;
        isDashing = false;
        trailRendererPlayer.emitting = false;
        animatorPlayer.SetBool("isDashing", false);

        yield return new WaitForSeconds(dashCooldown);
        onCooldown = false;
    }

    // manages the running functionality and the animation for running
    private void Run()
    {

        if(isDashing)
        {
            return;
        }

        Vector2 moveVector = new Vector2(moveInput.x * runVelocity, rigidbodyPlayer.velocity.y);
        rigidbodyPlayer.velocity = moveVector;

        //only turn on running animation, when player is running
        bool playerHasHorizontalMovement = Mathf.Abs(rigidbodyPlayer.velocity.x) > Mathf.Epsilon;

        animatorPlayer.SetBool("isRunning", playerHasHorizontalMovement);
    }

    // manages the animations for jumping and falling
    private void Fall()
    {
        if(! PlayerOnGround())
        {
            if(isDashing) return;

            playerHasUpwardMovement = rigidbodyPlayer.velocity.y > Mathf.Epsilon;
            if(playerHasUpwardMovement)
            {
                animatorPlayer.SetBool("isFalling", false);
                animatorPlayer.SetBool("isJumping", true);
            }
            else
            {
                animatorPlayer.SetBool("isJumping", false);
                animatorPlayer.SetBool("isFalling", true);
            }
            return;
        }
        animatorPlayer.SetBool("isFalling", false);
        animatorPlayer.SetBool("isJumping", false);
    }

    // slows down the player, when moving against a wall
    private void WallSlide()
    {
        if(! PlayerOnGround() && PlayerOnWall() && moveInput.x != 0)
        {
            isWallSliding = true;
            animatorPlayer.SetBool("isSliding", true);
            rigidbodyPlayer.velocity = new Vector2(moveInput.x, Mathf.Clamp(rigidbodyPlayer.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
            animatorPlayer.SetBool("isSliding", false);
        }
    }


    // -----other methods-----

    // used to shortly freeze the player in the beginning
    public void UnfreezePlayer()
    {
        rigidbodyPlayer.bodyType = RigidbodyType2D.Dynamic;
        frozen = false;
    }


    // -----Updates and State Checks-----

    // flips the player's sprite correctly
    private void FlipPlayer()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rigidbodyPlayer.velocity.x) > Mathf.Epsilon;

        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rigidbodyPlayer.velocity.x), 1f);
        }
    }

    // necessary changes made, when touching the ground
    private void UpdateOnGround()
    {
        if(! PlayerOnGround()) return;

        // re-enable dash
        if(! onCooldown)
        {
            canDash = true;
        }
    }

    // used for correct jumping, dashing etc.
    public bool PlayerOnGround()
    {
        return playerFeet.IsTouchingLayers(LayerMask.GetMask("Terrain")) || playerFeet.IsTouchingLayers(LayerMask.GetMask("Platforms"));
    }

    // used for correct wall jumping
    public bool PlayerOnWall()
    {
        return playerFront.IsTouchingLayers(LayerMask.GetMask("Terrain"));
    }

    public bool GetPlayerHasUpwardMovement()
    {
        return playerHasUpwardMovement;
    }

}
