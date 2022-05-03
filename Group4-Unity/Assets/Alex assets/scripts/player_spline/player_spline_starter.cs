using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_spline_starter : MonoBehaviour
{
    public curve_route[] player_route;

    [SerializeField]
    private int curr_index = 0;

    [Range(0, 1f)]
    public float t = 0;

    private GameObject Player;
    private Transform Player_body;

    public GameObject cart;

    public Camera cam;


    private MyCharacterController move_cont;
    //private GameManager manager;


    [Range(0.001f, 0.1f)]
    public float modifier_speed = 0.05f;

    private bool draw_text;

    // Start is called before the first frame update

    // issue is going to be with the parenting if the player follows the spline once out and parented
    // still need th eplayer in sidemovement state and only allow side to side movement?????????????

    public Vector3 last_pos;
    public Vector3 next_pos;


    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Player_body = Player.transform.GetChild(0);
        move_cont = FindObjectOfType<MyCharacterController>();
        //manager = FindObjectOfType<GameManager>();



        player_route = new curve_route[this.transform.childCount];


        for (int i = 0; i < this.transform.childCount; i++)
        {
            player_route[i] = this.transform.GetChild(i).gameObject.GetComponent<curve_route>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
        if (move_cont.onSpline ==true)
        {


            if (move_cont.xmove > 0) 
            {

                t = t + move_cont.xmove * Time.deltaTime * modifier_speed;
            }
            else if (move_cont.xmove == 0)
            {
                t = t + Time.deltaTime * 0.01f;
            }
            else if (move_cont.xmove < 0) 
            { 
            
            }

            

          

            next_pos = player_route[curr_index].Getpos(t);

           rot_cart();

            move_cont.SplineChangeDir(new Vector3(next_pos.x, Player.transform.position.y, next_pos.z));

            Player.transform.position = Vector3.Lerp(Player.transform.position, new Vector3(next_pos.x, next_pos.y, next_pos.z), 0.8f);

            


            if (move_cont.xmove > 0.1f)
            {
                Debug.Log("fornt");
                //move_cont.forward = true;
               // move_cont.move_action = Movement_controller.action_state.FORWARD;

                float t_look = t + move_cont.xmove * Time.deltaTime * modifier_speed + 0.5f;

                if (t_look > 1) { t_look = 1; }
                else if (t_look < 0) { t_look = 0; }

                var full_lookat = player_route[curr_index].Getpos(t);
                Vector3 look_at_pos = new Vector3(full_lookat.x, Player.transform.position.y, full_lookat.z);

                Player_body.transform.LookAt(look_at_pos);

            }
            else if (move_cont.xmove < -0.1f)
            {
                Debug.Log("back");
                //move_cont.forward = false;
                //move_cont.move_action = Movement_controller.action_state.FORWARD;

                float t_look = t + move_cont.xmove * Time.deltaTime * modifier_speed - 0.5f;
                if (t_look > 1) { t_look = 1; }
                else if (t_look < 0) { t_look = 0; }

                var full_lookat = player_route[curr_index].Getpos(t);
                Vector3 look_at_pos = new Vector3(full_lookat.x, Player_body.transform.position.y, full_lookat.z);

                Player_body.transform.LookAt(look_at_pos);

            }
            else if (move_cont.xmove >= -0.1f && move_cont.xmove <= 0.1f)
            {
                //move_cont.move_action = Movement_controller.action_state.IDLE;
            }


            if (t > 1)
            {
                curr_index++;

                if (curr_index == player_route.Length)
                {
                    move_cont.onSpline = false;
                    t = 0;
                    curr_index = 0;

                }


                t = 0;

            }
            //if we are not going to have a back then this has to change
            else if (t < 0)
            {
                curr_index--;
                if (curr_index < 0)
                {
                    move_cont.onSpline = false;
                    t = 0f;
                    curr_index = 0;
                }


                t = 1;

            }



            last_pos = next_pos;



        }
    }


    private void rot_cart() 
    {

        cam.transform.position = new Vector3 (last_pos.x + 15, last_pos.y + 3, last_pos.z);

        cam.transform.LookAt(next_pos);
        cart.transform.position = last_pos;
        cart.transform.LookAt(next_pos);
    

    }





    private void OnTriggerStay(Collider other)
    {
        
        if ( other.transform.tag == "Player" && !move_cont.onSpline) 
        {


            t = 0;

            move_cont.onSpline = true;




        }






        /*

        if (move_cont.action && other.transform.tag == "Player")
        {
            t = 0;

            
            if (move_cont.onSpline == true)
            {
                Debug.Log("is this getting caleld");
                move_cont.onSpline = false;
            }
            else if (move_cont.onSpline == false)
            {
                t = 0;

                Debug.Log("is this getting asdsadasdasdsdassadasddscaleld");
                //move_cont.forward = true;
                move_cont.onSpline = true;
            }
            move_cont.action = false;
            
        }
        */
        
    }// when on the first and going back it restarts


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            draw_text = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            draw_text = false;
        }
    }


    private void OnGUI()
    {
        
    }


}
