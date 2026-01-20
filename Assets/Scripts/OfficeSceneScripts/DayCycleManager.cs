using UnityEngine;
using System.Collections;
using TMPro;

public class DayCycleManager : MonoBehaviour
{
    public int currentDay = 1;
    public int tasksCompleted = 0;

    [Header("UI References")]
    public GameObject dayOverPanel;
    public TextMeshProUGUI dayText;

    public void OnTaskFinished()
    {
        tasksCompleted++;

        if (tasksCompleted >= 2)
        {
            StartCoroutine(TransitionToNextDay());
        }
    }

    IEnumerator TransitionToNextDay()
    {
        dayOverPanel.SetActive(true);
        dayText.text = "DAY " + currentDay + " COMPLETED.";

        yield return new WaitForSecondsRealtime(3f);

        currentDay++;
        tasksCompleted = 0;

        GameObject.FindGameObjectWithTag("Player").transform.position = Vector3.zero;

        dayOverPanel.SetActive(false);
        StartNewDay();
    }

    public void StartNewDay()
    {
        Debug.Log("Starting Day " + currentDay);
    }
}