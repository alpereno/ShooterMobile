using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public event System.Action onDeath;
    public float startingHealth = 10;
    protected float health;
    protected bool dead;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public virtual void takeDamage(float damage) {
        health -= damage;

        if (health <= 0 && !dead)
        {
            die();
        }
    }

    public virtual void takeHit(float damage, Vector3 hitPoint, Vector3 hitDirection) {
        //do some stuff with hit variable (Raycasthit)  (instead of hit variable, vector3 hitPoint and vector3 hitDirection same logic)
        //detect hit point instantiate particle 
        takeDamage(damage);
    }

    [ContextMenu ("Self Destruct")]
    protected virtual void die()
    {
        health = 0;
        dead = true;
        if (onDeath != null)
        {
            onDeath();
        }
        GameObject.Destroy(gameObject);
    }

    public float getHealth() {
        return health;
    }
}
