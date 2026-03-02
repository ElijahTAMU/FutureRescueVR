using UnityEngine;

public class CheckpointPickup : MonoBehaviour
{
    public GameObject nextPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (nextPoint != null)
        {
            nextPoint.SetActive(true);
            
        }
        else
        {
            //other.GetComponent<Player>().UnequipRockets();
        }
        gameObject.SetActive(false);
    }
}
