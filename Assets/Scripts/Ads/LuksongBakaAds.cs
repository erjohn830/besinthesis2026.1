using UnityEngine;
using Unity.Services.LevelPlay;

public class LuksongBakaAds : MonoBehaviour
{
    [Header("LEVELPLAY - ANDROID")]
    [SerializeField] private string appKey;
    [SerializeField] private string rewardedAdUnitId;

    [Header("LUKSONG BAKA REFERENCES")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameFlowManager gameFlow;

    [Header("UI")]
    [SerializeField] private GameObject losePanel;

    private LevelPlayRewardedAd rewardedAd;

    // =========================================================
    // REWARD STATE
    // =========================================================

    private bool waitingForReward = false;
    private bool rewardEarned = false;
    private bool adClosed = false;
    private bool rewardAlreadyGiven = false;

    // =========================================================
    // SAVED GAME STATE
    // =========================================================

    private int savedLevel = 1;
    private int savedScore = 0;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        RestoreNormalTime();

        if (levelManager == null)
        {
            levelManager =
                FindFirstObjectByType<LevelManager>();
        }

        if (gameFlow == null)
        {
            gameFlow =
                FindFirstObjectByType<GameFlowManager>();
        }

        LevelPlay.OnInitSuccess +=
            OnInitSuccess;

        LevelPlay.OnInitFailed +=
            OnInitFailed;

#if UNITY_EDITOR

        Debug.Log(
            "EDITOR MODE - Rewarded ads must be tested on Android."
        );

#elif UNITY_ANDROID

        // Child-directed settings.
        LevelPlayPrivacySettings.SetCOPPA(true);

        LevelPlay.SetMetaData(
            "is_deviceid_optout",
            "true"
        );

        if (string.IsNullOrWhiteSpace(appKey))
        {
            Debug.LogError(
                "LEVELPLAY ERROR: App Key is empty!"
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(rewardedAdUnitId))
        {
            Debug.LogError(
                "LEVELPLAY ERROR: Rewarded Ad Unit ID is empty!"
            );

            return;
        }

        Debug.Log(
            "INITIALIZING LUKSONG BAKA LEVELPLAY..."
        );

        LevelPlay.Init(appKey);

#endif
    }

    // =========================================================
    // RESTORE NORMAL GAME SPEED
    // =========================================================

    private void RestoreNormalTime()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    // =========================================================
    // LEVELPLAY INITIALIZED
    // =========================================================

    private void OnInitSuccess(
        LevelPlayConfiguration configuration)
    {
        Debug.Log(
            "LEVELPLAY INITIALIZED SUCCESSFULLY"
        );

#if UNITY_ANDROID && !UNITY_EDITOR

        rewardedAd =
            new LevelPlayRewardedAd(
                rewardedAdUnitId
            );

        rewardedAd.OnAdLoaded +=
            OnAdLoaded;

        rewardedAd.OnAdLoadFailed +=
            OnAdLoadFailed;

        rewardedAd.OnAdDisplayed +=
            OnAdDisplayed;

        rewardedAd.OnAdDisplayFailed +=
            OnAdDisplayFailed;

        rewardedAd.OnAdRewarded +=
            OnAdRewarded;

        rewardedAd.OnAdClosed +=
            OnAdClosed;

        LoadRewardedAd();

#endif
    }

    // =========================================================
    // INITIALIZATION FAILED
    // =========================================================

    private void OnInitFailed(
        LevelPlayInitError error)
    {
        RestoreNormalTime();

        Debug.LogError(
            "LEVELPLAY INIT FAILED: "
            + error
        );
    }

    // =========================================================
    // LOAD AD
    // =========================================================

    private void LoadRewardedAd()
    {
        if (rewardedAd == null)
        {
            return;
        }

        Debug.Log(
            "LOADING REWARDED AD..."
        );

        rewardedAd.LoadAd();
    }

    // =========================================================
    // WATCH AD BUTTON
    // =========================================================

    public void ShowRewardedAd()
    {
#if UNITY_EDITOR

        RestoreNormalTime();

        Debug.LogWarning(
            "Rewarded ads must be tested on Android."
        );

        return;

#elif UNITY_ANDROID

        // IMPORTANT:
        // Never allow the game to remain paused.
        RestoreNormalTime();

        if (rewardedAd == null)
        {
            Debug.LogWarning(
                "Rewarded ad is not initialized."
            );

            return;
        }

        if (!rewardedAd.IsAdReady())
        {
            Debug.LogWarning(
                "Rewarded ad is not ready."
            );

            LoadRewardedAd();

            return;
        }

        if (levelManager == null)
        {
            Debug.LogError(
                "LevelManager is missing!"
            );

            return;
        }

        // =============================================
        // SAVE LEVEL AND SCORE
        // =============================================

        savedLevel =
            levelManager.currentLevel;

        savedScore =
            levelManager.score;

        Debug.Log(
            "SAVING BEFORE AD"
            + " | LEVEL = "
            + savedLevel
            + " | SCORE = "
            + savedScore
        );

        // =============================================
        // RESET REWARD STATE
        // =============================================

        waitingForReward = true;
        rewardEarned = false;
        adClosed = false;
        rewardAlreadyGiven = false;

        // DO NOT USE:
        //
        // Time.timeScale = 0f;
        //
        // We don't want the game to get stuck paused.

        RestoreNormalTime();

        Debug.Log(
            "SHOWING REWARDED AD..."
        );

        rewardedAd.ShowAd();

#endif
    }

    // =========================================================
    // AD LOADED
    // =========================================================

    private void OnAdLoaded(
        LevelPlayAdInfo adInfo)
    {
        Debug.Log(
            "REWARDED AD READY"
        );
    }

    // =========================================================
    // LOAD FAILED
    // =========================================================

    private void OnAdLoadFailed(
        LevelPlayAdError error)
    {
        RestoreNormalTime();

        Debug.LogError(
            "REWARDED AD LOAD FAILED: "
            + error
        );
    }

    // =========================================================
    // AD DISPLAYED
    // =========================================================

    private void OnAdDisplayed(
        LevelPlayAdInfo adInfo)
    {
        // Make sure timescale is normal.
        RestoreNormalTime();

        Debug.Log(
            "REWARDED AD DISPLAYED"
        );
    }

    // =========================================================
    // DISPLAY FAILED
    // =========================================================

    private void OnAdDisplayFailed(
        LevelPlayAdInfo adInfo,
        LevelPlayAdError error)
    {
        RestoreNormalTime();

        Debug.LogError(
            "REWARDED AD DISPLAY FAILED: "
            + error
        );

        ResetRewardState();

        LoadRewardedAd();
    }

    // =========================================================
    // REWARD EARNED
    // =========================================================

    private void OnAdRewarded(
        LevelPlayAdInfo adInfo,
        LevelPlayReward reward)
    {
        RestoreNormalTime();

        Debug.Log(
            "PLAYER EARNED REWARD: "
            + reward.Name
            + " x"
            + reward.Amount
        );

        if (!waitingForReward)
        {
            return;
        }

        rewardEarned = true;

        TryGiveReward();
    }

    // =========================================================
    // AD CLOSED
    // =========================================================

    private void OnAdClosed(
        LevelPlayAdInfo adInfo)
    {
        // MOST IMPORTANT:
        // restore normal gameplay speed.
        RestoreNormalTime();

        Debug.Log(
            "REWARDED AD CLOSED"
        );

        adClosed = true;

        TryGiveReward();

        LoadRewardedAd();
    }

    // =========================================================
    // GIVE CONTINUE
    // =========================================================

    private void TryGiveReward()
    {
        if (!waitingForReward)
        {
            return;
        }

        if (!rewardEarned)
        {
            return;
        }

        if (!adClosed)
        {
            return;
        }

        if (rewardAlreadyGiven)
        {
            return;
        }

        rewardAlreadyGiven = true;
        waitingForReward = false;

        // =============================================
        // ABSOLUTELY MAKE SURE GAME IS NOT PAUSED
        // =============================================

        RestoreNormalTime();

        // =============================================
        // HIDE LOSE PANEL
        // =============================================

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        // =============================================
        // RESTORE LEVEL + SCORE
        // =============================================

        if (levelManager != null)
        {
            levelManager.ReviveFromRewardedAd(
                savedLevel,
                savedScore
            );
        }
        else
        {
            Debug.LogError(
                "LevelManager is missing!"
            );

            return;
        }

        // =============================================
        // RESTART SAME LEVEL
        // =============================================

        if (gameFlow != null)
        {
            gameFlow.ResumeCurrentLevelAfterRewardedAd();
        }
        else
        {
            Debug.LogError(
                "GameFlowManager is missing!"
            );

            return;
        }

        RestoreNormalTime();

        Debug.Log(
            "REWARDED CONTINUE COMPLETE"
            + " | LEVEL = "
            + savedLevel
            + " | SCORE = "
            + savedScore
        );
    }

    // =========================================================
    // RESET REWARD STATE
    // =========================================================

    private void ResetRewardState()
    {
        waitingForReward = false;
        rewardEarned = false;
        adClosed = false;
        rewardAlreadyGiven = false;

        RestoreNormalTime();
    }

    // =========================================================
    // APPLICATION FOCUS
    // =========================================================

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            // Some Android ad screens temporarily
            // remove application focus.
            //
            // When Unity gets focus again,
            // make sure gameplay is not frozen.
            RestoreNormalTime();
        }
    }

    // =========================================================
    // APPLICATION PAUSE
    // =========================================================

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            // App returned from advertisement.
            RestoreNormalTime();
        }
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        LevelPlay.OnInitSuccess -=
            OnInitSuccess;

        LevelPlay.OnInitFailed -=
            OnInitFailed;

        if (rewardedAd != null)
        {
            rewardedAd.OnAdLoaded -=
                OnAdLoaded;

            rewardedAd.OnAdLoadFailed -=
                OnAdLoadFailed;

            rewardedAd.OnAdDisplayed -=
                OnAdDisplayed;

            rewardedAd.OnAdDisplayFailed -=
                OnAdDisplayFailed;

            rewardedAd.OnAdRewarded -=
                OnAdRewarded;

            rewardedAd.OnAdClosed -=
                OnAdClosed;
        }

        RestoreNormalTime();
    }
}