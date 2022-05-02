using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLight : MonoBehaviour
{
    //Variables
    public bool lightActive;

    //References
    public GameObject button;
    public Collider collider;
    public Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = button.GetComponent<Light>();
        light.enabled = false;

        lightActive = false;
    }
    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftArm"))
        {
            light.enabled = true;
            lightActive = true;
            collider.enabled = false;
        }
    }
}
