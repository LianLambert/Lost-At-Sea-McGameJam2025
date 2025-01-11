using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static int _lifePoints;
    public static int lifePoints
    {
        get
        {
            return _lifePoints;
        }
        set
        {
            _lifePoints = value;

            // No more life points
            if (_lifePoints < 1)
            {
                StartGameLoss();
            }
        }
    }

    public static int coins;
    public static int timeLeft;
    public static int boatCount;
    public Difficulty difficulty;
    public static Animator WinStates;


    // Start is called before the first frame update
    void Start()
    {

        // TO DO: Manage these values when playtesting
        switch (difficulty)
        {
            case Difficulty.Easy:
                lifePoints = 3;
                coins = 50;
                timeLeft = 300;
                boatCount = 1;
                break;

            case Difficulty.Medium:
                lifePoints = 3;
                coins = 80;
                timeLeft = 400;
                boatCount = 3;
                break;

            case Difficulty.Hard:
                lifePoints = 3;
                coins = 100;
                timeLeft = 500;
                boatCount = 5;
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
