using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    GameObject player;
    //PlayerHealth playerHealth;
    EnemyHealth enemyHealth;
    UnityEngine.AI.NavMeshAgent nav;
    float speed;
    float baseSpeed = 1f;
    Animator animator;
    public bool isAngry = true;
    Vector3 destination;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //playerHealth = player.GetComponent<PlayerHealth>();
        enemyHealth = GetComponent<EnemyHealth>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (!enemyHealth.isDead)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                speed = 0;
                destination = transform.position;
            }
            else
            {
                destination = player.transform.position;
                if (isAngry)
                {
                    speed = baseSpeed * 2 * 3;
                }
                else
                {
                    speed = baseSpeed * 3;
                }
            }

            Move(speed, destination);
        }
        else
        {
            nav.enabled = false;
        }
    }

    private void Move(float speed, Vector3 destination)
    {
        animator.SetFloat("Speed_f", speed);
        nav.speed = Mathf.Min(speed, 9f);
        nav.SetDestination(destination);
    }
}
