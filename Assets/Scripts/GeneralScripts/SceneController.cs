using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.Cinemachine; 

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [Header("Player Settings")]
    public GameObject playerPrefab;
    [Header("Fader Settings")]
    public SpriteRenderer faderSprite;
    public float fadeDuration = 1f;

    [Header("Portal Settings")]
    public bool isPortal = false;
    public string targetSceneName;

    private static string lastSceneName;
    private bool isPlayerNearby = false;

    private void Awake()
    {
        if (isPortal) return;

        if (Instance == null)
        {
            Instance = this;
           
            transform.SetParent(null); 
            DontDestroyOnLoad(gameObject);
            
            if (faderSprite == null) faderSprite = GetComponent<SpriteRenderer>();
            Debug.Log("<color=green>[SCENE CONTROLLER]</color> Ana Instance kuruldu.");
        }
        else if (Instance != this)
        {
           
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (isPortal || Instance != this) return;

        BindToCamera();
        HandlePlayerSpawn();

        if (faderSprite != null)
        {
            faderSprite.DOKill();
            faderSprite.color = new Color(0, 0, 0, 1);
            faderSprite.DOFade(0f, fadeDuration).SetUpdate(true);
        }
    }

    public void FadeAndLoad(string sceneName)
    {
      
        if (Instance != this) return;

        if (faderSprite == null) faderSprite = GetComponent<SpriteRenderer>();

      
        lastSceneName = SceneManager.GetActiveScene().name;
        
        faderSprite.DOKill();
        faderSprite.sortingOrder = 999;
        faderSprite.gameObject.SetActive(true);

        faderSprite.DOFade(1f, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    private void OnEnable() { if (!isPortal) SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { if (!isPortal) SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance != this || isPortal) return;

        BindToCamera();
        HandlePlayerSpawn();
        
        if (faderSprite != null)
        {
            faderSprite.DOKill();
            faderSprite.sortingOrder = 999;
            faderSprite.DOFade(0f, fadeDuration).SetUpdate(true);
        }
    }

    public void HandlePlayerSpawn()
    {
       
        string currentScene = SceneManager.GetActiveScene().name;
        GameObject spawnPoint = null;

        if (currentScene == "Scene_Home" && lastSceneName == "Scene_Home")
        {
            spawnPoint = GameObject.Find("SpawnPoint_Default");
        }
        else
        {
            string targetSpawnName = "Spawn_from_" + lastSceneName;
            spawnPoint = GameObject.Find(targetSpawnName);
        }

        
        if (spawnPoint == null) spawnPoint = GameObject.Find("SpawnPoint_Default");
        if (spawnPoint == null) spawnPoint = GameObject.FindWithTag("Respawn");

        if (spawnPoint == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            player = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);
            DontDestroyOnLoad(player);
        }
        else
        {
            player.transform.position = spawnPoint.transform.position;
            if (player.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        UpdateCameraFollow(player.transform);
    }

    private void UpdateCameraFollow(Transform target)
    {
        var vcam = FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null) vcam.Follow = target;
    }

    private void BindToCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            transform.SetParent(mainCam.transform);
            transform.localPosition = new Vector3(0, 0, 1); 
            if(faderSprite != null) faderSprite.sortingOrder = 999;
        }
    }

    private void Update()
    {
        if (isPortal && isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (Instance != null)
            {
                string nextScene = targetSceneName;

               
                if (DayCycleManager.currentDay == 7 && targetSceneName == "Scene_Office")
                {
                    
                    if (DayCycleManager.isSystemAbandoned)
                    {
                        nextScene = "Scene_Final_Loop";
                        Debug.Log("<color=orange>[FINAL]</color> Denek başarısız oldu. Sonsuz döngü başlıyor.");
                    }
                    else
                    {
                        nextScene = "Scene_SalvationEnding";
                        Debug.Log("<color=cyan>[FINAL]</color> Denek sistemi bozdu. Kurtuluş yolu açıldı.");
                    }
                }

                Instance.FadeAndLoad(nextScene);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) { if (isPortal && other.CompareTag("Player")) isPlayerNearby = true; }
    private void OnTriggerExit2D(Collider2D other) { if (isPortal && other.CompareTag("Player")) isPlayerNearby = false; }
}