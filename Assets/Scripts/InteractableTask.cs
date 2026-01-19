using UnityEngine;

public class InteractableTask : MonoBehaviour
{
    public GameObject missionIcon;
    public GameObject mailPanel;  
    public string taskName = "";

    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Starting " + taskName);
            mailPanel.SetActive(true); 
            missionIcon.SetActive(false); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            missionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            missionIcon.SetActive(false); 
        }
    }
}