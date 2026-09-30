using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    private Button button;

    [Header("Button Sound Settings")]
    [SerializeField] private float soundCooldown = 0.15f;

    private float lastSoundTime = -999f;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(PlayButtonSound);
        }
    }

    private void PlayButtonSound()
    {
        // Prevent very fast repeated sounds
        // Uses unscaledTime so it still works when Time.timeScale = 0
        if (Time.unscaledTime - lastSoundTime < soundCooldown)
            return;

        lastSoundTime = Time.unscaledTime;

        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.PlayButton();
        }
        else
        {
            Debug.LogWarning("GameAudioManager not found.");
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(PlayButtonSound);
        }
    }
}