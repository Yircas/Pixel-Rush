using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : MonoBehaviour
{

    private Scene scene;

    void Awake()
    {
        // i don't need it, i don't need it, i don't need it... i need it?
        scene = SceneManager.GetActiveScene();
    }

    public void ResetCurrentScene(float delay)
    {
        Invoke("ResetCurrentSceneHelper", delay);
    }

    // used, because I can't friggin Invoke SceneManager.LoadScene(...)
    private void ResetCurrentSceneHelper()
    {
        SceneManager.LoadScene(scene.buildIndex);
    }
}
