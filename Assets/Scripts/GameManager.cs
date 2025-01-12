using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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


    // Start is called before the first frame update
    void Start()
    {
        numBoatsCollected = 0;
        numLives = 3;
        numTilesRevealed = 0;

        // TO DO: Manage these values when playtesting
        switch (difficulty)
        {
            case Difficulty.Easy:
                numCoins = 50;
                timeRemaining = 300;
                numBoatGoal = 1;
                break;

            case Difficulty.Medium:
                numCoins = 80;
                timeRemaining = 400;
                numBoatGoal = 3;
                break;

            case Difficulty.Hard:
                numCoins = 100;
                timeRemaining = 500;
                numBoatGoal = 5;
                break;
        }

        // Timer coroutine every second
        StartCoroutine(TimerCountdown());
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
        // to do: implement
        // WinStates.Play("GameWin");
    }

    IEnumerator TimerCountdown()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1);
            timeRemaining--;
        }

        StartGameLoss();
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
