using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFruit : MonoBehaviour
{
    // TODO: Integrate options for power-ups?
    [Header("Collection Settings")]
    [SerializeField] float destroyDelay = 5f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // DESTRRROY the fruit
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag != "Player")
        {
            return;
        }
        Debug.Log("Collected");


        animator.SetBool("wasCollected", true);
        
        Invoke("DestroyThis", destroyDelay);
    }

    private void DestroyThis() 
    {
        Destroy(this.gameObject);
    }

}
