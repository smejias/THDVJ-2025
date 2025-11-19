using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    [SerializeField] private Transform spawnPoint;

    private bool isPaused = false;


    private Vector3 initialSpawnPosition;
    private Quaternion initialSpawnRotation;

    [SerializeField] private PlayerInput playerInput;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (spawnPoint != null)
        {
            initialSpawnPosition = spawnPoint.position;
            initialSpawnRotation = spawnPoint.rotation;
            Debug.Log($"[GameManager] Spawn point set to: {initialSpawnPosition}");
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                initialSpawnPosition = player.transform.position;
                initialSpawnRotation = player.transform.rotation;
                Debug.Log($"[GameManager] Using player's initial position: {initialSpawnPosition}");
            }
            else
            {
                Debug.LogWarning("[GameManager] No spawn point and no player found! Using world origin.");
                initialSpawnPosition = Vector3.zero;
                initialSpawnRotation = Quaternion.identity;
            }
        }
    }

    public void OnRespawn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("[GameManager] F1 pressed - Respawning player...");
            RespawnPlayer();
        }
    }

    public void OnRestart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("[GameManager] F2 pressed - Restarting scene...");
            RestartScene();
        }
    }

    private void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[GameManager] Player not found! Make sure it has tag 'Player'");
            return;
        }

        CharacterController characterController = player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        player.transform.position = initialSpawnPosition;
        player.transform.rotation = initialSpawnRotation;

        if (characterController != null)
        {
            characterController.enabled = true;
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        HealthComponent health = player.GetComponentInChildren<HealthComponent>();
        if (health != null)
        {
            health.ResetToFullHealth();
            Debug.Log("[GameManager] Health reset");
        }

        GunController gun = player.GetComponentInChildren<GunController>();
        if (gun != null)
        {
            gun.ResetAmmo();
            Debug.Log("[GameManager] Ammo reset");
        }

        player.SetActive(false);
        player.SetActive(true);

        Debug.Log($"[GameManager] Player respawned successfully!");
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        TogglePause();
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;

            
            playerInput.SwitchCurrentActionMap("Pause");

            Debug.Log("Juego pausado");
        }
        else
        {
            Time.timeScale = 1f;

       
            playerInput.SwitchCurrentActionMap("Gameplay");

            Debug.Log("Juego reanudado");
        }
    }


}
