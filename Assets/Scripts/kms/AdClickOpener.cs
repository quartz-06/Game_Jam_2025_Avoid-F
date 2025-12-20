using UnityEngine;
using UnityEngine.EventSystems;



public class AdClickOpener : MonoBehaviour, IPointerClickHandler
{

    [Header("References")]
    public AdPopUp adPopUp;

    public GameManager gameManager;



    [Header("Scene Popup (Existing Object)")]
    public GameObject scenePopupObject;



    [Header("Optional")]
    public bool hideAdAfterOpen = false;



    private void Awake()
    {

        if (adPopUp == null)
        {

            adPopUp = GetComponentInParent<AdPopUp>();

        }

        if (gameManager == null)
        {

            gameManager = FindFirstObjectByType<GameManager>();

        }

    }



    public void OnPointerClick(PointerEventData eventData)
    {

        if (adPopUp != null && !adPopUp.IsShown)
        {

            return;

        }

        if (scenePopupObject == null)
        {

            Debug.LogWarning("[AdClickOpener] scenePopupObject is NULL");
            return;

        }

        if (scenePopupObject.activeSelf)
        {

            return;

        }

        scenePopupObject.SetActive(true);

        scenePopupObject.transform.SetAsLastSibling();



        if (hideAdAfterOpen && adPopUp != null)
        {

            adPopUp.Hide();

        }

    }

}
