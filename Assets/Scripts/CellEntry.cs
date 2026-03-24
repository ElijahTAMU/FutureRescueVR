using UnityEngine;

public class CellEntry : MonoBehaviour
{
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
        if(other.tag == "Player")
        {
            other.transform.parent = gameObject.transform.parent.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player" || other.tag == "SlidePuzzle")
        {
            other.transform.parent = null;
        }
    }
}
