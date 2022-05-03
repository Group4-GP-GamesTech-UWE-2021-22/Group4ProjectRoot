using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speednoost : MonoBehaviour
{
    public MyCharacterController player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<MyCharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            player.increaseMaxSpeed(3,6);

            Destroy(this.gameObject);
        }
    }

}
