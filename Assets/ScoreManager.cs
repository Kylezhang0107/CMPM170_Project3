using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    private int score = 0;

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score.ToString();
    }

}
