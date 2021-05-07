using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Entity
{
    public AttackData[] attacks;
    private int attackCurrent = -1;

    public float turnSpeed = 20f;

    private int attackBuffer = -1;
    float bufferTimer = 0f;

    Collider collideIgnore;

    bool moving = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Vector3 velocity = GetVelocity();

        if (animator && !dead && rb)
        {
            animator.SetFloat("speed", velocity.normalized.magnitude); // Walk animation

            animator.SetFloat("yspeed", rb.velocity.y); // Fall animation
        }
        else if (animator)
        {
            animator.SetFloat("speed", 1f);
        }

        // Trigger buffered attack
        if (bufferTimer > 0f && !dead)
        {
            bufferTimer -= Time.deltaTime;
            if (attackBuffer >= 0 && !InAttack() && !hitstun)
            {
                //print("buffered attack");
                Attack(attackBuffer);
                bufferTimer = 0f;
            }
        }



    }

    public override void Die()
    {
        if (animator) animator.SetBool("dead", true);

        base.Die();
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    protected override void Hitstun(float t)
    {
        CancelAttack();

        base.Hitstun(t);
    }

    // Our old friend the Attack() method
    /// <summary>
    /// Attempts to begin (or buffer) an AttackData animation.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>True if the requested attack starts, False otherwise.</returns>
    protected virtual bool Attack(int index)
    {
        if (Time.deltaTime == 0) return false;    // Paused

        if (hitstun) return false;

        if (InAttack())
        {
            // Buffer
            //print("b");
            attackBuffer = index;
            bufferTimer = 0.2f;
            return false;
        }

        if (dead) return false;

        if (!animator) return false;

        if (index < 0 || index >= attacks.Length) return false;

        animator.ResetTrigger("cancel");

        attackCurrent = index;  // Set the current attack
        AttackData a = attacks[attackCurrent];

        animator.SetTrigger(a.GetAnimationTag());   // Play the attack animation!

        SetAnimMove();

        attackBuffer = -1;  // Clear the attack buffer
        return true;
    }

    public override void TakeDamage(DamageData data, Vector3 forward, Fighter owner)
    {
        base.TakeDamage(data, forward, owner);
    }

    // Used to tell this fighter to not collide with another Entity
    public void SetCollide(Entity e, bool ignores)
    {
        Physics.IgnoreCollision(GetCollide(), e.GetCollide(), ignores);
        collideIgnore = e.GetCollide();
    }
    public void SetCollide(bool ignores)
    {
        if (collideIgnore) Physics.IgnoreCollision(GetCollide(), collideIgnore, false);

        collideIgnore = null;
    }

    public virtual void DidHit()
    {
        // Called from a hitbox owned by this fighter. Can be useful for tracking hits!
    }

    /// <summary>
    /// Returns true if we are currently in an attack.
    /// </summary>
    protected bool InAttack()
    {
        return attackCurrent >= 0;
    }

    protected AttackData GetCurrentAttack()
    {
        return attacks[attackCurrent];
    }

    // When our attack is done, we should mark it as finished
    void FinishAttack()
    {
        if (!InAttack()) return;

        GetCurrentAttack().Finish();
        StopAnimMove();

        attackCurrent = -1;
    }

    // When we get hit during an attack, we should stop attacking!
    protected void CancelAttack()
    {
        if (!InAttack()) return;

        // Setup to "cancel" the current animation in the animator
        if (animator) animator.SetTrigger("cancel");

        FinishAttack();
    }


    #region Animation Events
    // These are called from attack animation events!
    public void HitboxEvent(int index = 0)
    {
        if (!InAttack()) return;

        GetCurrentAttack().Hitbox(index, this);
    }

    public void HitboxEndEvent(int index = 0)
    {
        if (!InAttack()) return;

        GetCurrentAttack().HitboxEnd(index);
    }

    // These are called from attack animation files!
    public void AttackEndEvent()
    {
        if (!InAttack()) return;

        FinishAttack();
    }
    #endregion

    // For allowing Animator + Rigidbody movement
    private void OnAnimatorMove()
    {
        if (!animator || !rb || Time.deltaTime == 0f) return;

        if (!moving) return;

        // Do velocity changes
        //Debug.DrawRay(rb.position, animator.deltaPosition, Color.red, 1f);

        Vector3 f = rb.position + animator.deltaPosition;
        rb.MovePosition(f);
    }

    public void SetAnimMove()
    {
        moving = true;
    }

    public void StopAnimMove()
    {
        moving = false;
    }
}
