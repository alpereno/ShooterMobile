using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity
{
    public static event System.Action onDeathStatic;
    public enum State {Idle, Chasing, Attacking};
    [SerializeField] ParticleSystem damageEffect;
    [SerializeField] private AudioClip[] attackAudioClips;
    [SerializeField] private AudioClip[] deathAudioClips;
    State currentState;
    NavMeshAgent navMeshAgent;
    Transform target;
    LivingEntity targetEntity;

    Material material;
    Color originalColor;
    Color attackColor = Color.red;

    float damage = 1;
    float attackDistanceThreshold = 1f;
    float timeBetweenAttacks = 1;
    float nextAttackTime;
    float collisionRadius;
    float targetCollisionRadius;
    int randomClipNumber;
    bool targetAlive;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        // player could be dead when the enemy instantiated so check it
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            targetAlive = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

        }
    }

    protected override void Start()
    {
        base.Start();

        if (targetAlive)
        {
            collisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            currentState = State.Chasing;
            targetEntity.onDeath += onTargetDeath;
            StartCoroutine(updatePath());
        }
        randomClipNumber = UnityEngine.Random.Range(0, 2);
    }

    private void Update()
    {
        //attack to the target if the distance from enemy to target less than attack distance 
        //sqrt operation is costly which is why I'm not using Vector3.Distance.  So just compare sqr distance
        //or you can use (transform.pos - target.pos).sqrMagnitude           below
        if (targetAlive)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDistanceToTarget = Utility.calculateDistanceSqr(transform.position, target.position);
                // attack distance threshold should be edge of this 2 colliders
                // not center of the player and center of the enemy so add them 2 radius variable
                if (sqrDistanceToTarget < Mathf.Pow(attackDistanceThreshold + collisionRadius + targetCollisionRadius, 2))
                {
                    //dont want to attack every frame use timer
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(attack());
                }
            }
        }
    }

    public override void takeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= health)       // Particle Effect will instantiate when enemy die not each shoot
        {
            if (onDeathStatic != null)
            {
                onDeathStatic();
            }
            // enemy death audio
            Destroy(Instantiate(damageEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject,
                damageEffect.main.startLifetime.constant);
            AudioManager.instance.playAudio(deathAudioClips[randomClipNumber], transform.position, 1);
        }
        base.takeHit(damage, hitPoint, hitDirection);
    }

    void onTargetDeath() {
        targetAlive = false;
        currentState = State.Idle;
    }

    // This method is called before Start method so navmeshagent would be null. To fix this problem in awake method will run to assign navmeshagent
    public void setEnemyProperties(float enemyHealth, float enemyMovementSpeed, float hitsNumberToKillPlayer, Color skinColor) {
        startingHealth = enemyHealth;
        navMeshAgent.speed = enemyMovementSpeed;
        if (targetAlive)
        {
            damage = Utility.roundNumber(targetEntity.startingHealth / hitsNumberToKillPlayer);
            //print("Damage to each enemy in this wave " + damage);
        }
        damageEffect.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);
        material = GetComponent<Renderer>().material;
        material.color = skinColor;
        originalColor = material.color;
    }

    //animating lunge
    IEnumerator attack() {
        //when enemy attacking shouldn't move to target
        currentState = State.Attacking;
        navMeshAgent.enabled = false;
        //for lunge store enemy object starting attack pos then lunge to attack pos then go back starting pos
        //not inside of player just little bit of like biting from border
        Vector3 startPosition = transform.position;
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - directionToTarget * collisionRadius;
        float attackSpeed = 3;
        float percent = 0;
        material.color = attackColor;
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.takeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            //percent should be start pos go to attack pos and BACK to start pos so value be 0 to 1 and back to 0 again
            //when x=0 y=0, x=1 y=0, x=1/2 y=1  so solve the inspired by y=a(x-x1)(x-x2) equations a=-4
            //so parabola equation working y = 4(-x^2+x)            when percent 1/2 apply damage
            float interpolation = 4*(-percent * percent + percent);
            transform.position = Vector3.Lerp(startPosition, attackPosition, interpolation);
            yield return null;
        }
        material.color = originalColor;
        currentState = State.Chasing;
        navMeshAgent.enabled = true;
        AudioManager.instance.playAudio(attackAudioClips[randomClipNumber], transform.position, 1);
    }

    IEnumerator updatePath() {
        //enemy doesn't need to get into target exact position
        //pathfind to outside of target pos. collision radius
        float refreshRate = .25f;

        while (targetAlive)
        {
            if (currentState == State.Chasing)
            {
                // I don't wanna to the enemy to get inside the player
                // so use the direction * collider radius a little bit of more distance
                // enemy will be stop when it's near not inside
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * (collisionRadius + targetCollisionRadius +attackDistanceThreshold/3);
                if (!dead)
                {
                    navMeshAgent.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
