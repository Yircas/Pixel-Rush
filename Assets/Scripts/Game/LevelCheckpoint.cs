using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCheckpoint : MonoBehaviour
{
    // countdownIncrease: value that increases or decreases timer.countdownTime, ignored when timer.isCountingDown = false
    [Header("Options")]
    [SerializeField] float countdownIncrease = 0f;
    
    private Timer timer;
    private bool isActivated = false;
    private Animator animator;

    void Start()
    {
        timer = FindObjectOfType<Timer>();
        animator = GetComponent<Animator>();
        if(timer == null)
        {
            Debug.Log("LevelCheckpoint: Timer not found!");
        }
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if( isActivated || other.tag != "Player")
        {
            return;
        }

        isActivated = true;
        animator.SetBool("isActivated", true);
        timer.ChangeCountdown(countdownIncrease);
    }

    // used by SessionManager.cs
    public bool GetIsActivated()
    {
        return isActivated;
    }
}
