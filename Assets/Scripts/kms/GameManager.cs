    using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{

    public GameTimer timer;

    public KatalkPopUp katalkPopUp;

    public AdPopUp[] adPopUps;

    public GameObject startPanel;

    public GameObject hud;

    private bool isPlaying = false;

    private Coroutine katalkCoroutine;

    private List<Coroutine> adCoroutines = new List<Coroutine>();



    // ���� ��� ���� ���� ���� ����Ʈ
    private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>();



    private void Start()
    {

        if (startPanel != null)
        {

            startPanel.SetActive(true);

        }

        if (hud != null)
        {

            hud.SetActive(false);

        }

        if (timer != null)
        {

            timer.ResetTimer();

        }

        isPlaying = false;

    }



    private void Update()
    {

        if (!isPlaying)
        {

            return;

        }

        if (timer != null && !timer.IsRunning && timer.RemainingSeconds <= 0f)
        {

            StopGame();

        }

    }



    public void StartGame()
    {

        if (isPlaying)
        {

            return;

        }

        isPlaying = true;

        if (startPanel != null)
        {

            startPanel.SetActive(false);
            
        }
        if (hud != null)
        {

            hud.SetActive(true);

        }

        if (timer != null)
        {

            timer.StartTimer(timer.startSeconds);

        }

        katalkCoroutine = StartCoroutine(KatalkPopupLoop());

        StartAdPopupsIndependently();

    }



    public void StopGame()
    {

        if (!isPlaying)
        {

            return;

        }

        isPlaying = false;

        if (katalkCoroutine != null)
        {

            StopCoroutine(katalkCoroutine);
            katalkCoroutine = null;

        }

        for (int i = 0; i < adCoroutines.Count; i++)
        {

            StopCoroutine(adCoroutines[i]);

        }

        adCoroutines.Clear();

        occupiedSpawnPoints.Clear();

        HideAllAds();

        if (timer != null)
        {

            timer.StopTimer();

        }

    }



    private void StartAdPopupsIndependently()
    {

        if (adPopUps == null)
        {

            return;

        }

        for (int i = 0; i < adPopUps.Length; i++)
        {

            if (adPopUps[i] == null)
            {

                continue;

            }

            Coroutine coroutine = StartCoroutine(AdPopupLoop(adPopUps[i]));
            adCoroutines.Add(coroutine);

        }

    }

    private IEnumerator AdPopupLoop(AdPopUp ad)
    {

        while (isPlaying)
        {

            float waitTime = Random.Range(1f, 4f);
            yield return new WaitForSeconds(waitTime);

            if (ad.IsShown)
            {

                continue;

            }

            Transform spawnPoint = RequestSpawnPoint(ad.spawnPoints);

            if (spawnPoint != null)
            {

                ad.ShowAt(spawnPoint);

            }

        }

    }



    // ��� ������ ���� ����Ʈ �ϳ� ��û
    private Transform RequestSpawnPoint(Transform[] candidates)
    {

        if (candidates == null || candidates.Length <= 0)
        {

            return null;

        }

        List<Transform> available = new List<Transform>();

        for (int i = 0; i < candidates.Length; i++)
        {

            if (candidates[i] != null && !occupiedSpawnPoints.Contains(candidates[i]))
            {

                available.Add(candidates[i]);

            }

        }

        if (available.Count <= 0)
        {

            return null;

        }

        Transform selected = available[Random.Range(0, available.Count)];
        occupiedSpawnPoints.Add(selected);

        return selected;

    }



    // ���� ����Ʈ ��ȯ
    public void ReleaseSpawnPoint(Transform point)
    {

        if (point != null)
        {

            occupiedSpawnPoints.Remove(point);

        }

    }



    private void HideAllAds()
    {

        if (adPopUps == null)
        {

            return;

        }

        for (int i = 0; i < adPopUps.Length; i++)
        {

            if (adPopUps[i] != null)
            {

                adPopUps[i].Hide();

            }

        }

    }



    private IEnumerator KatalkPopupLoop()
    {

        while (isPlaying)
        {

            float waitTime = Random.Range(3f, 7f);
            yield return new WaitForSeconds(waitTime);

            if (katalkPopUp != null)
            {

                katalkPopUp.Show();

            }

        }

    }

}
