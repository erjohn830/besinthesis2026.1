using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Level")]
    public int currentLevel = 1;
    public int maxLevel = 3;

    [Header("Score")]
    public int score = 0;
    public int pointsPerJump = 100;

    [Header("UI Text")]
    public TMP_Text scoreText;
    public TMP_Text levelText;

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("References")]
    public TayaLevelController tayaController;
    public LuksongBakaTimer gameTimer;

    private bool gameEnded = false;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        maxLevel = 3;

        currentLevel = 1;
        score = 0;
        gameEnded = false;

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        UpdateHUD();
        ApplyCurrentLevel();
    }

    // =========================================================
    // UPDATE HUD
    // =========================================================

    public void UpdateHUD()
    {
        if (scoreText != null)
        {
            scoreText.text =
                "Score: " + score;
        }

        if (levelText != null)
        {
            levelText.text =
                "Level " + currentLevel;
        }
    }

    // =========================================================
    // SCORE
    // =========================================================

    public void AddScore(int amount)
    {
        if (gameEnded)
        {
            return;
        }

        score += amount;

        UpdateHUD();

        Debug.Log(
            "SCORE = " + score
        );
    }

    // =========================================================
    // NEXT LEVEL
    // =========================================================

    public void NextLevel()
    {
        if (gameEnded)
        {
            return;
        }

        if (currentLevel >= maxLevel)
        {
            return;
        }

        currentLevel++;

        currentLevel =
            Mathf.Clamp(
                currentLevel,
                1,
                3
            );

        Debug.Log(
            "CHANGING TO LEVEL "
            + currentLevel
        );

        UpdateHUD();
        ApplyCurrentLevel();
    }

    // =========================================================
    // APPLY TAYA LEVEL
    // =========================================================

    public void ApplyCurrentLevel()
    {
        currentLevel =
            Mathf.Clamp(
                currentLevel,
                1,
                3
            );

        if (tayaController == null)
        {
            Debug.LogError(
                "TAYA CONTROLLER IS NOT ASSIGNED!"
            );

            return;
        }

        tayaController.PlayLevel(
            currentLevel
        );
    }

    // =========================================================
    // TIMER
    // =========================================================

    public void PauseGameTimer()
    {
        if (gameTimer != null)
        {
            gameTimer.StopTimer();
        }
    }

    // =========================================================
    // WIN
    // =========================================================

    public void WinGame()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;

        PauseGameTimer();

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        Debug.Log(
            "YOU WIN! FINAL SCORE = "
            + score
        );
    }

    // =========================================================
    // LOSE
    // =========================================================

    public void LoseGame()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;

        PauseGameTimer();

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        Debug.Log(
            "GAME OVER! LOST AT LEVEL "
            + currentLevel
            + " | SCORE = "
            + score
        );
    }

    // =========================================================
    // NORMAL TRY AGAIN
    // =========================================================

    public void ResetGame()
    {
        gameEnded = false;

        currentLevel = 1;
        score = 0;

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        UpdateHUD();
        ApplyCurrentLevel();

        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
        }

        Debug.Log(
            "GAME RESET TO LEVEL 1"
        );
    }

    // =========================================================
    // REWARDED AD CONTINUE
    // =========================================================

    public void ReviveFromRewardedAd(
        int levelToRestore,
        int scoreToRestore)
    {
        gameEnded = false;

        currentLevel =
            Mathf.Clamp(
                levelToRestore,
                1,
                3
            );

        score =
            Mathf.Max(
                0,
                scoreToRestore
            );

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        UpdateHUD();
        ApplyCurrentLevel();

        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
        }

        Debug.Log(
            "REWARDED AD REVIVE"
            + " | LEVEL = "
            + currentLevel
            + " | SCORE = "
            + score
        );
    }

    // =========================================================
    // GAME ENDED?
    // =========================================================

    public bool GameEnded()
    {
        return gameEnded;
    }
}