using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    //Adjust the players movement speeds
    [Header("Movement")]
    [SerializeField] float runVelocity = 5f;
    [SerializeField] float jumpVelocity = 5f;

    //Contains the individual rigidbodies to be used for the player
    [Header("Rigidbody Components")]
    [SerializeField] CapsuleCollider2D playerBody;
    [SerializeField] BoxCollider2D playerFeet;

    private Vector2 moveInput;
    private bool playerHasDoubleJumped;

    private Rigidbody2D rigidbodyPlayer;
    private Animator animatorPlayer;

    void Awake()
    {
        rigidbodyPlayer = GetComponent<Rigidbody2D>();
        animatorPlayer = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateOnGround();
        Run();
        Fall();
        FlipSprite();
    }

    //flips the player's sprite correctly
    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rigidbodyPlayer.velocity.x) > Mathf.Epsilon;

        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rigidbodyPlayer.velocity.x), 1f);
        }
    }

    //method for move input in InputSystem
    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    //method for jump input in InputSystem
    //TODO: replicate the double jump mechanic from celeste
    private void OnJump(InputValue value)
    {
        LayerMask terrainLayer = LayerMask.GetMask("Terrain");

        if(playerFeet.IsTouchingLayers(terrainLayer) && value.isPressed)
        {
            rigidbodyPlayer.velocity += new Vector2(0f, jumpVelocity);
            playerHasDoubleJumped = false;
        }
        else if(value.isPressed && ! playerHasDoubleJumped)
        {
            rigidbodyPlayer.velocity = new Vector2(moveInput.x, jumpVelocity * 0.8f);
            animatorPlayer.SetBool("isJumping", false);
            animatorPlayer.SetBool("isFalling", false);
            animatorPlayer.SetBool("isDoubleJumping", true);
            playerHasDoubleJumped = true;
        }
    }

    //manages the running functionality and the animation for running
    private void Run()
    {
        Vector2 moveVector = new Vector2(moveInput.x * runVelocity, rigidbodyPlayer.velocity.y);
        rigidbodyPlayer.velocity = moveVector;

        //only turn on running animation, when player is running
        bool playerHasHorizontalMovement = Mathf.Abs(rigidbodyPlayer.velocity.x) > Mathf.Epsilon;

        if(playerHasDoubleJumped) return;
        animatorPlayer.SetBool("isRunning", playerHasHorizontalMovement);
    }

    //manages the animations for jumping and falling
    private void Fall()
    {
        bool playerIsOnGround = playerFeet.IsTouchingLayers(LayerMask.GetMask("Terrain"));
        if(! playerIsOnGround)
        {
            if(playerHasDoubleJumped) return;

            bool playerHasUpwardMovement = rigidbodyPlayer.velocity.y > Mathf.Epsilon;
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

    //necessary changes made, when touching the ground
    private void UpdateOnGround()
    {
        LayerMask terrainLayer = LayerMask.GetMask("Terrain");
        if(! playerFeet.IsTouchingLayers(terrainLayer)) return;

        playerHasDoubleJumped = false;
        animatorPlayer.SetBool("isDoubleJumping", false);
    }
}
