using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static int numLightHouses;
    private static int _numLives;
    public static int numLives
    {
        get
        {
            return _numLives;
        }
        set
        {
            _numLives = value;

            // No more life points
            if (_numLives < 1)
            {
                StartGameLoss();
            }
        }
    }

    private static int _numBoats;

    public static int numBoatsCollected
    {
        get
        {
            return _numBoats;
        }

        set
        {
            _numBoats = value;
            string newBoatText = _numBoats + "/" + numBoatGoal;
            Debug.Log(newBoatText);
            GameObject.FindGameObjectWithTag("NumBoats").GetComponent<TextMeshProUGUI>().text = newBoatText;

            // All boats found
            if (_numBoats >= numBoatGoal)
            {
                StartGameWin();
            }
        }
    }

    public static int numCoins;
    public static int timeRemaining;
    public static int numTilesRevealed;
    public static int numBoatGoal;
    public static Difficulty difficulty;
    public static Animator WinStates;
    public static bool gameIsOver = false;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // TO DO: Manage these values when playtesting
        switch (difficulty)
        {
            case Difficulty.Easy:
                numCoins = 50;
                timeRemaining = 300;
                numBoatGoal = 1;
                numLightHouses = 4;
                break;

            case Difficulty.Medium:
                numCoins = 80;
                timeRemaining = 400;
                numBoatGoal = 3;
                numLightHouses = 3;
                break;

            case Difficulty.Hard:
                numCoins = 100;
                timeRemaining = 500;
                numBoatGoal = 5;
                numLightHouses = 2;
                break;
        }

        numBoatsCollected = 0;
        numLives = 3;
        numTilesRevealed = 0;
        GameObject.FindGameObjectWithTag("NumLightHouses").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.numLightHouses.ToString();
        GameObject.FindGameObjectWithTag("NumCoinsText").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.numCoins.ToString();

        // Timer coroutine every second
        Invoke("TimerCountdown", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static void StartGameLoss()
    {
        // to do: implement
        // WinStates.Play("GameLoss");
    }

    private static void StartGameWin()
    {
        //WinStates.Play("GameWin");

        //TODO Play Win Sound/Animations
        gameIsOver = true;
        _instance.StartCoroutine(_instance.DelayedWinReveal());
    }

    private IEnumerator DelayedWinReveal()
    {
        yield return new WaitForSeconds(1.5f); // Wait for 2 seconds

        // Reveal the win panel
        var WinPanel = FindInactiveByTag.FindInactiveGameObjectByTag("WinPanel");
        if (WinPanel != null)
        {
            WinPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("WinPanel not found!");
        }
    }

    static string FormatTime(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes}:{seconds:D2}"; // Format seconds with two digits
    }

    void TimerCountdown()
    {
        if (gameIsOver)
            return;

        if (timeRemaining == 0)
        {
            StartGameLoss();
        }
        GameObject.FindGameObjectWithTag("NumTimeLeft").GetComponent<TextMeshProUGUI>().text = FormatTime(timeRemaining);
        timeRemaining--;
        Invoke("TimerCountdown", 1f);


    }

    // Pausing the game, covering the screen
    public void Pause()
    {
        // TO DO
    }

    // Resume the game, removing the screen cover
    public void Resume()
    {
        // TO DO
    }

    // Quitting the game, going back to main menu
    public void Quit()
    {
        // TO DO
    }
}


public class FindInactiveByTag : MonoBehaviour
{
    public static GameObject FindInactiveGameObjectByTag(string tag)
    {
        GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allGameObjects)
        {
            if (obj.CompareTag(tag) && obj.hideFlags == HideFlags.None)
            {
                return obj;
            }
        }

        return null; // Return null if no matching GameObject is found
    }
}