//using System;
//using JetBrains.Annotations;
//using NUnit.Framework;
//using UnityEngine;
//using UnityEngine.Events;
//using System.Collections;
//public class Task_Manager : MonoBehaviour
//{
//    public Bar_Manager Progress_UI;
//    public GameManager gameManager;
//    public float Current_percentage=0f; 
//    public float upPercentage=0.3f;
//    public GameObject pannel;
//    public GameObject gamePannel;
//    public float feverMultiply=1.25f;
//    public bool isFever=false;
//    public float feverBetweenTime=0.2f;
//    public float feverDuration=2f;
//    public float lastClickTime;
//    private int clickCount=0;
//    private bool Is_Finished=false;
//    // Start is called once before the first execution of Update after the MonoBehaviour is create
//    private void Awake()
//    {
//        Is_Finished=false;
//         if(pannel!=null)
//        {
//            pannel.gameObject.SetActive(false);
//        }
//    }
//    private void Start()
//    {
//    }
//    public void On_Click_Submit_Buttons()
//    {
//        if(Is_Finished)
//        {
//            return;
//        }
//        TrackFever();
//        Current_percentage+=upPercentage*(isFever?feverMultiply:1f);
//        Progress_UI.Update_Progress_Bar(Current_percentage);
//        if(Current_percentage>=100f)
//        {
//            Current_percentage=100f;
//            gamePannel.gameObject.SetActive(false);
//            Evaluate();
//        }
//    }
//    private void TrackFever()
//    {
//        if(isFever) return;
//        if(Time.time-lastClickTime>feverBetweenTime)
//        {
//            clickCount=0;
//        }
//        clickCount++;
//        lastClickTime=Time.time;
//        if(clickCount>=20)
//        {
//            StartCoroutine(FeverRoutin());
//        }
//    }
//    private IEnumerator FeverRoutin()
//    {
//        Debug.Log("Fever on");
//        isFever=true;
//        clickCount=0;
//        yield return new WaitForSeconds(feverDuration);
//        isFever=false;
//        Debug.Log("Fever off");
//    }
//    public void Evaluate()
//    {
//        gameManager.StopGame();
//        if(Current_percentage>=100f)
//        {
//            Current_percentage=100f;
//            gamePannel.gameObject.SetActive(false);
//            Finish_Game(0);
//        }
//        else
//        {
//            gamePannel.gameObject.SetActive(false);
//            Finish_Game(1);
//        }
//    }

//    private void Finish_Game(int iswin)
//    {
//        Is_Finished=true;
//        if(pannel!=null)
//        {
//            pannel.gameObject.SetActive(true);
//            pannel.GetComponent<Result_Controller>().Show(iswin);
//        }

//    }
//}

using UnityEngine;
using FMOD.Studio;
using FMODUnity;



public class Task_Manager : MonoBehaviour
{

    public Bar_Manager Progress_UI;
    public GameManager gameManager;

    public float Current_percentage = 0f;
    public float upPercentage = 0.3f;

    public GameObject pannel;
    public GameObject gamePannel;

    private bool Is_Finished = false;



    // ---------------------------
    // ▼ FMOD (Click One-Shot)
    // ---------------------------

    [Header("FMOD")]
    public EventReference clickEvent;

    [Header("FMOD Parameter")]
    public string clickParamName = "Click Sounds";          // FMOD 파라메터 이름(정확히 일치)

    public string labelUnuse = "Unuse";
    public string labelLow = "LowClicks";
    public string labelHigh = "HighClicks";

    [Header("State Threshold")]
    public float urgentThreshold = 50f;                     // 50% 기준



    private void Awake()
    {

        Is_Finished = false;

        if (pannel != null)
        {

            pannel.gameObject.SetActive(false);

        }

    }



    private void Start()
    {

    }



    public void On_Click_Submit_Buttons()
    {

        if (Is_Finished)
        {

            return;

        }

        // 1) 진행률 증가
        Current_percentage += upPercentage;
        Current_percentage = Mathf.Clamp(Current_percentage, 0f, 100f);

        // 2) UI 갱신
        if (Progress_UI != null)
        {

            Progress_UI.Update_Progress_Bar(Current_percentage);

        }

        // 3) 클릭 사운드 1회 재생 (현재 상태에 맞는 파라메터로)
        PlayClickOneShotByState();

        // 4) 완료 처리
        if (Current_percentage >= 100f)
        {

            Current_percentage = 100f;

            if (gamePannel != null)
            {

                gamePannel.gameObject.SetActive(false);

            }

            Evaluate();

        }

    }



    private void PlayClickOneShotByState()
    {

        if (clickEvent.IsNull)
        {

            return;

        }

        string label = (Current_percentage >= urgentThreshold) ? labelHigh : labelLow;

        EventInstance instance = RuntimeManager.CreateInstance(clickEvent);

        // 파라메터가 'Labeled Parameter'라면 이 방식이 가장 안전함
        // (FMOD Studio에서 Unuse/LowClicks/HighClicks 처럼 라벨로 구분되는 경우)
        instance.setParameterByNameWithLabel(clickParamName, label);

        instance.start();
        instance.release();

    }



    public void Evaluate()
    {

        if (gameManager != null)
        {

            gameManager.StopGame();

        }

        if (Current_percentage >= 100f)
        {

            Current_percentage = 100f;

            if (gamePannel != null)
            {

                gamePannel.gameObject.SetActive(false);

            }

            Finish_Game(0);

        }
        else
        {

            if (gamePannel != null)
            {

                gamePannel.gameObject.SetActive(false);

            }

            Finish_Game(1);

        }

    }



    private void Finish_Game(int iswin)
    {

        Is_Finished = true;

        // (선택) 게임 종료 상태에서 Unuse로 두고 싶으면 여기서 1회만 재생하거나,
        // 클릭 버튼을 비활성화하는 쪽이 더 깔끔함.
        // PlayClickOneShotWithLabel(labelUnuse);

        if (pannel != null)
        {

            pannel.gameObject.SetActive(true);

            Result_Controller result = pannel.GetComponent<Result_Controller>();
            if (result != null)
            {

                result.Show(iswin);

            }

        }

    }



    // 필요하면 외부에서 강제로 라벨 지정 재생할 수 있도록 남겨둔 함수
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
