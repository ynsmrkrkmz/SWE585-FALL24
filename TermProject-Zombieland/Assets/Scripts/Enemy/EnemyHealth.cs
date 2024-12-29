using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public float sinkSpeed = 2.5f;
    public AudioClip deathClip;
    public Sprite icon;
    public bool isDead;


    Animator anim;
    AudioSource enemyAudio;
    ParticleSystem hitParticles;
    CapsuleCollider capsuleCollider;
    bool isSinking;


    void Awake()
    {
        anim = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        hitParticles = GetComponentInChildren<ParticleSystem>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        currentHealth = startingHealth;
    }

    private void OnEnable()
    {
        Reset();
    }

    void Update()
    {
        if (isSinking)
        {
            transform.Translate(sinkSpeed * Time.deltaTime * -Vector3.up);
        }
    }


    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (isDead)
            return;

        enemyAudio.Play();

        currentHealth -= amount;

        hitParticles.transform.position = hitPoint;
        hitParticles.Play();

        if (currentHealth <= 0)
        {
            Death();
        }
    }


    void Death()
    {
        isDead = true;

        capsuleCollider.isTrigger = isDead;

        enemyAudio.clip = deathClip;
        enemyAudio.Play();
        StartSinking();
    }

    void StartSinking()
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = !isDead;
        GetComponent<Rigidbody>().isKinematic = isDead;
        isSinking = isDead;
            Destroy(gameObject, 0.5f);
    }

    private void Reset()
    {
        isDead = false;
        capsuleCollider.isTrigger = !isDead;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = !isDead;
        GetComponent<Rigidbody>().isKinematic = isDead;
        isSinking = isDead;
    }
}
