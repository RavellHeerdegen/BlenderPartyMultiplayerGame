using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetectorPlatforms : MonoBehaviour
{
    public GameObject death_animation;

    public GameObject[] accessoires;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);

        if (collision.gameObject.name == "Rotorblade")
        {
            GameObject death_anim = Instantiate(death_animation) as GameObject;
            death_anim.transform.position = transform.position;

            for (int i = 0; i < accessoires.Length; i++)
            {
                Destroy(accessoires[i]);
            }
            Destroy(this.gameObject);

        }
    }
}
