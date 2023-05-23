using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerOld : MonoBehaviour
{

    //Adjust the players movement speeds
    [Header("Movement")]
    [SerializeField] float runVelocity = 5f;
    [SerializeField] float jumpVelocity = 5f;
    private Vector2 moveInput;
    private bool playerHasDoubleJumped;
    private bool playerHasUpwardMovement;

    //Contains the individual rigidbodies to be used for the player
    [Header("Rigidbody Components")]
    [SerializeField] PolygonCollider2D playerBody;
    [SerializeField] BoxCollider2D playerFeet;

    //Options for dashing
    [Header("Dashing")]
    [SerializeField] float dashVelocity = 40f;
    [SerializeField] float dashTime = 0.1f;
    [SerializeField] float dashCooldown = 0.5f;
    private Vector2 dashDirection;
    private bool isDashing;
    private bool canDash = true;

    //referenced components
    private Rigidbody2D rigidbodyPlayer;
    private Animator animatorPlayer;
    private TrailRenderer trailRendererPlayer;

    // other variables
    private float originalGravity;
    private bool onCooldown;

    void Awake()
    {
        rigidbodyPlayer = GetComponent<Rigidbody2D>();
        animatorPlayer = GetComponent<Animator>();
        trailRendererPlayer = GetComponent<TrailRenderer>();
        originalGravity = rigidbodyPlayer.gravityScale;
    }

    void Update()
    {
        UpdateOnGround();
        Run();
        Fall();
        FlipSprite();
    }

    // flips the player's sprite correctly
    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rigidbodyPlayer.velocity.x) > Mathf.Epsilon;

        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rigidbodyPlayer.velocity.x), 1f);
        }
    }

    // method for move input in InputSystem
    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // method for jump input in InputSystem
    private void OnJump(InputValue value)
    {
        LayerMask terrainLayer = LayerMask.GetMask("Terrain");

        if(playerFeet.IsTouchingLayers(terrainLayer) && value.isPressed)
        {
            rigidbodyPlayer.velocity += new Vector2(0f, jumpVelocity);
        }
    }

    private void OldOnJump(InputValue value)
    {
        LayerMask terrainLayer = LayerMask.GetMask("Terrain");

        if(playerFeet.IsTouchingLayers(terrainLayer) && value.isPressed)
        {
            rigidbodyPlayer.velocity += new Vector2(0f, jumpVelocity);
            playerHasDoubleJumped = false;
        }
        else if(value.isPressed && ! playerHasDoubleJumped)
        {
            rigidbodyPlayer.velocity = new Vector2(moveInput.x, jumpVelocity * 1f);
            animatorPlayer.SetBool("isJumping", false);
            animatorPlayer.SetBool("isFalling", false);
            animatorPlayer.SetBool("isDoubleJumping", true);
            playerHasDoubleJumped = true;
        }
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
        // TODO: fix animation here
        if(moveInput == Vector2.zero)
        {
            rigidbodyPlayer.velocity = new Vector2(transform.localScale.x * dashVelocity, 0f);
        }
        else if(Math.Abs(moveInput.x) > float.Epsilon && Math.Abs(moveInput.y) > float.Epsilon)
        {
            rigidbodyPlayer.velocity = new Vector2(moveInput.x * dashVelocity * 0.9f, moveInput.y * dashVelocity * 0.9f);
        }
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

        if(playerHasDoubleJumped) return;
        animatorPlayer.SetBool("isRunning", playerHasHorizontalMovement);
    }

    // manages the animations for jumping and falling
    private void Fall()
    {
        bool playerIsOnGround = playerFeet.IsTouchingLayers(LayerMask.GetMask("Terrain"));
        if(! playerIsOnGround)
        {
            if(playerHasDoubleJumped || isDashing) return;

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

    // necessary changes made, when touching the ground
    // 1. re-enable double jump
    private void UpdateOnGround()
    {
        LayerMask terrainLayer = LayerMask.GetMask("Terrain");
        if(! playerFeet.IsTouchingLayers(terrainLayer)) return;

        playerHasDoubleJumped = false;
        animatorPlayer.SetBool("isDoubleJumping", false);

        if(! onCooldown)
        {
            canDash = true;
        }
    }

    public bool GetPlayerHasUpwardMovement()
    {
        return playerHasUpwardMovement;
    }

}
