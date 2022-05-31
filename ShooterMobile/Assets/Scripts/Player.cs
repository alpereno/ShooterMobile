using System;
using UnityEngine;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : LivingEntity
{
    PlayerController playerController;
    GunController gunController;
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 6;
    [SerializeField] private Crosshair crosshair;
    [SerializeField] private Animator anim;
    [SerializeField] private Joystick joystick;
    [SerializeField] private Joystick Rotatejoystick;
    [SerializeField] private LayerMask enemyMask;
    Camera viewCamera;

    private void Awake()
    {
        FindObjectOfType<Spawner>().onNewWave += onNewWave;
    }

    protected override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    void Update()
    {
        moveInput();
        weaponInput();
        checkPlayerYPos();
    }

    void onNewWave(int waveNumber) {
        health = startingHealth;
        gunController.reload();
        //gunController.equipWeapon(gunIndex)...    waveNumber maybe
    }

    private void weaponInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.reload();
        }
        createAimRay();
    }

    private void createAimRay()
    {
        //      OLD VERSION
        //      In the old version Crosshair Point was based on where it was touched
        //      but now based on player's forward

        //    Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        //    Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        //    float distance;
        //    if (groundPlane.Raycast(ray, out distance))
        //    {
        //        bool targetDetected;
        //        Vector3 point = ray.GetPoint(distance);
        //        playerController.lookAt(point);
        //        //Debug.DrawLine(ray.origin, point, Color.red);

        //        // I've added weaponheight to y axis cause crosshair should be same height 
        //        Vector3 crosshairPoint = new Vector3(point.x, point.y + gunController.getWeaponHeight / 2, point.z);
        //        crosshair.transform.position = crosshairPoint;
        //        targetDetected = crosshair.detectTarget(ray, distance);
        //        if (targetDetected)
        //        {
        //            gunController.shoot();
        //        }
        //        // when distance between lookpoint and player's pos. is less than [1- 1.2] gun(s) behaveing weirdly
        //        // so if the distance greater than 1.1, aim that point   (1.1 * 1.1 = 1.21)
        //        //print((new Vector2(crosshairPoint.x, crosshairPoint.z) - new Vector2(transform.position.x, transform.position.z)).magnitude);
        //        if ((new Vector2(crosshairPoint.x, crosshairPoint.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1.21)
        //        {
        //            gunController.aim(crosshairPoint);
        //        }
        //    }
        //}
        //if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        //{}

        // This Raycast is just managing the shooting
        // Shoots automatically if there is an enemy in front of the player within a certain distance

        // It's NOT firing func. just calling shoot func.

        Ray ray = new Ray(gunController.getWeaponHoldPos, gunController.getWeaponForward);
        RaycastHit hit;
        float distance = 11f;
        if (Physics.Raycast(ray, out hit, distance, enemyMask, QueryTriggerInteraction.Collide))
        {            
            gunController.shoot();
        }
        //Debug.DrawLine(gunController.getWeaponHoldPos, gunController.getWeaponHoldPos + gunController.getWeaponForward * 11, Color.magenta);
    }

    private void moveInput()
    {
        //Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveInput = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        Vector3 moveVelocity = moveInput.normalized;

        //if (moveVelocity != Vector3.zero)
        //{
        //    transform.forward = moveVelocity;
        //}

        if (Rotatejoystick.Direction != Vector2.zero)
        {
            transform.forward = new Vector3(Rotatejoystick.Direction.x, transform.forward.y, Rotatejoystick.Direction.y);
        }

        if (Input.GetKey(KeyCode.LeftShift))
            moveVelocity *= runSpeed;
        else moveVelocity *= walkSpeed;
        //FPS game move direction (relative to local coordinate system)
        //moveVelocity = transform.TransformDirection(moveVelocity);
        playerController.setVelocity(moveVelocity);
        animating(moveInput.x, moveInput.z);
    }

    void checkPlayerYPos() {
        if (transform.position.y < -10)
        {
            takeDamage(health);
        }
    }

    void animating(float horizontal, float vertical) {
        bool walking = horizontal != 0f || vertical != 0f;
        anim.SetBool("IsWalking", walking);
    }

    protected override void die() {
        FindObjectOfType<AudioManager>().playAudio("Player Death");
        base.die();
    }
}
