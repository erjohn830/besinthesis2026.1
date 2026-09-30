using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    [Header("Game Sounds")]
    [SerializeField] private AudioClip baboySound;
    [SerializeField] private AudioClip mudSound;

    private void Awake()
    {
        // Make only one Audio Manager exist
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // =========================================================
    // BUTTON SOUND
    // =========================================================
    public void PlayButton()
    {
        PlaySound(buttonClickSound);
    }

    // =========================================================
    // WIN SOUND
    // =========================================================
    public void PlayWin()
    {
        PlaySound(winSound);
    }

    // =========================================================
    // LOSE SOUND
    // =========================================================
    public void PlayLose()
    {
        PlaySound(loseSound);
    }

    // =========================================================
    // BABOY SOUND
    // =========================================================
    public void PlayBaboy()
    {
        PlaySound(baboySound);
    }

    // =========================================================
    // MUD SOUND
    // =========================================================
    public void PlayMud()
    {
        PlaySound(mudSound);
    }

    // =========================================================
    // PLAY SOUND
    // =========================================================
    private void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is missing.");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogWarning("AudioSource is missing.");
            return;
        }

        sfxSource.PlayOneShot(clip);
    }
}