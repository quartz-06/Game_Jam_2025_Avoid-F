using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Data;
using System;
public class Result_Controller : MonoBehaviour
{
    public Text Intro_Text;
    public Text Result_Text;
    public GameObject Restart_buttons;
    public Text restartText;
    public GameObject scorePanel;
    public Text nowTime;
    public Text Score;
    public string Intro_full_text="당신은 ";
    public string successText="F학점에서 \n벗어났다!!";
    public string failText="F학점 확정!!";
    public string failRestartText="재수강하러 가기";
    public string successRestartText="다른 과목 과제\n하러 가기";
    public float Wait_timer=3f;
    public float Type_timer=1f;
    public float Size_multiply=2f;
    public float Animation_time=10f;
    public string Restart;
    public void Show(int iswin,float lasttime,int total,int failcount)
    {   

        StopAllCoroutines();
        StartCoroutine(Result_Sequence(iswin,lasttime,total,failcount));
    }
    private void Reset_UI(int iswin)
    {  Intro_Text.text="";
        Intro_Text.gameObject.SetActive(true);
        Intro_Text.transform.localScale=Vector3.one;
        if(iswin==0)
        {
            Result_Text.text=successText;
            restartText.text=successRestartText;
        }  
        else
        {
            Result_Text.text=failText;
            restartText.text=failRestartText;
        }
            
        Result_Text.gameObject.SetActive(false);
        Result_Text.transform.localScale=Vector3.zero;
        Restart_buttons.gameObject.SetActive(false);  
        scorePanel.gameObject.SetActive(false);    
    }
    private IEnumerator Result_Sequence(int iswin,float lasttime,int total,int failcount)
    {
        Reset_UI(iswin);
        foreach(char Letter in Intro_full_text.ToCharArray())
        {
            Intro_Text.text+=Letter;
            yield return new WaitForSeconds(Type_timer);
        }
        yield return new WaitForSeconds(Wait_timer);
        Result_Text.gameObject.SetActive(true);
        yield return StartCoroutine(PopEffect(Result_Text.rectTransform,Size_multiply));
        yield return new WaitForSeconds(Wait_timer);
        Restart_buttons.SetActive(true);
        if(iswin==0)
        {
            scorePanel.gameObject.SetActive(true);
            nowTime.text=DateTime.Now.ToString("MM")+"월 "+DateTime.Now.ToString("dd")+"일 "+DateTime.Now.ToString("tt")+" "+DateTime.Now.ToString("HH")+"시"+DateTime.Now.ToString("mm")+"분";
            Score.text=$"총합 점수: {(lasttime*1000)+total*10-failcount*100}\n남은시간: {lasttime}\n최대연타수: {total}\n처리실패횟수: {failcount}"; 
        }
        
    }
    private IEnumerator PopEffect(RectTransform Rect,float Target_scale)
    {
        float spped=0f;
        while(spped<Animation_time)
        {
            spped+=Time.deltaTime;
            float T=spped/Animation_time;
            float Current_scale;
            if(T<0.5f)
            {
             Current_scale=Mathf.Lerp(0f,Target_scale,2f*T);
            }
            else
            {
                Current_scale=Mathf.Lerp(Target_scale,1f,(T-0.5f)*2f);
            }
            Rect.localScale=new Vector3(Current_scale,Current_scale,1f);
            yield return null;
        }
        Rect.transform.localScale=Vector3.one;
    
    }
     public void OnClickRestart()
    {
        SceneManager.LoadScene(Restart);
    }
}
