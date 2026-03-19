using System;
using UnityEngine;

public class RocketPickup : MonoBehaviour
{

    public GameObject[] firstCheckpoints;
    public Timer NavigationTimer;
    public CheckpointCompletion[] finalCheckpoints;

    public Confetti[] ConfettiCannons;
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



            if (CheckpointNavigationManager.Instance != null)
                CheckpointNavigationManager.Instance.RefreshTarget();

            foreach (CheckpointCompletion g in finalCheckpoints)
            {
                g.complete = false;
            }

            GameObject[] checkPoints = GameObject.FindGameObjectsWithTag("Checkpoint");
            foreach(GameObject g in checkPoints)
            {
                g.SetActive(false);
            }

            GameObject[] scramblers = GameObject.FindGameObjectsWithTag("Scrambler");
            foreach(GameObject g in scramblers)
            {
                g.SetActive(true);
            }
          

            foreach(GameObject g in firstCheckpoints)
            {
                g.SetActive(true);
            }
            NavigationTimer.StartTimer();


        }
    }

    public void SetCheckpointTrue(GameObject checkpoint)
    {
        foreach(CheckpointCompletion g in finalCheckpoints)
        {
            if(g.checkpoint == checkpoint)
            {
                g.complete = true;
            }
        }

        bool allTrue = true;
        for (int i = 0; i < finalCheckpoints.Length; i++)
        {
            allTrue = allTrue && finalCheckpoints[i].complete;
        }

        if (allTrue)
        {
            foreach(Confetti c in ConfettiCannons)
            {
                c.Fire();
            }

            NavigationTimer.StopTimer();    
        }
    }

    public void FailChallenge()
    {
        GetComponent<AudioSource>().Play();

        foreach (CheckpointCompletion g in finalCheckpoints)
        {
            g.complete = false;
        }

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            g.SetActive(false);
        }
    }
}

[Serializable]
public class CheckpointCompletion
{
    public GameObject checkpoint;
    public bool complete = false;
}
