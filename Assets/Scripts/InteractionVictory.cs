using UnityEngine;

public class InteractionVictory : MonoBehaviour
{
    public Confetti[] confettis;
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
        if(other.tag == "SlidePuzzle")
        {
            foreach(var confetti in confettis)
            {
                confetti.Fire();
            }
        }
    }
}
