using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.ComponentModel.Design.Serialization;

public class Timer : MonoBehaviour
{
    // Only used for displaying the time on the UI
    // timeText: Displays the elapsed time
    // isActivated: Turns the timer on
    [Header("General")]
    [SerializeField] TextMeshProUGUI timeText;
    public bool isActivated = true;

    // isCountingDown: Changes the current scene into a "time race"
    // countdownText: Displays the countdown
    // changeText: Displays the time added to or subtracted from the countdown by external factors, such as activating checkpoints
    // countdownTime: Sets the starting time for the countdown
    [Header("Time Race")]
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] TextMeshProUGUI changeText;
    public float countdownTime = 60f;
    private float initialCountdownTime;
    public bool isCountingDown = false;
    private bool initialIsCountingDown;
    [SerializeField] float changeDisplayTime = 0.5f;


    // These are used by DisplayCountdownChange
    [Header("Timer Colors")]
    [SerializeField] Color white = new Color(1, 1, 1, 0.8f);
    [SerializeField] Color transparent = new Color(1, 1, 1, 0);
    [SerializeField] Color green = new Color(0.2f, 1, 0.2f, 0.8f);
    [SerializeField] Color red = new Color(1, 0.2f, 0.2f, 0.8f);

    // Variables for the basic timer
    private DateTime startTime;
    private TimeSpan elapsedTime;

    // Variables for the countdown system
    // countdownTime is accessed by LevelCheckpoint.cs to increase the countdown
    public PlayerCollisions player;
    
    void Start()
    {
        // Initiallisation
        // Apply system time as starting point
        player = FindObjectOfType<PlayerCollisions>();
        startTime = DateTime.Now;
        initialCountdownTime = countdownTime;
        initialIsCountingDown = isCountingDown;

        // Forces the UI elements to be invisible by default
        countdownText.color = transparent;
        changeText.color = transparent;
        timeText.color = transparent;

        if(player == null)
        {
            Debug.Log("Timer: Player not found!");
        }

        if(isActivated)
        {
            timeText.color = white;
        }

        if(isCountingDown)
        {
            countdownText.color = white;
        }
    }

    
    void Update()
    {
        DisplayTime();
        DisplayCountdown();
    }

    // Optional, when making the level a "Time Race" with isCountingDown = true
    // Displays the Countdown
    private void DisplayCountdown()
    {
        if(! isActivated || ! isCountingDown)
        {
            return;
        }

        if(countdownTime <= 0f && player != null)
        {
            player.KillPlayer();
            isCountingDown = false;
            return;
        }

        countdownTime -= Time.deltaTime;
        countdownText.text = countdownTime.ToString("0.0");

    }

    // Shows the elapsed time on the UI text when activated
    private void DisplayTime()
    {
        if(! isActivated)
        {
            return;
        }

        elapsedTime = DateTime.Now - startTime;
        timeText.text = FormatString(elapsedTime);
    }

    // Formats the time
    private string FormatString(TimeSpan timeSpan)
    {
        // Calculate the total seconds and decimal seconds
        double totalSeconds = timeSpan.TotalSeconds;
        int minutes = (int)(totalSeconds / 60);
        int seconds = (int)(totalSeconds % 60);
        int decimalSeconds = (int) ((totalSeconds - (minutes * 60 + seconds)) * 60);

        // Format the time as minutes, seconds, and decimal seconds
        string formattedTime = string.Format("{0:D1}:{1:D2}:{2:D2}", minutes, seconds, decimalSeconds);

        return formattedTime;
    }

    // Resets the countdown, used by SessionManager.cs
    public void ResetCountdown()
    {
        countdownTime = initialCountdownTime;
        isCountingDown = initialIsCountingDown;
    }

    // adds or subtracts the time value to/from the current countdownTime
    public void ChangeCountdown(float time)
    {
        if(! isCountingDown)
        {
            return;
        }

        countdownTime += time;
        StartCoroutine(DisplayCountdownChange(time));

    }

    // Temporarely shows the added or subtracted time for 
    private IEnumerator DisplayCountdownChange(float time)
    {
        if(time >= 0)
        {
            changeText.text = "+" + time.ToString("0.0");
            changeText.color = green;
        }
        else
        {
            changeText.text = time.ToString("0.0");
            changeText.color = red;
        }

        yield return new WaitForSeconds(changeDisplayTime);

        changeText.color = transparent;
    }
}
