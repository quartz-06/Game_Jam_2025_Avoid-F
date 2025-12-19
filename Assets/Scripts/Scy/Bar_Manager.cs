using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;
using Unity.Android.Gradle.Manifest;
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
    private void Awake()
    {
        if(Bar_transform!=null)
        {
            Origin_scale=Bar_transform.localScale;
            Origin_ancpos=Bar_transform.anchoredPosition;
        }
    }
    public void Update_Progress_Bar(float Current_percentage)
    {
        if(Progress_bar!=null)
        {
            Progress_bar.value=Current_percentage/100f  ;
        }
        if(Scale_corutin!=null)
        {
            StopCoroutine(Scale_corutin);
        }
        Scale_corutin=StartCoroutine(Play_Clicked_Animation());
        
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
