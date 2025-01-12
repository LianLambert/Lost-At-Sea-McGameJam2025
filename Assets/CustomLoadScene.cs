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
}
