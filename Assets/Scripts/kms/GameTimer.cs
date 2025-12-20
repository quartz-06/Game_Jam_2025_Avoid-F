using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{

    public Text mainText;   // 59
    public Text subText;    // .32

    public float startSeconds = 60f;

    [Header("Color By Time (Hard Steps)")]
    public Color normalColor = new Color(0.15f, 0.15f, 0.15f, 1f);     // £�� ȸ��(���� ����)
    public Color warningColor = new Color(1f, 0.35f, 0.0f, 1f);        // £�� ��Ȳ(�� ������)
    public Color dangerColor = new Color(1f, 0.1f, 0.1f, 1f);          // ����

    [Range(0f, 1f)]
    public float warningStartRatio = 0.50f;                            // �� ���� ���Ϻ��� warningColor

    [Range(0f, 1f)]
    public float dangerStartRatio = 0.20f;                             // �� ���� ���Ϻ��� dangerColor

    [Header("Blink By Time (Hard Steps)")]
    public bool useBlink = true;

    public float warningBlinkInterval = 0.35f;                         // ��Ȳ ���� ������ ����(��)
    public float dangerBlinkInterval = 0.18f;                          // ���� ���� ������ ����(��)

    private float remainingSeconds;
    private bool isRunning;

    private float blinkTimer = 0f;
    private bool blinkVisible = true;

    public float RemainingSeconds => remainingSeconds;

    public bool IsRunning => isRunning;



    private void Awake()
    {

        Debug.Log($"[GameTimer] Awake() called. startSeconds = {startSeconds}");


        remainingSeconds = startSeconds;
        isRunning = false;

        blinkTimer = 0f;
        blinkVisible = true;

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

        blinkTimer = 0f;
        blinkVisible = true;

        UpdateText();

    }



    public void StopTimer()
    {

        isRunning = false;

        blinkTimer = 0f;
        blinkVisible = true;

        ApplyBlinkVisibility(true);

    }



    public void ResetTimer()
    {

        remainingSeconds = startSeconds;
        isRunning = false;

        blinkTimer = 0f;
        blinkVisible = true;

        ApplyBlinkVisibility(true);

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

        mainText.text = $"과제 제출 마감까지{seconds:00}";
        subText.text = $".{centiseconds:00}";

        ApplyTimeColor_HardSteps();
        ApplyBlink_HardSteps();

    }



    private void ApplyTimeColor_HardSteps()
    {

        if (startSeconds <= 0f)
        {

            return;

        }

        float ratio = Mathf.Clamp01(remainingSeconds / startSeconds);

        Color targetColor;

        if (ratio <= dangerStartRatio)
        {

            targetColor = dangerColor;

        }
        else if (ratio <= warningStartRatio)
        {

            targetColor = warningColor;

        }
        else
        {

            targetColor = normalColor;

        }

        mainText.color = targetColor;
        subText.color = targetColor;

    }



    private void ApplyBlink_HardSteps()
    {

        if (!useBlink)
        {

            ApplyBlinkVisibility(true);
            return;

        }

        if (!isRunning)
        {

            ApplyBlinkVisibility(true);
            return;

        }

        if (startSeconds <= 0f)
        {

            ApplyBlinkVisibility(true);
            return;

        }

        float ratio = Mathf.Clamp01(remainingSeconds / startSeconds);

        float interval;

        if (ratio <= dangerStartRatio)
        {

            interval = dangerBlinkInterval;

        }
        else if (ratio <= warningStartRatio)
        {

            interval = warningBlinkInterval;

        }
        else
        {

            ApplyBlinkVisibility(true);
            blinkTimer = 0f;
            blinkVisible = true;
            return;

        }

        blinkTimer += Time.deltaTime;

        if (blinkTimer >= interval)
        {

            blinkTimer = 0f;
            blinkVisible = !blinkVisible;

        }

        ApplyBlinkVisibility(blinkVisible);

    }



    private void ApplyBlinkVisibility(bool visible)
    {

        mainText.enabled = visible;
        subText.enabled = visible;

    }

}
