using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds information about an attack animation and related hitboxes
// To be contained in an array in Fighter
[System.Serializable]
public class AttackData
{
    [SerializeField] string animation = "attack";   // Holds attack events

    [Space]

    [SerializeField] Hitbox[] hitboxes;

    public string GetAnimationTag() { return animation; }

    public void Hitbox(int i, Fighter ownedBy)
    {
        if (i < 0 || i >= hitboxes.Length) return;

        Hitbox h = hitboxes[i];
        h.Activate(ownedBy);
    }

    public void HitboxEnd(int i)
    {
        if (i < 0 || i >= hitboxes.Length) return;

        Hitbox h = hitboxes[i];
        h.Deactivate();
    }

    /// <summary>
    /// Deactivates the attack.
    /// </summary>
    public void Finish()
    {
        foreach(Hitbox h in hitboxes)
        {
            h.Deactivate();
        }
    }
}