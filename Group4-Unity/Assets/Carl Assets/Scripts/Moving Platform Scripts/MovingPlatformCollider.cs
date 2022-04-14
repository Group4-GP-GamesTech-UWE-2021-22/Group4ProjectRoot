using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformCollider : MonoBehaviour
{

    Vector3 scale;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            scale = other.transform.localScale;
            other.transform.SetParent(this.transform);
            /*other.transform.localScale = scale;*/
            other.GetComponent<MyCharacterController>().onMovingPlatform = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.SetParent(null);
            //other.transform.localScale = scale;
            other.GetComponent<MyCharacterController>().onMovingPlatform = false;

        }

    }
}


