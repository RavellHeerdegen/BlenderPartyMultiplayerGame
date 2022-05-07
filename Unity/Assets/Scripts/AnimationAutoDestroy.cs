using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAutoDestroy : NetworkBehaviour
{

    public float delay = 0f;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        Debug.Log("Creating death animation of object SERVER");
        Invoke("DestroyAnimation", this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
    }

    public override void OnStartClient()
    {
        Debug.Log("Creating death animation of object CLIENT");
        Invoke("DestroyAnimation", this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
    }

    // DAS FUNKTIONIERT
    public void Start()
    {
        Invoke("DestroyAnimation", this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
    }

    void DestroyAnimation()
    {
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }

}
