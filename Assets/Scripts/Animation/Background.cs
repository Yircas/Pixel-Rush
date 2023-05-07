using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Background : MonoBehaviour
{
    //Determines the 2D-movement for the texture
    [SerializeField] Vector2 scrollSpeed;

    Vector2 offset;
    Material material;

    void Awake()
    {
        material = GetComponent<TilemapRenderer>().material;
    }

    
    void Update()
    {
        offset = -scrollSpeed * Time.deltaTime;
        material.mainTextureOffset += offset;
    }
}
