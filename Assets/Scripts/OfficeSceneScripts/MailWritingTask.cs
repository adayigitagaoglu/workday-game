using UnityEngine;
using TMPro;
using System.Collections;

public class MailWritingTask : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI typingArea;
    public TextMeshProUGUI timerText;

    [Header("Settings")]
    public float timeLimit = 30f;
    private string fullTargetText;
    private int charIndex = 0;
    private float timeLeft;
    private bool isGameActive = false;

    public void StartMailMission(string content)
    {
        fullTargetText = content;
        charIndex = 0;
        typingArea.text = ""; 
        timeLeft = timeLimit;
        isGameActive = true;
        timerText.color = Color.white;
    }

    void Update()
    {
        if (!isGameActive) return;

        timeLeft -= Time.unscaledDeltaTime;
        timerText.text = "TIME: " + Mathf.Ceil(timeLeft).ToString();

        if (timeLeft <= 0) EndMission();
        if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
        {
            if (charIndex < fullTargetText.Length)
            {
                typingArea.text += fullTargetText[charIndex];
                charIndex++;
            }
        }
    }

    void EndMission()
    {
        isGameActive = false;
        timerText.text = "TIME'S UP!";
        timerText.color = Color.red;
    }
}