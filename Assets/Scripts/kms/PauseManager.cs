using UnityEngine;

public class PauseManager : MonoBehaviour
{

    [Header("Pause Panel")]
    public GameObject pausePanel;



    [Header("Keys")]
    public KeyCode pauseKey = KeyCode.Escape;



    private bool isPaused = false;



    private void Awake()
    {

        if (pausePanel != null)
        {

            pausePanel.SetActive(false);

        }

        ResumeGame();

    }



    private void Update()
    {

        if (Input.GetKeyDown(pauseKey))
        {

            TogglePause();

        }

    }



    public void TogglePause()
    {

        if (isPaused)
        {

            ResumeGame();

        }
        else
        {

            PauseGame();

        }

    }



    public void PauseGame()
    {

        if (isPaused)
        {

            return;

        }

        isPaused = true;



        if (pausePanel != null)
        {

            pausePanel.SetActive(true);

        }



        Time.timeScale = 0f;

    }



    public void ResumeGame()
    {

        isPaused = false;



        if (pausePanel != null)
        {

            pausePanel.SetActive(false);

        }



        Time.timeScale = 1f;

    }



    public bool IsPaused()
    {

        return isPaused;

    }

}
