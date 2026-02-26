using UnityEngine;

public class RocketPickup : MonoBehaviour
{

    public GameObject firstCheckpoint;
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
            if (firstCheckpoint != null)
            {
                firstCheckpoint.SetActive(true);
            }
        }
    }
}
