using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollisions : MonoBehaviour
{
    // Colliders used by the player
    [Header("Collider Components")]
    [SerializeField] BoxCollider2D playerBody;
    Rigidbody2D rigidbodyPlayer;

    // Enables death and how fast the current scene resets on death
    [Header("Death Options")]
    [SerializeField] bool canDie = true;
    [SerializeField] float resetTime = 0.5f;
    private bool isAlive = true;
    private Animator animator;
    private SessionManager sessionManager;

    // Layers
    private LayerMask trap;

    void Awake()
    {
        GetLayers();
        rigidbodyPlayer = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sessionManager = FindObjectOfType<SessionManager>();
    }

    
    void Update()
    {
        CheckCollisions();
    }

    public void CheckCollisions()
    {
        if(! canDie)
        {
            return;
        }

        if(playerBody.IsTouchingLayers(trap))
        {
            KillPlayer();
        }
    }

    // disable player controls
    public void KillPlayer()
    {
        isAlive = false;

        // freezes the player in place and resets level
        rigidbodyPlayer.bodyType = RigidbodyType2D.Static;
        // animator.enabled = false;
        animator.SetBool("hasDied", true);
        sessionManager.ResetCurrentScene(resetTime);
    }

    // abstract all layers used by CheckCollision()
    private void GetLayers()
    {
        trap = LayerMask.GetMask("Traps");
    }

    // abstract isAlive
    public bool GetIsAlive()
    {
        return isAlive;
    }

}
