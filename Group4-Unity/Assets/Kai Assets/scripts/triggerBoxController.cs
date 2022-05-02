using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerBoxController : MonoBehaviour
{
    [SerializeField] private Animator Push = null;
    [SerializeField] private bool BlockPush = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (BlockPush)
            {
                Push.Play("block push", 0, 0.0f);
                gameObject.SetActive(false);
            }
        }
    }

}
