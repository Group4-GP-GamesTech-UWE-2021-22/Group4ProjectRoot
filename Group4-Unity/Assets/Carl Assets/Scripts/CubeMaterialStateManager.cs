using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMaterialStateManager : MonoBehaviour
{

    [SerializeField] public Material[] materials;
    Material currentMat;
    public int state = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentMat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
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
        }
    }
}
