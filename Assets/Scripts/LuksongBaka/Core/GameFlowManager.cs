using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameFlowManager : MonoBehaviour
{
    [Header("Main References")]
    public PlayerController player;
    public LuksongBakaTimer gameTimer;

    [Header("Player Start")]
    public Transform playerStartPoint;

    [Header("Gameplay")]
    public SlowZoneTrigger slowZone;
    public TimingBar timingBar;

    [Header("Gameplay UI")]
    public GameObject gameplayHUD;
    public GameObject jumpButton;

    [Header("Countdown")]
    public GameObject countdownPanel;
    public Image countdownImage;

    [Header("Countdown Sprites")]
    public Sprite number3;
    public Sprite number2;
    public Sprite number1;
    public Sprite goImage;

    [Header("Countdown Duration")]
    public float numberDuration = 1f;
    public float goDuration = 0.7f;

    // SlowZone checks this.
    public bool GameplayStarted
    {
        get;
        private set;
    }

    private bool starting = false;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        GameplayStarted = false;
        starting = false;

        // Hide TimingBar.
        if (timingBar != null)
        {
            timingBar.Deactivate();
        }

        // Hide Jump button.
        if (jumpButton != null)
        {
            jumpButton.SetActive(false);
        }

        // Reset jump timer.
        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
        }

        HideCountdown();

        // Prepare player.
        ResetPlayerToSpawn();

        if (player != null)
        {
            player.StopPlayer();
        }
    }

    // =========================================================
    // LOCK GAMEPLAY
    // Used during menu/tutorial.
    // =========================================================

    public void LockGameplay()
    {
        StopAllCoroutines();

        GameplayStarted = false;
        starting = false;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (player != null)
        {
            player.StopPlayer();
        }

        if (gameTimer != null)
        {
            gameTimer.StopTimer();
        }

        if (timingBar != null)
        {
            timingBar.Deactivate();
        }

        if (jumpButton != null)
        {
            jumpButton.SetActive(false);
        }

        HideCountdown();
    }

    // =========================================================
    // RESET PLAYER TO SPAWN
    // =========================================================

    public void ResetPlayerToSpawn()
    {
        if (player == null)
        {
            Debug.LogError(
                "PLAYER IS NOT ASSIGNED IN GAME FLOW!"
            );

            return;
        }

        // Prefer PlayerController's own Start Point.
        if (playerStartPoint != null)
        {
            player.TeleportTo(
                playerStartPoint
            );

            player.PrepareAtSpawn();
        }
        else
        {
            // Backup.
            player.ResetToStart();
        }
    }

    // =========================================================
    // NORMAL START
    // =========================================================

    public void ConfirmTaya()
    {
        if (starting)
        {
            return;
        }

        Debug.Log(
            "STARTING LUKSONG BAKA"
        );

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        starting = true;
        GameplayStarted = false;

        // Make sure HUD is visible.
        if (gameplayHUD != null)
        {
            gameplayHUD.SetActive(true);
        }

        // Hide timing controls first.
        if (timingBar != null)
        {
            timingBar.Deactivate();
        }

        if (jumpButton != null)
        {
            jumpButton.SetActive(false);
        }

        // Reset player.
        ResetPlayerToSpawn();

        // Reset SlowZone.
        if (slowZone != null)
        {
            slowZone.ResetZone();
        }

        // Reset the 5/7 second jump timer.
        //
        // IMPORTANT:
        // Do NOT StartTimer here.
        // Timer starts when SlowZone is touched.
        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
        }

        StartCoroutine(
            CountdownRoutine()
        );
    }

    // =========================================================
    // COUNTDOWN
    // =========================================================

    private IEnumerator CountdownRoutine()
    {
        GameplayStarted = false;

        if (player != null)
        {
            player.StopPlayer();
        }

        // =============================================
        // 3
        // =============================================

        ShowCountdown(
            number3
        );

        yield return new WaitForSecondsRealtime(
            numberDuration
        );

        // =============================================
        // 2
        // =============================================

        ShowCountdown(
            number2
        );

        yield return new WaitForSecondsRealtime(
            numberDuration
        );

        // =============================================
        // 1
        // =============================================

        ShowCountdown(
            number1
        );

        yield return new WaitForSecondsRealtime(
            numberDuration
        );

        // =============================================
        // GO
        // =============================================

        ShowCountdown(
            goImage
        );

        yield return new WaitForSecondsRealtime(
            goDuration
        );

        // =============================================
        // START REAL GAME
        // =============================================

        HideCountdown();

        GameplayStarted = true;
        starting = false;

        // IMPORTANT:
        // Jump timer does NOT begin here.
        //
        // It begins when Player enters SlowZone.

        if (player != null)
        {
            player.StartRun();
        }

        Debug.Log(
            "GO! PLAYER STARTED RUNNING"
        );
    }

    // =========================================================
    // SHOW COUNTDOWN SPRITE
    // =========================================================

    private void ShowCountdown(
        Sprite sprite)
    {
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }

        if (countdownImage == null)
        {
            Debug.LogError(
                "COUNTDOWN IMAGE NOT ASSIGNED!"
            );

            return;
        }

        if (sprite == null)
        {
            Debug.LogError(
                "COUNTDOWN SPRITE IS MISSING!"
            );

            countdownImage.enabled = false;

            return;
        }

        countdownImage.sprite = sprite;
        countdownImage.enabled = true;
        countdownImage.preserveAspect = true;
    }

    // =========================================================
    // HIDE COUNTDOWN
    // =========================================================

    private void HideCountdown()
    {
        if (countdownImage != null)
        {
            countdownImage.enabled = false;
        }

        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }
    }

    // =========================================================
    // REWARDED AD CONTINUE
    // =========================================================

    public void ResumeCurrentLevelAfterRewardedAd()
    {
        Debug.Log(
            "RESUME SAME LEVEL AFTER REWARDED AD"
        );

        StopAllCoroutines();

        // =============================================
        // RESTORE NORMAL TIME
        // =============================================

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        starting = false;
        GameplayStarted = false;

        // =============================================
        // SHOW HUD
        // =============================================

        if (gameplayHUD != null)
        {
            gameplayHUD.SetActive(true);
        }

        // =============================================
        // RESET PLAYER
        // =============================================

        ResetPlayerToSpawn();

        // =============================================
        // RESET SLOW ZONE
        // =============================================

        if (slowZone != null)
        {
            slowZone.ResetZone();
        }

        // =============================================
        // HIDE TIMING BAR
        // =============================================

        if (timingBar != null)
        {
            timingBar.Deactivate();
        }

        // =============================================
        // HIDE JUMP BUTTON
        // =============================================

        if (jumpButton != null)
        {
            jumpButton.SetActive(false);
        }

        // =============================================
        // RESET JUMP TIMER
        // =============================================

        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
        }

        // =============================================
        // START COUNTDOWN AGAIN
        // =============================================

        starting = true;

        StartCoroutine(
            CountdownRoutine()
        );
    }
}