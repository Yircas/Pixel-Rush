using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    // wich instance to lock on by default
    // [Header("Settings")]
    // [SerializeField] GameObject followTarget;

    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        this.GetComponent<CinemachineStateDrivenCamera>().Follow = player.transform;
        this.GetComponent<CinemachineStateDrivenCamera>().m_AnimatedTarget = player.GetComponent<Animator>();
    }
}
