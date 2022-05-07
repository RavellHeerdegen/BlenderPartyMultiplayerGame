using Mirror;
using UnityEngine;

public class SoundAutoDestroy : NetworkBehaviour
{
    public float delay = 0f;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        playSound();
    }
    
    public override void OnStartClient()
    {
        playSound();
    }

    // DAS FUNKTIONIERT
    public void Start()
    {
        gameObject.GetComponent<AudioSource>().Play();

        Invoke("DestroySound", delay);
    }

    private void playSound()
    {
        gameObject.GetComponent<AudioSource>().Play();

        Invoke("DestroySound", delay);
    }

    void DestroySound()
    {
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
