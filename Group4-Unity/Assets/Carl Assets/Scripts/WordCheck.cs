using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordCheck : MonoBehaviour
{

    public GameObject[] letterCubes;
    public float wordLength;
    public int[] letterStates;
    public int correctCubes = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (letterCubes.Length != wordLength)
        {
            Debug.Log("Word Check Script Error - Not enough cubes for word length!!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*        for (int i = 0; i < wordLength; i++)
                {
                    if (letterCubes[i].GetComponent<CubeMaterialStateManager>().state == letterStates[i])
                    {
                        correctCubes++;
                    }
                }*/

        int count = 0;
        for (int i = 0; i < wordLength; i++)
        {
            if (letterCubes[i].GetComponent<CubeMaterialStateManager>().correct)
            {
                count++;
            }
        }
        if (count == wordLength)
        {
            gameObject.GetComponent<CubeMaterialStateManager>().state = 0;
        }

    }

    public void incrementCorrectCubes()
    {
        correctCubes++;
    }
}
