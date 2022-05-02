using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMaterialStateManager : MonoBehaviour
{

    [SerializeField] public Material[] materials;
    Material currentMat;
    public int state = 0;

    public bool correct = false;
    public bool isPuzzle = false;
    public int letterPosition = 0;
    public int targetPosition = 0;
    [SerializeField] public GameObject resultCube;

    // Start is called before the first frame update
    void Start()
    {
        currentMat = GetComponent<Renderer>().material;

        if (resultCube == null && isPuzzle)
        {
            Debug.Log("Warning - No Result Cube Connected, intended?");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state == letterPosition)
        {
            correct = true;
        }
        else
        {
            correct = false;
        }
        if (currentMat != null)
        {
            //if (currentMat.mainTexture == alphanetTextures[state])
            if (currentMat != materials[state])
            {
                currentMat = materials[state];
            }
        }
        GetComponent<Renderer>().material = currentMat;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            state++;
            if (state == materials.Length)
            {
                state = 0;
            }
            /* if (state == resultCube.GetComponent<WordCheck>().letterStates[letterPosition])
             {
                 resultCube.GetComponent<WordCheck>().incrementCorrectCubes();
                 correct = true;
             }
            */ /*            else if (state == resultCube.GetComponent<WordCheck>().letterStates[letterPosition + 1])
                         {
                             resultCube.GetComponent<WordCheck>().correctCubes--;
                             correct = false;
                         }*/

        }
    }
}
