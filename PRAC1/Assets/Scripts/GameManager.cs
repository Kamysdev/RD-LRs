using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Round")]
    [SerializeField] private float roundTime = 60f;
    [SerializeField] private int totalItems = 10;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text restartHintText;

    [Header("References")]
    [SerializeField] private CatPlayerController playerController;

    private int score;
    private float timeRemaining;
    private bool isGameOver;

    public bool IsGameOver => isGameOver;

    private void Awake()
    {
        timeRemaining = roundTime;

        if (playerController == null)
        {
            playerController = FindFirstObjectByType<CatPlayerController>();
        }

        if (totalItems <= 0)
        {
            totalItems = FindObjectsByType<DroppableItem>(FindObjectsSortMode.None).Length;
        }

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        if (restartHintText != null)
        {
            restartHintText.gameObject.SetActive(false);
        }

        RefreshUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
            return;
        }

        if (isGameOver)
        {
            return;
        }

        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0f)
        {
            timeRemaining = 0f;
        }

        RefreshUI();

        if (timeRemaining <= 0f)
        {
            EndGame($"Time is over! Score: {score}");
        }
    }

    public void AddScore()
    {
        if (isGameOver)
        {
            return;
        }

        score++;
        RefreshUI();

        if (score >= totalItems)
        {
            EndGame($"All items dropped! Score: {score}");
        }
    }

    public void EndGame(string message)
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        timeRemaining = Mathf.Max(0f, timeRemaining);

        if (playerController != null)
        {
            playerController.SetControlsLocked(true);
        }

        if (messageText != null)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
        }

        if (restartHintText != null)
        {
            restartHintText.text = "Press R to restart";
            restartHintText.gameObject.SetActive(true);
        }

        RefreshUI();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void RefreshUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}/{totalItems}";
        }

        if (timerText != null)
        {
            timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}";
        }
    }
}
