using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Transition Settings")]
    [SerializeField] private Image fadeImage; // Fullscreen UI Image for fade
    [SerializeField] private float fadeDuration = 1f;
    

    private bool isTransitioning = false;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Start with a fade-in effect
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            //StartCoroutine(FadeIn());
        }

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Camera.main.GetComponent<AudioManager>().PlayMusic("Menu Theme - Guiding Light");
            Debug.Log("menu music!");


        }

        if (SceneManager.GetActiveScene().name == "SampleScene1")
        {
            Camera.main.GetComponent<AudioManager>().PlayMusic("Level Theme - Lost at Sea");
            Debug.Log("level music!");
        }
    }

    /// <summary>
    /// Load a new scene with a fade transition.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(Transition(sceneName));
        }

        if (sceneName == "MainMenu")
        {
            Camera.main.GetComponent<AudioManager>().PlayMusic("Menu Theme - Guiding Light");

        }

        if (sceneName == "SampleScene1")
        {
            Camera.main.GetComponent<AudioManager>().PlayMusic("Level Theme - Lost at Sea");
        }
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

    public void ReloadCurrentScene()
    {
        GameManager.gameIsOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Coroutine to handle scene transition with fade-out and fade-in.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load.</param>
    private IEnumerator Transition(string sceneName)
    {
        isTransitioning = true;

        // Fade-out
        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeOut());
        }

        // Load the new scene
        var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        fadeImage = GameObject.FindGameObjectWithTag("fadeIn").GetComponent<Image>();

        // Fade-in
        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeIn());
        }

        isTransitioning = false;
    }

    /// <summary>
    /// Fade-out effect.
    /// </summary>
    private IEnumerator FadeOut()
    {
        Color color = fadeImage.color;
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1;
        fadeImage.color = color;
    }

    /// <summary>
    /// Fade-in effect.
    /// </summary>
    private IEnumerator FadeIn()
    {
        Color color = fadeImage.color;
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);
    }
}
