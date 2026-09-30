using UnityEngine;

public class ResultPanelSound : MonoBehaviour
{
    public enum ResultType
    {
        Win,
        Lose
    }

    [Header("Result")]
    [SerializeField] private ResultType resultType;

    private bool hasPlayed = false;

    private void OnEnable()
    {
        hasPlayed = false;

        PlayResultSound();
    }

    private void PlayResultSound()
    {
        if (hasPlayed)
            return;

        hasPlayed = true;

        if (GameAudioManager.Instance == null)
        {
            Debug.LogWarning("GameAudioManager not found.");
            return;
        }

        if (resultType == ResultType.Win)
        {
            GameAudioManager.Instance.PlayWin();
        }
        else
        {
            GameAudioManager.Instance.PlayLose();
        }
    }
}