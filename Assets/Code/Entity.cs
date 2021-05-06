﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    protected Rigidbody rb;
    protected Collider collide;

    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    // Physics stuff
    virtual protected Vector3 GetVelocity()
    {
        if (!rb) return Vector3.zero;
        return (rb.position - lastPosition) / Time.deltaTime;
    }
    private Vector3 lastPosition;

    public Vector3 GetLocation()
    {
        return rb.position;
    }

    public Animator animator;

    /*public float iFrameTime = 0f;
    public bool iSuperArmor = false;*/

    public int maxHealth = 100;
    protected int currHealth = 10;
    protected bool dead = false;
    protected bool invincible = false;

    public bool destroyOnDeath = false;

    protected bool hitstun
    {
        get
        {
            return hitstunCooldown > 0f;
        }
    }
    private bool hitstunning = false;
    float hitstunCooldown = 0f;

    public UnityEvent onHurt;
    public UnityEvent onDeath;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        collide = GetComponent<Collider>();

        currHealth = maxHealth;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (dead)
        {
            if (animator) animator.SetBool("hurt", false);
        }

        // Finish hitstunning
        if (hitstunning && hitstunCooldown <= 0f)
        {
            if (animator) animator.SetBool("hurt", false);

            hitstunning = false;
        }

        // Timer
        hitstunCooldown -= Time.deltaTime;
    }

    public void SetKinematic(bool s)
    {
        if (!rb) return;

        if (!s) rb.isKinematic = s;
        rb.collisionDetectionMode = s ? CollisionDetectionMode.ContinuousSpeculative : CollisionDetectionMode.ContinuousDynamic;
        if(s) rb.isKinematic = s;
    }

    public bool IsDead()
    {
        return dead;
    }

    public virtual void TakeDamage(DamageData data, Vector3 forward, Fighter owner)
    {
        if (invincible) return;

        currHealth -= data.GetDamage(); // Lose health from damage

        if (animator) animator.SetBool("hurt", true);

        hitstunning = true;
        if (data.GetDamage() > 0)
        {
            onHurt.Invoke();
        }

        forward.y = 1f; // For knockback

        // Death
        if (currHealth <= 0)
        {
            currHealth = 0;

            rb.velocity = Vector3.zero; // Cancel velocity

            if (!dead)
            {
                Die();
            }

            return;
        }

        if (data.GetDamage() > 0)
        {
            rb.velocity = Vector3.zero; // Cancel velocity

            Knockback(data, forward);
        }
    }

    public virtual void TakeHeals(int amount)
    {
        currHealth += amount;

        if (currHealth > maxHealth) currHealth = maxHealth;

        //print("TakeHeals() Healed by " + amount + "!!!");

    }

    protected virtual void Knockback(DamageData data, Vector3 forward)
    {
        if (dead) return;

        Vector3 dir = data.GetForce() * forward;
        rb.AddForce(dir, ForceMode.Impulse);

        Debug.DrawRay(rb.position, dir, Color.green, 2f);

        Hitstun(data.GetHitstun());
    }

    protected virtual void Hitstun(float t)
    {
        if (t <= 0f) return;

        // Flash effect? nah
        hitstunCooldown = t;
    }

    public virtual void Die()
    {
        dead = true;

        onDeath.Invoke();

        if(destroyOnDeath) Destroy();
    }
    public virtual void Destroy()
    {
        Destroy(0f);
    }
    public void Destroy(float t)
    {
        Destroy(gameObject, t);
    }
    public void Disable()
    {
        collide.enabled = false;
        MeshRenderer m = GetComponent<MeshRenderer>();
        if (m) m.enabled = false;
        this.enabled = false;
    }

    public Collider GetCollide()
    {
        return collide;
    }

    // Sounds!
    public void PlaySound(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
