using UnityEngine;

public class RocketPickup : MonoBehaviour
{

    public GameObject[] firstCheckpoints;
    public Timer NavigationTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if(other.gameObject.tag == "Player")
        {
            Player p = other.gameObject.GetComponent<Player>();
            p.rocketsEquipped = true;

            foreach(GameObject g in firstCheckpoints)
            {
                g.SetActive(true);
            }

            if (CheckpointNavigationManager.Instance != null)
                CheckpointNavigationManager.Instance.RefreshTarget();

            NavigationTimer.StartTimer();
        }
    }
}
