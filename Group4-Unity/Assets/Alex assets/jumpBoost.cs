using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpBoost : MonoBehaviour
{
    public MyCharacterController player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<MyCharacterController>();
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            player.hasDoubleJumped = true;

            Destroy(this.gameObject);
        }
    }
}
