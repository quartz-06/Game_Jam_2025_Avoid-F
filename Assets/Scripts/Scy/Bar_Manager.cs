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
    public Text valueText;
    private float pastPercentage=-1f;
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
                        valueText.text="정신이 산만해졌습니다.진행도가 깍입니다.";
                        valueText.color=Color.red;
                        valueText.fontStyle=FontStyle.Bold;
                        return;
                    default:
                        break;
                }
            }
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
