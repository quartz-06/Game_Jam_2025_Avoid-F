
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextBlinkRedWhite : MonoBehaviour
{

    [Header("Target")]
    public Text targetText;

    [Header("Blink")]
    public float interval = 0.25f;

    [Header("Colors")]
    public Color red = Color.red;
    public Color white = Color.white;

    private Coroutine blinkCoroutine;



    private void Awake()
    {

        if (targetText == null)
        {

            targetText = GetComponent<Text>();

        }

    }



    private void OnEnable()
    {

        StartBlink();

    }



    private void OnDisable()
    {

        StopBlink();

    }



    public void StartBlink()
    {

        if (blinkCoroutine != null)
        {

            StopCoroutine(blinkCoroutine);

        }

        blinkCoroutine = StartCoroutine(BlinkRoutine());

    }



    public void StopBlink()
    {

        if (blinkCoroutine != null)
        {

            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;

        }

        if (targetText != null)
        {

            targetText.color = white;

        }

    }



    private IEnumerator BlinkRoutine()
    {

        if (targetText == null)
        {

            yield break;

        }

        bool isRed = false;

        while (true)
        {

            isRed = !isRed;
            targetText.color = isRed ? red : white;

            yield return new WaitForSeconds(interval);

        }

    }

}

