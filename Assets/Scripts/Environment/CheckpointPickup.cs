using UnityEngine;

public class CheckpointPickup : MonoBehaviour
{
    public GameObject nextPoint;

    void Start()
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (CheckpointNavigationManager.Instance != null)
            CheckpointNavigationManager.Instance.Register(this);
    }

    void OnDisable()
    {
        if (CheckpointNavigationManager.Instance != null)
            CheckpointNavigationManager.Instance.Unregister(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (nextPoint != null)
            nextPoint.SetActive(true);

        gameObject.SetActive(false);
    }
}