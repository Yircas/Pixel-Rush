using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SessionManager : MonoBehaviour
{
    [Header("User Interface")]
    [SerializeField] TextMeshProUGUI scoreText;
    
    [Header("Other Components")]
    [SerializeField] Timer timer;

    // Variables for counting fruit:
    private int totalFruitCount;
    private int collectedFruitCount;


    void Awake()
    {
        CheckSingleton();
        totalFruitCount = CountFruits();
    }

    void Update() 
    {
        DisplayFruitCount();
    }

    // Reloads current level without destroying the session manager
    public void ResetCurrentScene(float delay)
    {
        Invoke("ResetCurrentSceneHelper", delay);
    }

    // Used, because I can't friggin Invoke SceneManager.LoadScene(...)
    private void ResetCurrentSceneHelper()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        collectedFruitCount = 0;
        timer.ResetCountdown();
    }

    // Loads a specific scene represented by its build index
    // resets the session manager
    // stops the timer
    public void LoadLevel(int sceneIndex, float delay)
    {
        timer.isActivated = false;
        StartCoroutine(LoadLevelHelper(sceneIndex, delay));
    }

    private IEnumerator LoadLevelHelper(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneIndex);
        Destroy(gameObject);
    }

    // Initializing total amount of fruits, when starting the level
    private int CountFruits()
    {
        return GameObject.FindGameObjectsWithTag("Fruit").Length;
    }

    // Called by fruit instances to increment the collectedFruitCount variable
    public void increaseCollectedFruitCount()
    {
        collectedFruitCount++;
    }

    // Updates the fruit count on the screen
    private void DisplayFruitCount()
    {
        scoreText.text = collectedFruitCount.ToString() + "/" + totalFruitCount.ToString();
    }

    // Ensures that excessive session managers are destroyed, same as in ScenePersist.cs
    // WIP: Implement the SingletonBase class and use that instead of this crap
    private void CheckSingleton()
    {
        int instanceCount = FindObjectsOfType<SessionManager>().Length;

        if(instanceCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    
    }
}