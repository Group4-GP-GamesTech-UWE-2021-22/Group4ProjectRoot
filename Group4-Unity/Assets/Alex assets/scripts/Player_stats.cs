using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player_stats : MonoBehaviour
{
    public Slider health_slider;

    private int coins_nums;

    public float health = 100;
    public float max_health = 100;
    private bool dead = false;    // WIP

    private bool take_damage_able = true;

    private float damage_take_rate = 2f;
    private float last_damage = 0.0f;


    private void Awake()
    {
        health = dataToParse.health_remaining;
        //move_contr = FindObjectOfType<Movement_controller>();
    }


    void Update()
    {
        if (health < 0 && !dead)
        {
            health = 0;
            //move_contr.player_state = Movement_controller.status_state.DEAD;
            //move_contr.anim.SetTrigger("dead");
            gameObject.GetComponent<Animator>().SetTrigger("death");
            dead = true;
        }
        else if (health > 100)
        {
            health = 100;
        }



        dataToParse.health_remaining = health;

        health_slider.value = health / max_health;
    }



    public void take_damage(float damage)
    {
        if (take_damage_able)
        {
            health -= damage;
            take_damage_able = false;
        }
        else
        {

            if (Time.time > damage_take_rate + last_damage)
            {
                last_damage = Time.time;
                take_damage_able = true;
            }
        }

    }



    public void HealthGaincall()
    {
        health += 30;
    }

    public void Coincall()
    {
        coins_nums += 1;
    }

    private void OnGUI()
    {
        // GUI.Box(new Rect(20, Screen.height - 50, 125, 30), "Coins collected: " + coins_nums);
    }
}
