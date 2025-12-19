using UnityEngine;
using UnityEngine.UI;



public class GameTimer : MonoBehaviour
{

    public Text mainText;   // 59
    public Text subText;    // .32

    public float startSeconds = 60f;

    private float remainingSeconds;
    private bool isRunning;

    public float RemainingSeconds => remainingSeconds;

    public bool IsRunning => isRunning;



    private void Awake()
    {

        Debug.Log($"[GameTimer] Awake() called. startSeconds = {startSeconds}");


        remainingSeconds = startSeconds;
        isRunning = false;
        UpdateText();

    }



    private void Update()
    {

        if (!isRunning)
        {

            return;

        }

        remainingSeconds -= Time.deltaTime;

        if (remainingSeconds <= 0f)
        {

            remainingSeconds = 0f;
            isRunning = false;

        }

        UpdateText();

    }



    public void StartTimer(float seconds)
    {

        Debug.Log($"[GameTimer] StartTimer called. seconds = {seconds}");


        remainingSeconds = Mathf.Max(0f, seconds);
        isRunning = true;
        UpdateText();

    }



    public void StopTimer()
    {

        isRunning = false;

    }



    public void ResetTimer()
    {

        remainingSeconds = startSeconds;
        isRunning = false;
        UpdateText();

    }



    private void UpdateText()
    {

        if (mainText == null || subText == null)
        {

            return;

        }

        int totalCentiseconds = Mathf.FloorToInt(remainingSeconds * 100f);

        int seconds = totalCentiseconds / 100;
        int centiseconds = totalCentiseconds % 100;

        mainText.text = $"{seconds:00}";
        subText.text = $".{centiseconds:00}";

    }

}
