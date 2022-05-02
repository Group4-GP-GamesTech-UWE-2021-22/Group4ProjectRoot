using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeysCollectedCheck : MonoBehaviour
{
    //Variables


    //References
    public Collection key1;
    public Collection key2;
    public Collection key3;

    public GameObject keySlot1;
    public GameObject keySlot2;
    public GameObject keySlot3;

    public GameObject bossDoor;

    // Start is called before the first frame update
    void Start()
    {
        keySlot1.SetActive(false);
        keySlot2.SetActive(false);
        keySlot3.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player"))
        {
            if (key1.collected && key2.collected && key3.collected)
            {
                keySlot1.SetActive(true);
                keySlot2.SetActive(true);
                keySlot3.SetActive(true);
            }
        }
    }
}
