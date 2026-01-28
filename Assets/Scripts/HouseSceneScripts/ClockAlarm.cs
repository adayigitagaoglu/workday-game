using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AlarmClock : MonoBehaviour
{
    [Header("Ses Ayarları")]
    public AudioSource alarmSource;

    [Header("Mahmurluk Ayarları")]
    public float slowSpeed = 1.5f;   
    public float normalSpeed = 5f;  
    public Volume globalVolume;      

    private bool isRinging = false;
    private bool isPlayerNearby = false;
    private PlayerMovement player;
    private DepthOfField dof;
    private Vector3 originalLocalPos;

    void Start()
    {
        originalLocalPos = transform.localPosition;
        player = FindFirstObjectByType<PlayerMovement>();

     
        if (globalVolume != null)
        {
            globalVolume.profile.TryGet<DepthOfField>(out dof);
        }

      
        if (DayCycleManager.tasksCompleted > 0)
        {
            Debug.Log("[ALARM] Akşam eve dönüldü, bulanıklık ve alarm iptal.");
        
            
            if (player != null) player.moveSpeed = normalSpeed;
        
           
            if (dof != null) dof.active = false; 

            this.enabled = false; 
            return; 
        }

      

        if (dof != null)
        {
            dof.active = true;
            dof.focusDistance.Override(0.1f); 
        }

        if (player != null) player.moveSpeed = slowSpeed;

        Invoke("StartRinging", 5f);
    }

    void StartRinging()
    {
        isRinging = true;
        if (alarmSource != null) alarmSource.Play();
        transform.DOShakePosition(100f, 0.07f, 15).SetLoops(-1, LoopType.Restart).SetUpdate(true);
    }

    void Update()
    {
        if (isRinging && isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            StopAlarm();
        }
    }

    private void OnMouseDown()
    {
        if (isRinging) StopAlarm();
    }

    void StopAlarm()
    {
        isRinging = false;
        if (alarmSource != null) alarmSource.Stop();
        
        transform.DOKill(); 
        transform.localPosition = originalLocalPos; 

        if (player != null) player.moveSpeed = normalSpeed;

        if (dof != null)
        {
            DOTween.To(() => dof.focusDistance.value, x => dof.focusDistance.value = x, 10f, 1.5f)
                .SetUpdate(true)
                .OnComplete(() => dof.active = false);
        }
        Debug.Log("[ALARM] Sabah mahmurluğu bitti.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isRinging) isPlayerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = false;
    }
}