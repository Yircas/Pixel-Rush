using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{

    // Options for Testing
    [Header("Settings")]
    [SerializeField] bool resetLevel = false;
    [SerializeField] float loadLevelCooldown = 1f;

    // components and Game Objects
    private Animator animator;
    private SessionManager sessionManager;
    private ParticleSystem particleSystem;

    // other variables
    private bool canLoadNextLevel;

    void Awake()
    {
        animator = GetComponent<Animator>();
        sessionManager = FindObjectOfType<SessionManager>();
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag != "Player" || animator.GetBool("isPressed"))
        {
            return;
        }

        // play animations
        animator.SetBool("isPressed", true);
        particleSystem.Play();

        // for testing purposes
        if(resetLevel)
        {
            Debug.Log("LevelEnd: Resetting level...");
            sessionManager.LoadLevel(SceneManager.GetActiveScene().buildIndex, 1f);
        }
    }

}
