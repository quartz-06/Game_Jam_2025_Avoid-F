using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FMOD.Studio;
using FMODUnity;



public class GameManager : MonoBehaviour
{

    public GameTimer timer;

    public KatalkPopUp[] katalkPopUps;
    public Task_Manager taskManager;

    public AdPopUp[] adPopUps;

    public GameObject startPanel;

    public GameObject hud;



    [Header("FMOD - HeartBeat")]
    public EventReference heartBeatEvent;



    [Header("FMOD - BGM")]
    public EventReference bgmEvent;



    [Header("FMOD - Parameter")]
    public string heartRateParameterName = "HeartRate";



    [Header("HeartRate Range")]
    [Range(60f, 160f)]
    public float heartRateMin = 60f;

    [Range(60f, 160f)]
    public float heartRateMax = 160f;



    [Header("HeartRate Smoothing")]
    public float heartRateSmoothSpeed = 5f;

    private float currentHeartRate = 60f;



    private bool isPlaying = false;

    private Coroutine katalkCoroutine;

    private List<Coroutine> adCoroutines = new List<Coroutine>();


    // 현재 광고창 위치 중복 방지용 점유 리스트
    private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>();



    // FMOD
    private EventInstance heartBeatInstance;
    private EventInstance bgmInstance;

    private bool isHeartBeatPlaying = false;
    private bool isBgmPlaying = false;



    private void Start()
    {

        if (startPanel != null)
        {

            startPanel.SetActive(true);

        }

        if (hud != null)
        {

            hud.SetActive(false);

        }

        if (timer != null)
        {

            timer.ResetTimer();

        }

        isPlaying = false;

        StopHeartBeat();
        StopBgm();

    }



    private void Update()
    {

        if (!isPlaying)
        {

            return;

        }

        UpdateHeartRateParameter();

        if (timer != null && !timer.IsRunning && timer.RemainingSeconds <= 0f)
        {

            if (taskManager != null)
            {

                taskManager.Evaluate();

            }

        }

    }



    public void StartGame()
    {

        if (isPlaying)
        {

            return;

        }

        isPlaying = true;

        if (startPanel != null)
        {

            startPanel.SetActive(false);

        }

        if (hud != null)
        {

            hud.SetActive(true);

        }

        if (timer != null)
        {

            timer.StartTimer(timer.startSeconds);

        }

        katalkCoroutine = StartCoroutine(KatalkPopupLoop());

        StartAdPopupsIndependently();

        StartHeartBeat();
        StartBgm();

    }



    public void StopGame()
    {

        if (!isPlaying)
        {

            return;

        }

        isPlaying = false;

        if (katalkCoroutine != null)
        {

            StopCoroutine(katalkCoroutine);
            katalkCoroutine = null;

        }

        for (int i = 0; i < adCoroutines.Count; i++)
        {

            StopCoroutine(adCoroutines[i]);

        }

        adCoroutines.Clear();

        occupiedSpawnPoints.Clear();

        HideAllAds();
        HideAllKatalks();

        if (timer != null)
        {

            timer.StopTimer();

        }

        StopHeartBeat();
        StopBgm();

    }



    private void StartAdPopupsIndependently()
    {

        if (adPopUps == null)
        {

            return;

        }

        for (int i = 0; i < adPopUps.Length; i++)
        {

            if (adPopUps[i] == null)
            {

                continue;

            }

            Coroutine coroutine = StartCoroutine(AdPopupLoop(adPopUps[i]));
            adCoroutines.Add(coroutine);

        }

    }



    public int GetActivePopCount()
    {

        int count = 0;

        if (adPopUps != null)
        {

            foreach (var ad in adPopUps)
            {

                if (ad != null && ad.IsShown)
                {

                    count++;

                }

            }

        }

        if (katalkPopUps != null)
        {

            foreach (var k in katalkPopUps)
            {

                if (k != null && k.IsShown)
                {

                    count++;

                }

            }

        }

        return count;

    }



    private IEnumerator AdPopupLoop(AdPopUp ad)
    {

        while (isPlaying)
        {

            float waitTime = Random.Range(3f, 13f);
            yield return new WaitForSeconds(waitTime);

            if (ad.IsShown)
            {

                continue;

            }

            Transform spawnPoint = RequestSpawnPoint(ad.spawnPoints);

            if (spawnPoint != null)
            {

                ad.ShowAt(spawnPoint);

            }

        }

    }



    // 광고 스폰 포인트 하나 요청
    private Transform RequestSpawnPoint(Transform[] candidates)
    {

        if (candidates == null || candidates.Length <= 0)
        {

            return null;

        }

        List<Transform> available = new List<Transform>();

        for (int i = 0; i < candidates.Length; i++)
        {

            if (candidates[i] != null && !occupiedSpawnPoints.Contains(candidates[i]))
            {

                available.Add(candidates[i]);

            }

        }

        if (available.Count <= 0)
        {

            return null;

        }

        Transform selected = available[Random.Range(0, available.Count)];
        occupiedSpawnPoints.Add(selected);

        return selected;

    }



    // 광고 스폰 포인트 반환
    public void ReleaseSpawnPoint(Transform point)
    {

        if (point != null)
        {

            occupiedSpawnPoints.Remove(point);

        }

    }



    private void HideAllAds()
    {

        if (adPopUps == null)
        {

            return;

        }

        for (int i = 0; i < adPopUps.Length; i++)
        {

            if (adPopUps[i] != null)
            {

                adPopUps[i].Hide();

            }

        }

    }



    private IEnumerator KatalkPopupLoop()
    {

        while (isPlaying)
        {

            float waitTime = Random.Range(3f, 7f);
            yield return new WaitForSeconds(waitTime);

            KatalkPopUp selected = PickRandomHiddenKatalk();

            if (selected != null)
            {

                selected.Show();

            }

        }

    }



    private KatalkPopUp PickRandomHiddenKatalk()
    {

        if (katalkPopUps == null || katalkPopUps.Length <= 0)
        {

            return null;

        }

        List<KatalkPopUp> candidates = new List<KatalkPopUp>();

        for (int i = 0; i < katalkPopUps.Length; i++)
        {

            KatalkPopUp k = katalkPopUps[i];

            if (k == null)
            {

                continue;

            }

            if (!k.IsShown)
            {

                candidates.Add(k);

            }

        }

        if (candidates.Count <= 0)
        {

            return null;

        }

        return candidates[Random.Range(0, candidates.Count)];

    }



    private void HideAllKatalks()
    {

        if (katalkPopUps == null)
        {

            return;

        }

        for (int i = 0; i < katalkPopUps.Length; i++)
        {

            if (katalkPopUps[i] != null)
            {

                katalkPopUps[i].Hide();

            }

        }

    }



    // ---------------------------
    // ▼ FMOD HeartRate 공유 업데이트
    // ---------------------------

    private void UpdateHeartRateParameter()
    {

        float targetHR = CalculateHeartRate();

        currentHeartRate = Mathf.Lerp(
            currentHeartRate,
            targetHR,
            Time.deltaTime * heartRateSmoothSpeed
        );

        if (isHeartBeatPlaying)
        {

            heartBeatInstance.setParameterByName(heartRateParameterName, currentHeartRate);

        }

        if (isBgmPlaying)
        {

            bgmInstance.setParameterByName(heartRateParameterName, currentHeartRate);

        }

    }



    private void StartHeartBeat()
    {

        if (isHeartBeatPlaying)
        {

            return;

        }

        if (heartBeatEvent.IsNull)
        {

            Debug.LogWarning("[GameManager] heartBeatEvent is not assigned.");
            return;

        }

        heartBeatInstance = RuntimeManager.CreateInstance(heartBeatEvent);

        float initialHR = CalculateHeartRate();
        currentHeartRate = initialHR;

        heartBeatInstance.setParameterByName(heartRateParameterName, currentHeartRate);

        heartBeatInstance.start();
        isHeartBeatPlaying = true;

    }



    private void StopHeartBeat()
    {

        if (!isHeartBeatPlaying)
        {

            return;

        }

        heartBeatInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        heartBeatInstance.release();

        isHeartBeatPlaying = false;

    }



    private void StartBgm()
    {

        if (isBgmPlaying)
        {

            return;

        }

        if (bgmEvent.IsNull)
        {

            Debug.LogWarning("[GameManager] bgmEvent is not assigned.");
            return;

        }

        bgmInstance = RuntimeManager.CreateInstance(bgmEvent);

        float initialHR = CalculateHeartRate();
        currentHeartRate = initialHR;

        bgmInstance.setParameterByName(heartRateParameterName, currentHeartRate);

        bgmInstance.start();
        isBgmPlaying = true;

    }



    private void StopBgm()
    {

        if (!isBgmPlaying)
        {

            return;

        }

        bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        bgmInstance.release();

        isBgmPlaying = false;

    }



    private float CalculateHeartRate()
    {

        float p = 0f;

        if (taskManager != null)
        {

            p = Mathf.Clamp(taskManager.Current_percentage, 0f, 100f);

        }

        float t = p / 100f;

        float d = 0f;

        if (taskManager != null && taskManager.isDistraction)
        {

            d = 1f;

        }

        float hr =
            heartRateMin
            + 80f * Mathf.Pow(t, 1.5f)
            + 20f * d * t;

        hr = Mathf.Clamp(hr, heartRateMin, heartRateMax);

        return hr;

    }



    public Transform RequestSpawnPointForExternal(Transform[] candidates)
    {

        return RequestSpawnPoint(candidates);

    }



    private void OnDisable()
    {

        StopHeartBeat();
        StopBgm();

    }



    private void OnDestroy()
    {

        StopHeartBeat();
        StopBgm();

    }

}
