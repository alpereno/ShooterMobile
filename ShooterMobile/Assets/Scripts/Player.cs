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

        if (Input.GetMouseButton(0))
        {
            createAimRay();
        }
    }

    private void createAimRay()
    {
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        if (groundPlane.Raycast(ray, out distance))
        {
            bool targetDetected;
            Vector3 point = ray.GetPoint(distance);
            playerController.lookAt(point);

            // I've added weaponheight to y axis cause crosshair should be same height 
            Vector3 crosshairPoint = new Vector3(point.x, point.y + gunController.getWeaponHeight / 2, point.z);
            crosshair.transform.position = crosshairPoint;
            targetDetected = crosshair.detectTarget(ray, distance);
            if (targetDetected)
            {
                gunController.shoot();
            }
            // when distance between lookpoint and player's pos. is less than [1- 1.2] gun(s) behaveing weirdly
            // so if the distance greater than 1.1, aim that point   (1.1 * 1.1 = 1.21)
            //print((new Vector2(crosshairPoint.x, crosshairPoint.z) - new Vector2(transform.position.x, transform.position.z)).magnitude);
            if ((new Vector2(crosshairPoint.x, crosshairPoint.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1.21)
            {
                gunController.aim(crosshairPoint);
            }
        }
    }

    private void moveInput()
    {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized;
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
