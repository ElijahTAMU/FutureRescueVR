using UnityEngine;

public class HumanTarget : MonoBehaviour
{

    public bool isFinalHuman;
    public Confetti[] Confettis;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (isFinalHuman)
            {
                gameObject.SetActive(false);
                foreach (Confetti c in Confettis)
                {

                    c.Fire();
                }
            }
            else
            {
                gameObject.SetActive(false);
                HumanController hc = GameObject.FindGameObjectWithTag("NPCHandler").GetComponent<HumanController>();
                GameObject.Find("Scanner").GetComponent<Scanner>().timer = 60;
                hc.SpawnNewHuman();
            }
        }
    }
}
