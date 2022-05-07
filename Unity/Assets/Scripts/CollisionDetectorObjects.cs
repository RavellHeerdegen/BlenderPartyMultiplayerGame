using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetectorObjects : NetworkBehaviour
{
    public GameObject death_animation;

    [SerializeField] private GameObject deathsound_Prefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Rotorblade")
        {
            DestroyGameObject();
        }
    }

    void DestroyGameObject()
    {
        // Muss auf allen Clients passieren an alle Clients gesendet werden
        GameObject death_anim = Instantiate(death_animation) as GameObject;
        death_anim.transform.position = transform.position;

        //NetworkServer.Spawn(death_anim); // Spawn death animation on all clients

        // Play death sound
        GameObject deathsound_object = Instantiate(deathsound_Prefab);
        //NetworkServer.Spawn(deathsound_object);


        //NetworkServer.Destroy(gameObject);
        //Destroy(gameObject.transform.parent.gameObject);
        gameObject.SetActive(false);

        //NetworkServer.UnSpawn(gameObject);
    }
}
