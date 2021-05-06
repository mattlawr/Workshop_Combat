using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Fighter
{
    [Space]

    public Vector2 moveSpeed = new Vector2(3f, 2f);

    protected override Vector3 GetVelocity() {
        return moveDir / Time.deltaTime;
    }
    private Vector2 input = Vector2.zero;
    private Vector3 moveDir;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (dead)
        {
            base.Update();

            return;
        }

        // Check references
        if (!rb) return;

        // Input
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // Allow movement only when not attacking
        if (!InAttack() && !hitstun)
        {
            // Movement --
            moveDir = (Vector3.right * input.x) + (Vector3.forward * input.y);    // Raw
            moveDir = moveDir.normalized * Time.deltaTime;   // Normalized input per second
            moveDir.x *= moveSpeed.x;
            moveDir.z *= moveSpeed.y;

            Vector3 f = rb.position + moveDir;
            rb.MovePosition(f); // Moves player position

            if (moveDir != Vector3.zero)
            {
                rb.rotation = Quaternion.LookRotation(moveDir / Time.deltaTime, Vector3.up);
            }

            // --
        }

        // Attacks via Input
        // TODO

        base.Update();
    }


    protected override bool Attack(int index)
    {
        bool a = base.Attack(index);

        // Do special case stuff in here if desired

        return a;
    }

    public override void DidHit()
    {
        // Increase score, combo counter, anything

        base.DidHit();
    }

    public override void TakeDamage(DamageData data, Vector3 forward, Fighter owner)
    {
        base.TakeDamage(data, forward, owner);

        // Update UI
    }

    public override void TakeHeals(int amount)
    {
        base.TakeHeals(amount);

        // Update UI
    }

    public override void Die()
    {
        base.Die(); // Set dead = true and other stuff

        // Call a game over method here
    }

}
