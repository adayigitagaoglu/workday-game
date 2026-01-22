using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.Cinemachine; 

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [Header("Player Ayarları")]
    public GameObject playerPrefab; 

    [Header("Fader Ayarları ")]
    public SpriteRenderer faderSprite;
    public float fadeDuration = 1f;

    [Header("Portal Ayarları ")]
    public bool isPortal = false; 
    public string targetSceneName; 
    public Collider2D portalTrigger;

    private bool isPlayerNearby = false;
    private static string lastSceneName; 

    private void Awake()
    {
        if (!isPortal)
        {
            if (Instance == null)
            {
                Instance = this;
                if (transform.parent != null) transform.SetParent(null); 
                DontDestroyOnLoad(gameObject);
                BindToCamera();
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private void Start()
    {
        if (!isPortal && faderSprite != null)
        {
            faderSprite.color = new Color(0, 0, 0, 1);
            faderSprite.DOFade(0f, fadeDuration).SetUpdate(true);
        }

        if (!isPortal) HandlePlayerSpawn();
    }

    private void Update()
    {
        if (isPortal && isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (Instance != null)
            {
                Instance.FadeAndLoad(targetSceneName);
            }
        }
    }

    public void FadeAndLoad(string sceneName)
    {
        if (faderSprite == null) return;

        lastSceneName = SceneManager.GetActiveScene().name;

        faderSprite.DOKill(); 
        faderSprite.DOFade(1f, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    private void OnEnable() { if (!isPortal) SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { if (!isPortal) SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isPortal)
        {
            BindToCamera();
            HandlePlayerSpawn();
            if (faderSprite != null)
            {
                faderSprite.DOKill();
                faderSprite.DOFade(0f, fadeDuration).SetUpdate(true);
            }
        }
    }

    private void HandlePlayerSpawn()
    {
     
        string targetSpawnName = "Spawn_from_" + lastSceneName;
        GameObject spawnPoint = GameObject.Find(targetSpawnName);

        
        if (spawnPoint == null)
        {
            spawnPoint = GameObject.Find("SpawnPoint_Default");
        }

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
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        UpdateCameraFollow(player.transform);
    }

    private void UpdateCameraFollow(Transform target)
    {
        CinemachineCamera vcam = FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            vcam.Follow = target;
        }
    }

    private void BindToCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null && !isPortal)
        {
            transform.SetParent(mainCam.transform);
            transform.localPosition = new Vector3(0, 0, 1); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPortal && other.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isPortal && other.CompareTag("Player")) isPlayerNearby = false;
    }
}