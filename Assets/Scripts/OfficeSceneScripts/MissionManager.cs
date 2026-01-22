using UnityEngine;
using TMPro;

public class MissionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mailPanel;
    public TextMeshProUGUI bodyText;
    public GameObject glitchButton;
    public Typewriter typewriter;
    public DayCycleManager dayManager;
    public MailWritingTask mailTask;


    public void OnClickWork()
    {
        Debug.Log("Button Clicked: Normal Work Path"); // button check
        //  ComplianceScore -= 1;
        dayManager.OnTaskFinished(); 
        CloseUI();
    }

    public void OnClickGlitch()
    {
        Debug.Log("Button Clicked: Glitch Path"); // button check
        // ComplianceScore += 1; 
        dayManager.OnTaskFinished(); 
        CloseUI();
    }

    private void CloseUI()
    {
        mailPanel.SetActive(false);
        // 
        Time.timeScale = 1f;
    }

    public void OpenUI(MissionData data)
    {
        mailPanel.SetActive(true);

        // Check if it's a mail mission
        if (data.type == MissionData.MissionType.Mail)
        {
            mailTask.StartMailMission(data.bodyText);
        }

        glitchButton.SetActive(data.isGlitchy);
        Time.timeScale = 0f;
    }

    public void CloseMailPanel()
    {
        mailPanel.SetActive(false); 
        Time.timeScale = 1f;
        typewriter.StopAllCoroutines();
    }
}