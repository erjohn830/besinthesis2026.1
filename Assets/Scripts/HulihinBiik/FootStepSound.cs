using UnityEngine;

public class PlayerFootstepSound : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource footstepSource;

    [Header("Settings")]
    [SerializeField] private float minimumMoveDistance = 0.001f;

    private Vector3 lastPosition;

    private void Start()
    {
        if (footstepSource == null)
        {
            footstepSource = GetComponent<AudioSource>();
        }

        lastPosition = transform.position;
    }

    private void Update()
    {
        if (footstepSource == null)
            return;

        Vector3 currentPosition = transform.position;

        // Ignore vertical movement
        Vector3 currentFlat = new Vector3(
            currentPosition.x,
            0f,
            currentPosition.z
        );

        Vector3 lastFlat = new Vector3(
            lastPosition.x,
            0f,
            lastPosition.z
        );

        float movedDistance = Vector3.Distance(currentFlat, lastFlat);

        bool isMoving = movedDistance > minimumMoveDistance;

        if (isMoving)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }

        lastPosition = currentPosition;
    }
}