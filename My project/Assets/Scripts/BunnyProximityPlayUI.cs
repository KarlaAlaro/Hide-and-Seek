using UnityEngine;
using UnityEngine.UI;

public class BunnyProximityPlayUI : MonoBehaviour
{
    public Transform player;
    public GameObject uiPanel;
    public Button playButton;
    public ProjectileLauncher projectileLauncher;
    public BunnyRunToGameSpot bunnyRunner;

    public float showDistance = 3f;
    public bool facePlayer = true;
    public bool waitForBunnyArrival = true;

    private bool minigameStarted;

    void Awake()
    {
        if (projectileLauncher != null)
        {
            projectileLauncher.enabled = false;
        }

        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        if (playButton != null)
        {
            playButton.onClick.AddListener(StartMinigame);
        }
    }

    void Update()
    {
        if (player == null || uiPanel == null || minigameStarted)
        {
            return;
        }

        if (waitForBunnyArrival && bunnyRunner != null && !bunnyRunner.HasArrived)
        {
            uiPanel.SetActive(false);
            return;
        }

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        bool playerIsClose = distanceToPlayer <= showDistance;

        uiPanel.SetActive(playerIsClose);

        if (playerIsClose && facePlayer)
        {
            Vector3 directionToPlayer = uiPanel.transform.position - player.position;
            directionToPlayer.y = 0f;

            if (directionToPlayer != Vector3.zero)
            {
                uiPanel.transform.rotation = Quaternion.LookRotation(directionToPlayer);
            }
        }
    }

    public void StartMinigame()
    {
        if (minigameStarted)
        {
            return;
        }

        minigameStarted = true;

        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        if (projectileLauncher != null)
        {
            projectileLauncher.enabled = true;
        }
    }
}
