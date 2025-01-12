using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CustomLoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMainMenudirtty()
    {
        GameManager.gameIsOver = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadSAMPLESCENEdirtty()
    {
        GameManager.gameIsOver = false;
        SceneManager.LoadScene("SampleScene 1");
    }

    public void LoadCURRENTdirtty()
    {
        GameManager.gameIsOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetDifficulty(int diff)
    {
        if (diff == 0)
            GameManager.difficulty = Difficulty.Easy;
        if (diff == 1)
            GameManager.difficulty = Difficulty.Medium;
        if (diff == 2)
            GameManager.difficulty = Difficulty.Hard;
    }
}
