using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ReportCorrectionTask : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text instructionText;
    public List<ReportErrorItem> allErrors; 

    [Header("Settings")]
    public float gameDuration = 15f; 
    private float currentTime;
    private bool isGameActive = false;
    private bool isSabotageActive = false;
    private int fixedCount = 0;
    private MissionManagerNew _manager;

    public void StartGame(MissionManagerNew manager, bool glitch)
    {
        _manager = manager;
        currentTime = gameDuration;
        fixedCount = 0;
        isGameActive = true;

       
        isSabotageActive = DayCycleManager.currentDay >= 2 && !DayCycleManager.isSystemAbandoned;

        if (isSabotageActive)
        {
            instructionText.text = "<color=white> RAPORU DÜZELTME. HATALI BIRAK.</color>";
            Debug.Log("[REPORT TASK] Sabotaj aktif.");
        }
        else if (DayCycleManager.isSystemAbandoned)
        {
          
            instructionText.text = "Lütfen işinizi her zamanki gibi yapın.";
            Debug.Log("[REPORT TASK] Sistem vazgeçti, mesaj yok.");
        }
        else
        {
            instructionText.text = "Kırmızı hataları tıklayarak yeşil yapınız";
        }

        if (allErrors != null)
        {
            foreach (var error in allErrors)
            {
                if(error != null) error.Setup(this);
            }
        }
    }

    void Update()
    {
        if (isGameActive)
        {
            currentTime -= Time.unscaledDeltaTime;
            if (timerText != null) timerText.text = "Time: " + Mathf.CeilToInt(currentTime);

            if (currentTime <= 0) EndTask();
        }
    }

    public void RegisterFix()
    {
        fixedCount++;
    }

    public void OnSubmitPressed()
    {
        EndTask();
    }

    private void EndTask()
    {
        if (!isGameActive) return;
        isGameActive = false;

        int finalScore = 0;
        int totalPossible = allErrors.Count;
        
        int errorsLeft = totalPossible - fixedCount;

        if (isSabotageActive)
        {
            
            if (errorsLeft >= 2)
            {
                finalScore = 1;
                Debug.Log($"<color=cyan>[REPORT TASK]</color> Sabotaj Başarılı: {errorsLeft} hata bilerek bırakıldı.");
            }
            else
            {
                finalScore = 0;
                Debug.Log($"<color=white>[REPORT TASK]</color> Sabotaj Başarısız: Sadece {errorsLeft} hata kaldı, yeterli değil.");
            }
        }
        else
        {
            
            finalScore = 0;
        }

        if (_manager != null) _manager.OnTaskCompleted(finalScore);
    }
}