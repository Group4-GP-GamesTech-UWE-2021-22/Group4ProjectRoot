using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemyCheck : MonoBehaviour
{
    //Variables


    //References
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;

    public GameObject doorKey;

    private void Start()
    {
        doorKey.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (enemy1 == null && enemy2 == null && enemy3 == null)
        {
            doorKey.SetActive(true);
        }       
    }
}
