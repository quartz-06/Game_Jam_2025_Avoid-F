using System.Collections;
using UnityEngine;

using FMOD.Studio;
using FMODUnity;

public class KatalkPopUp : MonoBehaviour
{

    // ��ġ ����
    public float hiddenY = -200f;
    public float shownY = 100f;

    // �ִϸ��̼� ����
    public float slideDuration = 0.3f;

    // ���� ����
    private RectTransform rectTransform;
    private Coroutine moveCoroutine;
    private bool isShown = false;
    public bool IsShown =>isShown;
    [Header("FMOD")]
    public EventReference clickEvent;



    [Header("FMOD Parameter")]
    public string clickParamName = "Click Sounds";

    public string labelUnuse = "Unuse";

    // �ʱ�ȭ
    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, hiddenY);

        isShown = false;

    }

    // �׽�Ʈ�� �Է� (���߿� ���� ����)
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {

            Debug.Log("KatalkPopUp: Show called");
            Show();

        }

    }

    // �ܺ� ȣ���
    public void Show()
    {

        if (isShown)
        {

            return;

        }

        StartMove(hiddenY, shownY, true);

    }

    public void Hide()
    {

        if (!isShown)
        {

            return;

        }

        PlayClickOneShotWithLabel(labelUnuse);

        StartMove(shownY, hiddenY, false);

    }

    // ���� �̵� ó��
    private void StartMove(float fromY, float toY, bool setShownState)
    {

        if (moveCoroutine != null)
        {

            StopCoroutine(moveCoroutine);

        }

        moveCoroutine = StartCoroutine(Slide(fromY, toY, setShownState));

    }

    private IEnumerator Slide(float fromY, float toY, bool setShownState)
    {

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {

            elapsed += Time.deltaTime;

            float t = elapsed / slideDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, Mathf.Lerp(fromY, toY, t));

            yield return null;

        }

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, toY);

        isShown = setShownState;

    }

    private void PlayClickOneShotWithLabel(string label)
    {

        if (clickEvent.IsNull)
        {

            return;

        }

        EventInstance instance = RuntimeManager.CreateInstance(clickEvent);
        instance.setParameterByNameWithLabel(clickParamName, label);
        instance.start();
        instance.release();

    }

}
