using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
                Camera.main.GetComponent<GameManager>().StartGameLoss();
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
                Camera.main.GetComponent<GameManager>().StartGameWin();
            }
        }
    }

    public static int numCoins;
    public static int timeRemaining;
    public static int timeElapsed;
    public static int numTilesRevealed;
    public static int numBoatGoal;
    public static Difficulty difficulty;
    public static Animator WinStates;
    public static bool gameIsOver = false;

    // Start is called before the first frame update
    void Start()
    {
        // TO DO: Manage these values when playtesting
        switch (difficulty)
        {
            case Difficulty.Easy:
                numCoins = 50;
                timeRemaining = 300;
                numBoatGoal = 2;
                numLightHouses = 2;
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
                numLightHouses = 5;
                break;
        }

        timeElapsed = 0;
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

    public void StartGameLoss()
    {
        // to do: implement
        // WinStates.Play("GameLoss");
        gameIsOver = true;


        Invoke("DelayedLossReveal", 1.5f);

    }

    private void DelayedLossReveal()
    {
        // Reveal the win panel
        var WinPanel = FindInactiveByTag.FindInactiveGameObjectByTag("LossPanel");
        if (WinPanel != null)
        {
            WinPanel.SetActive(true);
            Camera.main.GetComponent<AudioManager>().PlaySound("EndOfGame - Lose");
        }
        else
        {
            Debug.LogError("WinPanel not found!");
        }
    }

    public void StartGameWin()
    {
        //WinStates.Play("GameWin");

        //TODO Play Win Sound/Animations
        gameIsOver = true;
        Invoke("DelayedWinReveal", 1.5f);
    }

    private void DelayedWinReveal()
    {

        // Reveal the win panel
        var WinPanel = FindInactiveByTag.FindInactiveGameObjectByTag("WinPanel");
        if (WinPanel != null)
        {
            WinPanel.SetActive(true);
            Camera.main.GetComponent<AudioManager>().PlaySound("EndOfGame - Win");

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
        timeElapsed++;

        if(timeElapsed % 50 == 0 && timeRemaining > 0)
        {
            if (difficulty == Difficulty.Easy)
            {
                if (timeRemaining > 150)
                {
                   Camera.main.GetComponent<AudioManager>().PlaySound("TimerChill");
                }

                else if (timeRemaining > 50)
                {
                    Camera.main.GetComponent<AudioManager>().PlaySound("TimerMedium");
                }

                else
                {
                    Camera.main.GetComponent<AudioManager>().PlaySound("TimerUrgent");
                }
            }

            else if (difficulty == Difficulty.Medium)
            {
                if (timeRemaining > 200)
                {
                    Camera.main.GetComponent<AudioManager>().PlaySound("TimerChill");
                }

                else if (timeRemaining > 50)
                {
                    Camera.main.GetComponent<AudioManager>().PlaySound("TimerMedium");
                }

                else
                {
                    Camera.main.GetComponent<AudioManager>().PlaySound("TimerUrgent");
                }
            }

            else if (difficulty == Difficulty.Hard)
            {
                if (timeRemaining > 200)
                {
                    Camera.main.GetComponent<AudioManager>().PlaySound("TimerChill");
                }

                else if (timeRemaining > 50)
                {
                    Camera.main.GetComponent<AudioManager>().PlaySound("TimerMedium");
                }

                else
                {
                    Camera.main.GetComponent<AudioManager>().PlaySound("TimerUrgent");
                }
            }
        }
        

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

    // to do: use this function
    // called if you try to spin the wheel and you end up with a negative balance
    // you can "start" spin if you have a positive balance
    private void DebtCollection()
    {
        // Reveal the debt panel
        var DebtPanel = FindInactiveByTag.FindInactiveGameObjectByTag("DebtPanel");
        if (DebtPanel != null)
        {
            DebtPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("DebtPanel not found!");
        }
    }
}