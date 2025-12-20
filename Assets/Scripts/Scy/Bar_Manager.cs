using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;
using NUnit.Framework;

public class Bar_Manager : MonoBehaviour
{
    public Slider Progress_bar;
    public RectTransform Bar_transform;
    public float Scale_multyply=1.1f;
    public float Animation_time=0.5f;
    public float Shake_power=5.0f;
    public float Shake_speed=10f;
    private Vector3 Origin_scale;
    private Vector2 Origin_ancpos;
    private Coroutine Scale_corutin;
    private float currentpercentage;
    private Coroutine comboCorutin;
    public Text comboText;
    public Text valueText;
    public Text percentageWord;
    private float pastPercentage=-1f;
    private string[]  s1={"초안을 작성 중입니다.","초안을 작성 중입니다..","초안을 작성 중입니다..."};
    private string[]  s2={"과제를 완성 중입니다.","과제를 완성 중입니다..","과제를 완성 중입니다..."};
    private string[]  s3={"컴파일 중입니다.","컴파일 중입니다..","컴파일 중입니다..."};
    private string[]  s4={"제출 양식을 확인 중입니다.","제출 양식을 확인 중입니다..","제출 양식을 확인 중입니다..."};
    private string[]  s5={"이 과제 제출은 내 자신의 독창적인 작업입니다.","이 과제 제출은 내 자신의 독창적인 작업입니다..","이 과제 제출은 내 자신의 독창적인 작업입니다..."};
    private Coroutine peedbackCorutin;
    private void Awake()
    {
        if(Bar_transform!=null)
        {
            Origin_scale=Bar_transform.localScale;
            Origin_ancpos=Bar_transform.anchoredPosition;
        }
    }
    public void Update_Progress_Bar(float Current_percentage,int currentState,bool playAnimation=false)
    {
        currentpercentage=Current_percentage;
        if(Progress_bar!=null)
        {
            Progress_bar.value=Current_percentage/100f  ;
            if(valueText!=null)
            {
                switch(currentState)
                {
                    case 0:
                        valueText.text=$"{Progress_bar.value:P2}";
                        valueText.color=Color.black;
                        valueText.fontStyle=FontStyle.Normal;
                        break;
                    case 1:
                        valueText.text="Fever Time!!";
                        valueText.color=Color.yellow;
                        valueText.fontStyle=FontStyle.Bold;
                        break;
                    case 2:
                        valueText.text="정신이 산만해지고 있습니다.";
                        valueText.color=Color.orange;
                        break;
                    case 3:
                        valueText.text="정신이 산만해졌습니다.\n진행도가 깍입니다.";
                        valueText.color=Color.red;
                        valueText.fontStyle=FontStyle.Bold;
                        return;
                    default:
                        break;
                }
            }
        }
        if (percentageWord != null && peedbackCorutin == null)
            {
                // 변수에 할당하여 중복 실행 방지
                peedbackCorutin = StartCoroutine(TextCycle());
            }
        bool hasChanged=Mathf.Approximately(pastPercentage,Current_percentage);
        if (playAnimation&&!hasChanged)
        {
            if(Scale_corutin!=null)
            {
                StopCoroutine(Scale_corutin);
                Reset_Transform();
            }
            Scale_corutin=StartCoroutine(Play_Clicked_Animation());
        }
        pastPercentage=Current_percentage;
        
    }
    public string[] SelectWords(float Current_percentage)
    {
        if(Current_percentage<=50f)return s1;
        else if(Current_percentage<=70f)return s2;
        else if(Current_percentage<=80f)return s3;
        else if(Current_percentage<=90f)return s4;
        else return s5;
    }
    private IEnumerator TextCycle()
    {
        int stringIndex=0;
        while(true)
        {
            string[] e=SelectWords(currentpercentage);
            stringIndex%=3;
            percentageWord.text=e[stringIndex];
            stringIndex+=1;
            yield return new WaitForSeconds(1.5f);
        }
    }
    public void UpdateCombo(int clickcount)
    {
        comboText.text=$"{clickcount}";
        if(clickcount%20==0)
        {
            if(comboCorutin==null)
            {
                comboCorutin=StartCoroutine(PopWord());
            }
        }
    }
    private IEnumerator PopWord()
    {
        float ex_time=0f;
        Vector3 initScale=Vector3.one;
        Vector3 TargetSCale=Vector3.one*2f;
        while(ex_time<2f)
        {
            ex_time+=Time.deltaTime;
            float T=ex_time/2f;
            if(T<=0.5f)
            {
                comboText.transform.localScale=Vector3.Lerp(initScale,TargetSCale,T*2f);
            }
            else
            {
                comboText.transform.localScale=Vector3.Lerp(TargetSCale,initScale,(T-0.5F)*2f);   
            }
            yield return null;
        }
        comboText.transform.localScale=Vector3.one;
        comboCorutin=null;
    }
    private void Reset_Transform()
    {
        Bar_transform.localScale=Origin_scale;
        Bar_transform.anchoredPosition=Origin_ancpos;
    }
    private IEnumerator Play_Clicked_Animation()
    {
        if(Bar_transform==null) yield break;
        float speed=0f;
        while(speed<Animation_time)

        {
            speed+=Time.deltaTime;
            float T=speed/Animation_time;
            Bar_transform.localScale=Vector3.Lerp(Origin_scale*Scale_multyply,Origin_scale,T);
            float currentShake=Shake_power*(1-T);
            Vector2 Shake=UnityEngine.Random.insideUnitCircle*currentShake;
            Bar_transform.anchoredPosition=Origin_ancpos+Shake;
            yield return null;
        }
        Reset_Transform();
    }

}
