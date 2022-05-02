using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCheck : MonoBehaviour
{
    //Variables

    //References
    public PuzzleLight red;
    public PuzzleLight blue;
    public PuzzleLight green;

    public GameObject doorKey;

    // Start is called before the first frame update
    void Start()
    {
        //keyDrop = false;
        doorKey.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        LightActiveCheck();
    }

    public void LightActiveCheck()
    {
        if (red.lightActive)
        {
            StartCoroutine(RedCheck());
        }

        if (blue.lightActive)
        {
            StartCoroutine(BlueCheck());
        }

        if (green.lightActive)
        {
            StartCoroutine(GreenCheck());
        }
    }

    public IEnumerator RedCheck()
    {
        if (!green.lightActive)
        {
            yield return new WaitForSeconds(1f);

            red.lightActive = false;
            red.light.enabled = false;
            red.collider.enabled = true;

            blue.lightActive = false;
            blue.light.enabled = false;
            blue.collider.enabled = true;

            green.lightActive = false;
            green.light.enabled = false;
            green.collider.enabled = true;
        }
    }

    public IEnumerator BlueCheck()
    {
        if (green.lightActive)
        {
            yield return new WaitForSeconds(1f);

            blue.lightActive = false;
            blue.light.enabled = false;
            blue.collider.enabled = true;

            green.lightActive = false;
            green.light.enabled = false;
            green.collider.enabled = true;
        }

        if (red.lightActive)
        {
            yield return new WaitForSeconds(1f);

            red.lightActive = false;
            red.light.enabled = false;
            red.collider.enabled = true;

            blue.lightActive = false;
            blue.light.enabled = false;
            blue.collider.enabled = true;
        }
    }

    public IEnumerator GreenCheck()
    {
        if (red.lightActive)
        {
            if (blue.lightActive)
            {
                //bluebool = true;
                keyDropFunction();

                red.light.enabled = true;

                blue.light.enabled = true;

                green.light.enabled = true;
            }
        }

        if (blue.lightActive)
        {
            yield return new WaitForSeconds(1f);

            blue.lightActive = false;
            blue.light.enabled = false;
            blue.collider.enabled = true;

            green.lightActive = false;
            green.light.enabled = false;
            green.collider.enabled = true;
        }
    }

    void keyDropFunction()
    {
        doorKey.SetActive(true);
    }
}
