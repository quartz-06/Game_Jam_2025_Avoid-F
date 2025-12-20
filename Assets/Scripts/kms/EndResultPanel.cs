using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using FMODUnity;



public class EndResultPanel : MonoBehaviour
{

    [Header("Root")]
    public GameObject rootPanel;



    [Header("UI - Icon")]
    public RectTransform checkIconRect;



    [Header("UI - Text")]
    public Text titleText;
    public Text dateTimeText;

    public Text detailTitleText;
    public Text detailText;



    [Header("Animation - Pop")]
    public float checkPopDuration = 0.25f;
    public float checkPopOvershootScale = 1.08f;



    [Header("Animation - Fade")]
    public CanvasGroup titleCanvasGroup;
    public CanvasGroup detailCanvasGroup;

    public float titleFadeDuration = 0.25f;
    public float detailFadeDuration = 0.25f;



    [Header("Animation - Typing")]
    public float typingCharInterval = 0.015f;
    public float typingLinePause = 0.08f;



    //[Header("FMOD - SFX (Optional)")]
    //public EventReference submitSuccessSfx;



    private Coroutine showCoroutine;



    private void Awake()
    {

        if (rootPanel != null)
        {

            rootPanel.SetActive(false);

        }



        EnsureCanvasGroup(ref titleCanvasGroup, titleText);
        EnsureCanvasGroup(ref detailCanvasGroup, detailText);

    }



    public void ShowResult(GameResultData resultData)
    {

        if (showCoroutine != null)
        {

            StopCoroutine(showCoroutine);
            showCoroutine = null;

        }



        showCoroutine = StartCoroutine(CoShowSequence(resultData));

    }



    private IEnumerator CoShowSequence(GameResultData resultData)
    {

        rootPanel.SetActive(true);



        // ---------------------------
        // 0) 초기 상태 세팅
        // ---------------------------

        SetTexts(resultData);

        PrepareInitialVisualState();



        // ---------------------------
        // 1) 성공 효과음
        // ---------------------------

        // PlaySuccessSfx();



        // ---------------------------
        // 2) 체크 아이콘 팝
        // ---------------------------

        yield return StartCoroutine(CoPop(checkIconRect, checkPopDuration, checkPopOvershootScale));



        // ---------------------------
        // 3) 타이틀 페이드 인
        // ---------------------------

        yield return StartCoroutine(CoFadeCanvasGroup(titleCanvasGroup, 0f, 1f, titleFadeDuration));



        // ---------------------------
        // 4) 디테일 영역 페이드 인
        // ---------------------------

        yield return StartCoroutine(CoFadeCanvasGroup(detailCanvasGroup, 0f, 1f, detailFadeDuration));



        // ---------------------------
        // 5) 디테일 타이핑(한 줄씩)
        // ---------------------------

        yield return StartCoroutine(CoTypeLines(detailText));

    }



    private void SetTexts(GameResultData resultData)
    {

        titleText.text = "제출됨!";



        DateTime now = DateTime.Now;
        dateTimeText.text = $"{now.Month}월 {now.Day}일 {GetKoreanAmPm(now)} {now:hh:mm}";



        detailTitleText.text = "제출물 세부정보";



        detailText.text =
            $"클릭 횟수: {resultData.clickCount}\n" +
            $"점수: {resultData.score}\n" +
            $"플레이 시간: {resultData.playTime:F1}초";

    }



    private void PrepareInitialVisualState()
    {

        // 체크 아이콘: 0스케일로 숨김
        if (checkIconRect != null)
        {

            checkIconRect.localScale = Vector3.zero;

        }



        // 타이틀/날짜: 바로 보이게 할지, 타이틀만 페이드할지 선택 가능
        // 여기서는 "제출됨!" 텍스트(타이틀)만 페이드 대상으로 잡았고,
        // dateTimeText는 즉시 표시(원하면 dateTimeText도 CanvasGroup으로 묶어 같이 페이드 가능)

        if (titleCanvasGroup != null)
        {

            titleCanvasGroup.alpha = 0f;

        }



        // 디테일 텍스트는 페이드 + 타이핑이므로, alpha 0 + 내용 비워두기
        if (detailCanvasGroup != null)
        {

            detailCanvasGroup.alpha = 0f;

        }



        // 타이핑용으로 현재 텍스트를 저장해두고, 보여지는 건 비워둠
        // CoTypeLines에서 원문을 다시 읽기 때문에 여기서 비워도 됨
        // (단, CoTypeLines가 text.text를 원문으로 쓰기 때문에, 원문을 별도 캐시로 가져간다)

    }



    //private void PlaySuccessSfx()
    //{

    //    if (submitSuccessSfx.IsNull)
    //    {

    //        return;

    //    }



    //    RuntimeManager.PlayOneShot(submitSuccessSfx);

    //}



    private IEnumerator CoPop(RectTransform target, float duration, float overshootScale)
    {

        if (target == null)
        {

            yield break;

        }



        float t = 0f;

        Vector3 start = Vector3.zero;
        Vector3 mid = Vector3.one * overshootScale;
        Vector3 end = Vector3.one;



        // 1) 0 -> overshoot
        while (t < duration)
        {

            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);

            float eased = EaseOutBack(p);

            target.localScale = Vector3.LerpUnclamped(start, mid, eased);

            yield return null;

        }



        // 2) overshoot -> 1
        t = 0f;
        float settleDuration = duration * 0.45f;

        while (t < settleDuration)
        {

            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / settleDuration);

            float eased = EaseOutCubic(p);

            target.localScale = Vector3.LerpUnclamped(mid, end, eased);

            yield return null;

        }



        target.localScale = end;

    }



    private IEnumerator CoFadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {

        if (cg == null)
        {

            yield break;

        }



        float t = 0f;
        cg.alpha = from;

        while (t < duration)
        {

            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);

            float eased = EaseOutCubic(p);

            cg.alpha = Mathf.Lerp(from, to, eased);

            yield return null;

        }



        cg.alpha = to;

    }



    private IEnumerator CoTypeLines(Text targetText)
    {

        if (targetText == null)
        {

            yield break;

        }



        string full = targetText.text;

        string[] lines = full.Split('\n');



        targetText.text = "";


        for (int i = 0; i < lines.Length; i++)
        {

            string line = lines[i];

            for (int c = 0; c < line.Length; c++)
            {

                targetText.text += line[c];
                yield return new WaitForSecondsRealtime(typingCharInterval);

            }



            if (i < lines.Length - 1)
            {

                targetText.text += "\n";
                yield return new WaitForSecondsRealtime(typingLinePause);

            }

        }

    }



    private void EnsureCanvasGroup(ref CanvasGroup canvasGroup, Component target)
    {

        if (target == null)
        {

            return;

        }



        if (canvasGroup != null)
        {

            return;

        }



        canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {

            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();

        }

    }



    private string GetKoreanAmPm(DateTime time)
    {

        return time.Hour < 12 ? "오전" : "오후";

    }



    // ---------------------------
    // Easing
    // ---------------------------

    private float EaseOutCubic(float t)
    {

        t = Mathf.Clamp01(t);
        return 1f - Mathf.Pow(1f - t, 3f);

    }



    private float EaseOutBack(float t)
    {

        t = Mathf.Clamp01(t);

        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);

    }

}
