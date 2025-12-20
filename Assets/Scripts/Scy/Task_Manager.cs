using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
public class Task_Manager : MonoBehaviour
{
    public Bar_Manager Progress_UI;
    public float Current_percentage=0f; 
    public float upPercentage=0.3f;
    public GameObject pannel;
    public GameObject gamePannel;
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
        Current_percentage+=upPercentage;
        Progress_UI.Update_Progress_Bar(Current_percentage);
        if(Current_percentage>=100f)
        {
            Current_percentage=100f;
            gamePannel.gameObject.SetActive(false);
            Finish_Game(0);
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
