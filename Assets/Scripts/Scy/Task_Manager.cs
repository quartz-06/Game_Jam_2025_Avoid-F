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

//    private IEnumerator FeverRoutin()
//    {
//        Debug.Log("Fever on");
//        isFever=true;
//        clickCount=0;
//        yield return new WaitForSeconds(feverDuration);
//        isFever=false;
//        Debug.Log("Fever off");
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
using System.Collections;
using NUnit.Framework;
using Unity.AppUI.UI;
using System;
using Unity.AppUI.Core;

public class Task_Manager : MonoBehaviour
{

    public Bar_Manager Progress_UI;
    public GameManager gameManager;

    public float Current_percentage = 0f;
    public float upPercentage = 0.3f;

    public GameObject pannel;
    public GameObject gamePannel;

    private bool Is_Finished = false;
    public int currentState=0;
    public float pastPercentage=-1f;
    [Header("fever")]

    public float feverBetweenTime=0.2f;
    public float feverMultiply=1.25f;
    public bool isFever=false;
   public float feverDuration=2f;
    public float lastClickTime;
    private int clickCount=0;
    private int totalClickCount=0;
    [Header("distraction")]
    public int distractionCount=4;
    public float warringDuration=2f;
    public float decresePercent=2f;
    public float warringTimer=0f;
    private bool isWrong=false;
    public bool isDistraction=false;


    [Header("FMOD")]
    public EventReference clickEvent;

    [Header("FMOD Parameter")]
    public string clickParamName = "Click Sounds";          // FMOD �Ķ���� �̸�(��Ȯ�� ��ġ)

    public string labelUnuse = "Unuse";
    public string labelLow = "LowClicks";
    public string labelHigh = "HighClicks";

    [Header("State Threshold")]
    public float urgentThreshold = 50f;                     // 50% ����


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

    private void Update()
    {
        if(Is_Finished||gameManager==null)
        return;
        CheckDistractionState();
        DistractionHandler();
        if (totalClickCount > 0 && Time.time - lastClickTime > feverBetweenTime)
        {
            totalClickCount = 0;
            clickCount = 0;
            Progress_UI.UpdateCombo(0);
        }
    }
    public void CheckDistractionState()
    {
        int activePopup=gameManager.GetActivePopCount();
        if(activePopup>=distractionCount)
        {
            if(!isDistraction)
            {
                warringTimer+=Time.deltaTime;
                if(warringTimer>=warringDuration)
                {
                    isDistraction=true;
                    isWrong=false;
                }
                else
                {
                    isDistraction=false;
                    isWrong=true;
                }
            }
        }
        else
        {
            isDistraction=false;
            isWrong=false;
            warringTimer=0f;
        }
    }
    public void DistractionHandler()
    {
        if(isDistraction)
        {
            float startPercentage=Current_percentage;
            Current_percentage-=decresePercent*Time.deltaTime;
            Current_percentage=Mathf.Clamp(Current_percentage,0f,100f);
             Progress_UI.Update_Progress_Bar(Current_percentage,isDistraction? 3: isWrong ? 2: isFever? 1:0,true);

        }
        else
        {
             Progress_UI.Update_Progress_Bar(Current_percentage,isDistraction? 3: isWrong ? 2: isFever? 1:0,false);

        }
    }
    public void On_Click_Submit_Buttons()
    {

        if (Is_Finished)
        {

            return;

        }
        TrackFever();
        // 1) ����� ����
        Current_percentage += upPercentage*(isDistraction? 1f: Current_percentage>=80? 1.1f: isFever? feverMultiply:1f);
        Current_percentage = Mathf.Clamp(Current_percentage, 0f, 100f);

        // 2) UI ����
        if (Progress_UI != null)
        {
            Progress_UI.Update_Progress_Bar(Current_percentage,isDistraction? 3: isWrong ? 2: isFever? 1:0,true);
        }
        // 3) Ŭ�� ���� 1ȸ ��� (���� ���¿� �´� �Ķ���ͷ�)
        PlayClickOneShotByState();

        // 4) �Ϸ� ó��
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


        private void TrackFever()
    {
        if(isFever)
        {
            if(Time.time-lastClickTime>feverBetweenTime)
             {
                clickCount=0;
                totalClickCount=0;
            }
                totalClickCount++;
                lastClickTime=Time.time;
                Progress_UI.UpdateCombo(totalClickCount);
        }
        else
        {
            
            if(Time.time-lastClickTime>feverBetweenTime)
            {
                clickCount=0;
                totalClickCount=0;
            }
            clickCount++;
            totalClickCount++;
            lastClickTime=Time.time;
            Progress_UI.UpdateCombo(totalClickCount);
            if(clickCount>=20)
            {
                StartCoroutine(FeverRoutin());
            }
        }
    }
    private IEnumerator FeverRoutin()
    {
        Debug.Log("Fever on");
        isFever=true;
        clickCount=0;
        yield return new WaitForSeconds(feverDuration);
        isFever=false;
        Debug.Log("Fever off");
   }
    private void PlayClickOneShotByState()
    {

        if (clickEvent.IsNull)
        {   
            return;

        }

        string label = (Current_percentage >= urgentThreshold) ? labelHigh : labelLow;

        EventInstance instance = RuntimeManager.CreateInstance(clickEvent);

        // �Ķ���Ͱ� 'Labeled Parameter'��� �� ����� ���� ������
        // (FMOD Studio���� Unuse/LowClicks/HighClicks ó�� �󺧷� ���еǴ� ���)
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

    private void IsDistraction()
    {
        
    }

    private void Finish_Game(int iswin)
    {

        Is_Finished = true;

        // (����) ���� ���� ���¿��� Unuse�� �ΰ� ������ ���⼭ 1ȸ�� ����ϰų�,
        // Ŭ�� ��ư�� ��Ȱ��ȭ�ϴ� ���� �� �����.
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
