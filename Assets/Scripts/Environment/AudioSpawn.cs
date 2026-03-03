using UnityEngine;

public class AudioSpawn : MonoBehaviour
{
    public AudioClip ac;
    public AudioSource audioPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioPlayer.clip = ac;
        audioPlayer.Play();
        Destroy(gameObject, audioPlayer.clip.length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
