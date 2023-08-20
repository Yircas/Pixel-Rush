using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour
{
    // Options for starting the Level
    [Header("Start Settings")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject SpawnPoint;

    private Animator animator;


    void Awake()
    {
        SpawnPlayer();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag != "Player")
        {
            return;
        }

        animator.SetBool("isMoving", true);
        Invoke("SetIsMoving", 0.4f);
    }

    private void SetIsMoving()
    {
        animator.SetBool("isMoving", false);
    }

    private void SpawnPlayer()
    {
        if(! GameObject.FindWithTag("Player"))
        {
            Instantiate(player, SpawnPoint.transform);
        }
    }

}
