using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AdPopUp : MonoBehaviour
{

    // 스폰 포인트 부모 (AdSpawnPoints)
    public Transform spawnPointsRoot;



    public float fadeInDuration = 0.2f;
    public float fadeOutDuration = 0.5f;



    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private bool isShown;
    public bool IsShown => isShown;



    // GameManager가 읽는 실제 스폰 포인트 목록
    [HideInInspector]
    public Transform[] spawnPoints;



    private Transform currentSpawnPoint;



    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {

            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        }

        canvasGroup.alpha = 0f;
        isShown = false;

        RefreshSpawnPoints();

    }



#if UNITY_EDITOR
    private void OnValidate()
    {

        RefreshSpawnPoints();

    }
#endif



    // spawnPointsRoot 아래 자식들을 자동 수집
    private void RefreshSpawnPoints()
    {

        if (spawnPointsRoot == null)
        {

            spawnPoints = null;
            return;

        }

        List<Transform> list = new List<Transform>();

        for (int i = 0; i < spawnPointsRoot.childCount; i++)
        {

            Transform child = spawnPointsRoot.GetChild(i);

            if (child == null)
            {

                continue;

            }

            list.Add(child);

        }

        spawnPoints = list.ToArray();

    }



    public void ShowAt(Transform spawnPoint)
    {

        if (isShown || spawnPoint == null)
        {

            return;

        }

        currentSpawnPoint = spawnPoint;

        MoveToPoint(spawnPoint);

        if (fadeCoroutine != null)
        {

            StopCoroutine(fadeCoroutine);

        }

        fadeCoroutine = StartCoroutine(FadeIn());

    }



    public void Hide()
    {

        if (!isShown)
        {

            return;

        }

        if (fadeCoroutine != null)
        {

            StopCoroutine(fadeCoroutine);

        }

        fadeCoroutine = StartCoroutine(FadeOut());

    }

    public void FakeHide()
    {

        Debug.Log("AdPopUp: FakeHide called");

    }

    private IEnumerator FadeIn()
    {

        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {

            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);

            yield return null;

        }

        canvasGroup.alpha = 1f;
        isShown = true;
        fadeCoroutine = null;

    }



    private IEnumerator FadeOut()
    {

        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < fadeOutDuration)
        {

            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);

            yield return null;

        }

        canvasGroup.alpha = 0f;
        isShown = false;

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {

            gm.ReleaseSpawnPoint(currentSpawnPoint);

        }

        currentSpawnPoint = null;
        fadeCoroutine = null;

    }



    private void MoveToPoint(Transform point)
    {

        if (rectTransform != null)
        {

            rectTransform.anchoredPosition = point.localPosition;

        }
        else
        {

            transform.localPosition = point.localPosition;

        }

    }

}
