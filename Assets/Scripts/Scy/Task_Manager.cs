using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
public class Task_Manager : MonoBehaviour
{
    public Bar_Manager Progress_UI;
    public float Current_percentage=0f; 
    public float upPercentage=0.3f;
    public GameObject pannel;
    private bool Is_Finished=false;
    // Start is called once before the first execution of Update after the MonoBehaviour is create
    private void Start()
    {
        if(pannel!=null)
        {
            pannel.gameObject.SetActive(false);
        }
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
