using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Container of data to deal damage to Entities
// To be held inside a hitbox
[System.Serializable]
public class DamageData
{
    [SerializeField] int damage = 10;   // Raw
    [SerializeField] float hitStun = 0.2f;
    [SerializeField] float hitFreeze = 0.1f;
    [SerializeField] float knockback = 1f;
    [SerializeField] Vector3 knockbackDirection = Vector3.forward;   // Z axis is forward


    public int GetDamage() { return damage; }
    public float GetHitstun() { return hitStun; }
    public float GetHitFreeze() { return hitFreeze; }
    public float GetKnockback() { return knockback; }
    public Vector2 GetKnockbackDirection() { return knockbackDirection; }
    public Vector2 GetForce()
    {
        return GetKnockbackDirection().normalized * GetKnockback();
    }

    public DamageData(int damage, float hitStun)
    {
        this.damage = damage;
        this.hitStun = hitStun;
    }
    public DamageData(int damage, float hitStun, Vector2 dir, float k)
    {
        this.damage = damage;
        this.hitStun = hitStun;
        this.knockbackDirection = dir;
        knockback = k;
    }
}