using UnityEngine;

public class Confetti : MonoBehaviour
{
    public ParticleSystem ps;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire()
    {
        ps.Play();
        GetComponent<AudioSource>().Play();
    }
}
