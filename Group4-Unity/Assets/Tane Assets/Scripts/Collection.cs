using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    //Variabes
    public bool collected;

    //References
    public GameObject self;

    // Start is called before the first frame update
    void Start()
    {
        collected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collected = true;
            self.SetActive(false);
        }
    }
}
