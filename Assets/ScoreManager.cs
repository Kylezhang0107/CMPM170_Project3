using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text statusText;
    [SerializeField] private int startingHealth = 10;

    private int score;
    private int health;
    private int currentLevel = 1;
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Time.timeScale = 1f;
        health = startingHealth;
    }

    private void Start()
    {
        if (scoreText == null || statusText == null)
        {
            Debug.LogError("ScoreManager needs score and status text references.", this);
            return;
        }

        UpdateScoreText();
        UpdateStatusText();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
        UpdateStatusText();
    }

    public void TakeDamage(int amount)
    {
        if (IsGameOver || amount <= 0)
        {
            return;
        }

        health = Mathf.Max(0, health - amount);
        if (health == 0)
        {
            IsGameOver = true;
            statusText.text = "GAME OVER\nScore: " + score;
            Time.timeScale = 0f;
            return;
        }

        UpdateStatusText();
    }

    public void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    public void FinishGame()
    {
        IsGameOver = true;
        statusText.text = "GARDEN SAVED!\nScore: " + score;
        Time.timeScale = 0f;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void UpdateStatusText()
    {
        if (statusText != null)
        {
            statusText.text = "Level: " + currentLevel + "\nHealth: " + health;
        }
    }
}
