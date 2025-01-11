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

    public static int numBoats
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
    public static int timeLeft;
    public static int numBoatGoal;
    public Difficulty difficulty;
    public static Animator WinStates;


    // Start is called before the first frame update
    void Start()
    {
        numBoats = 0;
        numLives = 3;

        // TO DO: Manage these values when playtesting
        switch (difficulty)
        {
            case Difficulty.Easy:
                numCoins = 50;
                timeLeft = 300;
                numBoatGoal = 1;
                break;

            case Difficulty.Medium:
                numCoins = 80;
                timeLeft = 400;
                numBoatGoal = 3;
                break;

            case Difficulty.Hard:
                numCoins = 100;
                timeLeft = 500;
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
        WinStates.Play("GameLoss");
    }

    private static void StartGameWin()
    {
        WinStates.Play("GameWin");
    }

    IEnumerator TimerCountdown()
    {
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1);
            timeLeft--;
        }

        StartGameLoss();
    }
}
