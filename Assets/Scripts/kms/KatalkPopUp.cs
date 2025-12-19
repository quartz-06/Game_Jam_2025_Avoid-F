using System.Collections;
using UnityEngine;

public class KatalkPopUp : MonoBehaviour
{

    // 위치 설정
    public float hiddenY = -200f;
    public float shownY = 100f;

    // 애니메이션 설정
    public float slideDuration = 0.3f;

    // 내부 변수
    private RectTransform rectTransform;
    private Coroutine moveCoroutine;
    private bool isShown = false;

    // 초기화
    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, hiddenY);

        isShown = false;

    }

    // 테스트용 입력 (나중에 제거 예정)
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {

            Debug.Log("KatalkPopUp: Show called");
            Show();

        }

    }

    // 외부 호출용
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

        StartMove(shownY, hiddenY, false);

    }

    // 내부 이동 처리
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

}
