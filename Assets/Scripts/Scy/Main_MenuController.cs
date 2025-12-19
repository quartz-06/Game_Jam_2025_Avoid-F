using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
public class Main_MenuController : MonoBehaviour
{
    [SerializeField] private Camera Main_camera;
    [SerializeField] private RectTransform Computer_button;

    [SerializeField] private float Zoom_time=30f;

    [SerializeField] private float Target_orthsize=2f; 

    [SerializeField] private GameObject Block_panel;
    [SerializeField] private string Next_scene;
    private bool Is_Zoom=false;
    private float Origin_fov;
    private Vector3 Origin_pos;
    void Awake()
    {
        if(Main_camera!=null)
        {
            Origin_fov=Main_camera.fieldOfView;
            Origin_pos=Main_camera.transform.position;
        }
        if(Block_panel!=null)
        {
            Block_panel.SetActive(false);
        }
    }

    public void On_Clicked_Computer()
    {
        if(!Is_Zoom)
        {
            StartCoroutine(Camera_Corutin());
        }
    }
    public IEnumerator Camera_Corutin()
    {
        Is_Zoom=true;
        if(Block_panel!=null)
            {
                Block_panel.SetActive(false);
            }
        float speeded_time=0f;
        Vector3 Start_pos=Main_camera.transform.position;
        float Start_size=Main_camera.orthographicSize;
        Vector3 Target_pos=new Vector3(Computer_button.position.x,Computer_button.position.y,Start_pos.z);
        while(speeded_time<Zoom_time)
        {
            speeded_time+=Time.deltaTime;
            float T=speeded_time/Zoom_time;
            float SmoothT=Mathf.SmoothStep(0f,1f,T);
            Main_camera.orthographicSize=Mathf.Lerp(Start_size,Target_orthsize,T);
            Main_camera.transform.position=Vector3.Lerp(Start_pos,Target_pos,SmoothT);
            yield return null;
        }
        Main_camera.orthographicSize=Target_orthsize;
        Main_camera.transform.position=Target_pos;
        yield return new WaitForSeconds(0.2f);
        if(!string.IsNullOrEmpty(Next_scene))
        {
            SceneManager.LoadScene(Next_scene);
        }
    }

}
