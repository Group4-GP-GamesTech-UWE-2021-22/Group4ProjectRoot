using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class fade_effect : MonoBehaviour
{
    public Image fade;

    public enum State {DO_NOTHING, FADE_IN, FADE_OUT };

    public State curr_state;


    public float some_timer;

    bool done1 = false;

    public float fade_in_speed = 0.1f;
    public float fade_out_speed = 2f;

    public string level;




    // Start is called before the first frame update
    void Start()
    {
        curr_state = State.FADE_IN;
    }

    // Update is called once per frame
    void Update()
    {
        


        switch (curr_state) 
        {

            case State.DO_NOTHING:


                break;


            case State.FADE_IN:

                fade_in();

                break;


            case State.FADE_OUT:
                fade_out();

                break;


        }
    }




    public void fade_in() 
    {
        fade.color = new Color32(0, 0, 0, (byte)((fade.color.a - (Time.deltaTime * fade_in_speed)) * 255));


        if (fade.color.a < 0.05) 
        {
            curr_state = State.DO_NOTHING;
            fade.color = new Color32(0, 0, 0, (byte)(0 * 255));
        }

    }


    public void fade_out()
    {

        Debug.Log(fade.color.a + (Time.deltaTime * fade_out_speed));
        fade.color = new Color32(0, 0, 0, (byte)((fade.color.a + (Time.deltaTime * fade_out_speed)) * 255));

        if (fade.color.a > 0.95)
        {
            curr_state = State.DO_NOTHING;
            fade.color = new Color32(0, 0, 0, (byte)(1 * 255));
            SceneManager.LoadScene(level);
        }
    }


    private void OnTriggerEnter(Collider other)
    {


        if (other.transform.tag == "Player"  &&  !done1) 
        {

             curr_state = State.FADE_OUT; 
            
            done1 = true;


        }

    }

}
