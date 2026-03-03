using UnityEngine;

public class Scrambler : MonoBehaviour
{
    public GameObject scrambleAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        other.GetComponent<Player>().Scramble();
        GameObject g = Instantiate(scrambleAudio);
        g.transform.position = gameObject.transform.position;
        gameObject.SetActive(false);
    }
}
