using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFruit : MonoBehaviour
{
    // TODO: Integrate options for power-ups?
    [Header("Collection Settings")]
    [SerializeField] float destroyDelay = 5f;

    // other variables
    private Animator animator;
    private SessionManager countManager;


    void Start()
    {
        animator = GetComponent<Animator>();
        countManager = FindObjectOfType<SessionManager>();
    }

    // DESTRRROY the fruit
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag != "Player" || animator.GetBool("wasCollected"))
        {
            return;
        }
        Debug.Log("Collected");


        animator.SetBool("wasCollected", true);
        countManager.increaseCollectedFruitCount();

        Invoke("DestroyThis", destroyDelay);
    }

    private void DestroyThis() 
    {
        Destroy(this.gameObject);
    }

}
