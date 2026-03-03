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
        {
            if (nextPoint.GetComponent<RocketPickup>() != null)
            {
                nextPoint.GetComponent<RocketPickup>().SetCheckpointTrue(gameObject);
            }
            else
            {
                nextPoint.SetActive(true);
            }

        }

        if (transform.parent != null)
        {
            if (transform.parent.GetComponent<AudioSource>() != null)
            {
                transform.parent.GetComponent<AudioSource>().Play();
            }
        }

        gameObject.SetActive(false);
    }
}