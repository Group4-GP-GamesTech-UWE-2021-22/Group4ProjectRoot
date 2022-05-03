using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class medkit_pickup : MonoBehaviour
{



    public Player_stats player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player_stats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            player.HealthGaincall();

            Destroy(this.gameObject);
        }
    }


}
