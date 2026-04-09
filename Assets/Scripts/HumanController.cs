using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HumanController : MonoBehaviour
{
    public bool GameIsInPlay = false;

    public List<GameObject> HumanSpawnPoints;

    public GameObject FinalHuman;

    public void StartGame()
    {
        Debug.Log("Starting Game");
        foreach (GameObject gameObject in HumanSpawnPoints)
        {
            gameObject.SetActive(false);
        }

        FinalHuman.SetActive(false);

        SpawnNewHuman();
    }

    public void LoseGame()
    {
        Debug.Log("Losing Game");
        foreach (GameObject gameObject in HumanSpawnPoints)
        {
            gameObject.SetActive(false);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SpawnNewHuman()
    {

        if (HumanSpawnPoints.Count > 5)
        {
            int r = Random.Range(0, HumanSpawnPoints.Count);

            HumanSpawnPoints[r].SetActive(true);
            HumanSpawnPoints.Remove(HumanSpawnPoints.ElementAt(r));
        }
        else
        {
            FinalHuman.SetActive(true);
        }

    }
}
