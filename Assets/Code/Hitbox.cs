using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Hitboxes will be referenced inside AttackData
[RequireComponent(typeof(Collider))]
/// <summary>
/// Handles physical attacks, with dedicated damage properties.
/// </summary>
public class Hitbox : MonoBehaviour
{
    public DamageData data;
    private Fighter owner;

    //public Projectile projectile = null;

    [Space]

    // Which direction is forward on the transform? (usually Z axis)
    public Vector3 hitboxForward = new Vector3(0f, 0f, 1f);

    public bool multiHit = false;

    public float timeActive = 0f; // How long before we auto-deactivate? (disable with 0)

    // Timers
    float timer = 0f;
    float timeLastHit = 0f;
    public float rate = 0.2f;   // How often hits can register
    float rateTimer = 0f;

    public Collider colliderIgnore;

    // Custom events
    public UnityEvent onActivate;
    public UnityEvent onHit;    // for sounds, effects
    public UnityEvent onDeactivate;

    bool activated = false;
    Entity lastHit = null;
    protected Collider c;

    private void Awake()
    {
        if (onHit == null)
        {
            onHit = new UnityEvent();
        }
    }

    private void Start()
    {
        c = GetComponent<Collider>();

        if (colliderIgnore && c) Physics.IgnoreCollision(c, colliderIgnore, true);
    }

    private void Update()
    {
        if (!activated)
            return;

        timer += Time.deltaTime;
        rateTimer += Time.deltaTime;

        if (activated && timer > timeActive && timeActive != 0f)
        {
            Deactivate();
        }

        if (timer - timeLastHit > 1f && timeActive <= 0f)
        {
            lastHit = null;
        }
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        Entity e = other.GetComponent<Entity>();
        if (e) Hit(e, GetForward(e));
    }

    private void OnTriggerStay(Collider other)
    {
        if (multiHit && rateTimer > rate)
        {
            Entity e = other.GetComponent<Entity>();
            if (e) Hit(e, GetForward(e));
        }
    }

    // Used to check for direction of knockback
    Vector3 GetForward(Entity i)
    {
        return transform.TransformDirection(hitboxForward);
    }

    public Vector3 GetHitboxPosition()
    {
        return transform.position;
    }

    public Entity GetLastHit()
    {
        return lastHit;
    }

    #region State Methods
    /// <summary>
    /// Begin doing hitbox collisions
    /// </summary>
    public void Activate(Fighter owner)
    {
        if (!activated)
        {
            onActivate.Invoke();
        }

        activated = true;

        this.owner = owner;

        lastHit = null;
        timer = 0f;

        if(!c) c = GetComponent<Collider>();

        c.enabled = true;
    }

    /// <summary>
    /// Stop doing hitbox collisions
    /// </summary>
    public void Deactivate()
    {
        if (activated)
        {
            onDeactivate.Invoke();
        }

        if (c) c.enabled = false;
        activated = false;
    }

    public bool IsActive()
    {
        return activated;
    }

    // Deal damage/knockback to target
    public void Hit(Entity target, Vector3 direction)
    {
        if (!target) return;
        if (target == lastHit && !multiHit) return;

        rateTimer = 0f;

        lastHit = target;

        // Tell owner that it got a hit
        if (owner) owner.DidHit();

        // Send damage data to entity
        target.TakeDamage(data, direction, owner);

        onHit.Invoke();
        timeLastHit = timer;
    }

    #endregion
    
}