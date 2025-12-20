
//using UnityEngine;

//using FMOD.Studio;
//using FMODUnity;



//public class AudioController : MonoBehaviour
//{

//    [Header("References")]
//    public Task_Manager taskManager;



//    [Header("FMOD - Events")]
//    public EventReference heartBeatEvent;
//    public EventReference bgmEvent;



//    [Header("FMOD - Parameter")]
//    public string heartRateParameterName = "HeartRate";



//    [Header("HeartRate Range")]
//    [Range(60f, 160f)]
//    public float heartRateMin = 60f;

//    [Range(60f, 160f)]
//    public float heartRateMax = 160f;



//    [Header("HeartRate Smoothing")]
//    public float heartRateSmoothSpeed = 5f;



//    private EventInstance heartBeatInstance;
//    private EventInstance bgmInstance;

//    private bool isHeartBeatPlaying = false;
//    private bool isBgmPlaying = false;

//    private float currentHeartRate = 60f;



//    private void Update()
//    {

//        if (!isHeartBeatPlaying && !isBgmPlaying)
//        {

//            return;

//        }

//        UpdateHeartRateParameter();

//    }



//    public void OnGameStart()
//    {

//        StartHeartBeat();

//        StartBgm();

//    }



//    public void OnGameStop()
//    {

//        StopHeartBeat();

//        StopBgm();

//    }



//    private void StartHeartBeat()
//    {

//        if (isHeartBeatPlaying)
//        {

//            return;

//        }

//        if (heartBeatEvent.IsNull)
//        {

//            Debug.LogWarning("[AudioController] heartBeatEvent is not assigned.");
//            return;

//        }

//        heartBeatInstance = RuntimeManager.CreateInstance(heartBeatEvent);

//        float initialHR = CalculateHeartRate();
//        currentHeartRate = initialHR;

//        heartBeatInstance.setParameterByName(heartRateParameterName, currentHeartRate);

//        heartBeatInstance.start();
//        isHeartBeatPlaying = true;

//    }



//    private void StopHeartBeat()
//    {

//        if (!isHeartBeatPlaying)
//        {

//            return;

//        }

//        heartBeatInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
//        heartBeatInstance.release();

//        isHeartBeatPlaying = false;

//    }



//    private void StartBgm()
//    {

//        if (isBgmPlaying)
//        {

//            return;

//        }

//        if (bgmEvent.IsNull)
//        {

//            Debug.LogWarning("[AudioController] bgmEvent is not assigned.");
//            return;

//        }

//        bgmInstance = RuntimeManager.CreateInstance(bgmEvent);

//        float initialHR = CalculateHeartRate();
//        currentHeartRate = initialHR;

//        bgmInstance.setParameterByName(heartRateParameterName, currentHeartRate);

//        bgmInstance.start();
//        isBgmPlaying = true;

//    }



//    private void StopBgm()
//    {

//        if (!isBgmPlaying)
//        {

//            return;

//        }

//        bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
//        bgmInstance.release();

//        isBgmPlaying = false;

//    }



//    private void UpdateHeartRateParameter()
//    {

//        float targetHR = CalculateHeartRate();

//        currentHeartRate = Mathf.Lerp(
//            currentHeartRate,
//            targetHR,
//            Time.deltaTime * heartRateSmoothSpeed
//        );

//        if (isHeartBeatPlaying)
//        {

//            heartBeatInstance.setParameterByName(heartRateParameterName, currentHeartRate);

//        }

//        if (isBgmPlaying)
//        {

//            bgmInstance.setParameterByName(heartRateParameterName, currentHeartRate);

//        }

//    }



//    // HR = 60 + 80 * (p/100)^1.5 + 20 * d * (p/100)
//    // p : Current_percentage (0~100)
//    // d : isDistraction -> true = 1, false = 0
//    // 결과 : 60~160 사이 정수(실제로는 float로 보내도 FMOD에서 잘 처리됨)
//    private float CalculateHeartRate()
//    {

//        float p = 0f;

//        if (taskManager != null)
//        {

//            p = Mathf.Clamp(taskManager.Current_percentage, 0f, 100f);

//        }

//        float t = p / 100f;

//        float d = 0f;

//        if (taskManager != null && taskManager.isDistraction)
//        {

//            d = 1f;

//        }

//        float hr =
//            heartRateMin
//            + 80f * Mathf.Pow(t, 1.5f)
//            + 20f * d * t;

//        hr = Mathf.Clamp(hr, heartRateMin, heartRateMax);

//        return hr;

//    }



//    private void OnDisable()
//    {

//        OnGameStop();

//    }



//    private void OnDestroy()
//    {

//        OnGameStop();

//    }

//}

