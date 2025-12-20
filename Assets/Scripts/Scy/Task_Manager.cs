using System;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
public class Task_Manager : MonoBehaviour
{
    public Bar_Manager Progress_UI;
    public GameManager gameManager;
    public float Current_percentage=0f; 
    public float upPercentage=0.3f;
    public GameObject pannel;
    public GameObject gamePannel;
    public float feverMultiply=1.25f;
    public bool isFever=false;
    public float feverBetweenTime=0.2f;
    public float feverDuration=2f;
    public float lastClickTime;
    private int clickCount=0;
    private bool Is_Finished=false;
    // Start is called once before the first execution of Update after the MonoBehaviour is create
    private void Awake()
    {
        Is_Finished=false;
         if(pannel!=null)
        {
            pannel.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
    }
    public void On_Click_Submit_Buttons()
    {
        if(Is_Finished)
        {
            return;
        }
        TrackFever();
        Current_percentage+=upPercentage*(isFever?feverMultiply:1f);
        Progress_UI.Update_Progress_Bar(Current_percentage);
        if(Current_percentage>=100f)
        {
            Current_percentage=100f;
            gamePannel.gameObject.SetActive(false);
            Evaluate();
        }
    }
    private void TrackFever()
    {
        if(isFever) return;
        if(Time.time-lastClickTime>feverBetweenTime)
        {
            clickCount=0;
        }
        clickCount++;
        lastClickTime=Time.time;
        if(clickCount>=20)
        {
            StartCoroutine(FeverRoutin());
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
    public void Evaluate()
    {
        gameManager.StopGame();
        if(Current_percentage>=100f)
        {
            Current_percentage=100f;
            gamePannel.gameObject.SetActive(false);
            Finish_Game(0);
        }
        else
        {
            gamePannel.gameObject.SetActive(false);
            Finish_Game(1);
        }
    }

    private void Finish_Game(int iswin)
    {
        Is_Finished=true;
        if(pannel!=null)
        {
            pannel.gameObject.SetActive(true);
            pannel.GetComponent<Result_Controller>().Show(iswin);
        }
        
    }
}
