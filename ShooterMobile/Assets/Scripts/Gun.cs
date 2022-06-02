using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform muzzle;              // ObjectPooler implemented. 
    //[SerializeField] private Projectile projectile;       // So we dont need projectile here anymore. (it's in objectPooler)
    [SerializeField] private Transform chamber;             // shell ejection point
    //[SerializeField] private Transform shell;             // we dont need shell prefab too 

    [Header("Gun Properties")]
    [SerializeField] private float msBetweenShots = 100;
    [SerializeField] private float muzzleVelocity = 35;
    [SerializeField] int bulletsPerMagazine = 7;
    [SerializeField] private float reloadTime = .8f;
    [SerializeField] private Vector2 gunRecoilMinMax = new Vector2(.5f, 2f);
    //[SerializeField] private Vector2 gunRecoilAngleMinMax = new Vector2(3f, 10f);

    [Header("Gun Effects")]
    [SerializeField] private float recoilMoveTime = .1f;
    //[SerializeField] private float recoilRotationTime = .1f;
    [SerializeField] private AudioClip shootAudioClip;
    [SerializeField] private AudioClip reloadAudioClip;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float laserRange = 10f;

    float nextShotTime;
    [HideInInspector]
    public int bulletsRemainingInMagazine { get; private set; }
    bool reloading;
    Vector3 recoilSmoothDampVelocity;
    float recoilAngleSmoothDampVelocity;
    float recoilAngle;
    ObjectPooler objectPooler;

    private void Start()
    {
        bulletsRemainingInMagazine = bulletsPerMagazine;
        objectPooler = ObjectPooler.instance;
        laserLine = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        laserLine.SetPosition(0, muzzle.position);
        RaycastHit hit;
        if (Physics.Linecast(muzzle.position, muzzle.position + muzzle.forward * laserRange, out hit, enemyMask))
        {
            laserLine.SetPosition(1, hit.point);
            laserLine.startColor = Color.red;
        }
        else
        {
            laserLine.SetPosition(1, muzzle.position + muzzle.forward * laserRange);
            laserLine.startColor = Color.green;
        }
    }

    private void LateUpdate()
    {
        //animate recoil in late update cause aim method keeps gun in the same rotation each update
        //...
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveTime);
        
        //recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilAngleSmoothDampVelocity, recoilRotationTime);
        //transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if (!reloading && bulletsRemainingInMagazine == 0)
        {
            reload();
        }
    }

    public void shoot() {
        if (!reloading && bulletsRemainingInMagazine  > 0 && Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;
            muzzleFlash.Play();

            //Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            Projectile newProjectile = objectPooler.spawnFromPool("Projectile", muzzle.position, muzzle.rotation).GetComponent<Projectile>();
            newProjectile.setBulletSpeed(muzzleVelocity);
            bulletsRemainingInMagazine--;

            //Instantiate(shell, chamber.position, chamber.rotation);
            objectPooler.spawnFromPool("Shell", chamber.position, chamber.rotation);

            transform.localPosition -= Vector3.forward * Random.Range(gunRecoilMinMax.x, gunRecoilMinMax.y);

            //recoilAngle += Random.Range(gunRecoilAngleMinMax.x, gunRecoilAngleMinMax.y);
            //recoilAngle = Mathf.Clamp(recoilAngle, 0, 35);
            AudioManager.instance.playAudio(shootAudioClip, transform.position, .25f);


        }
    }

    public void aim(Vector3 aimPoint) {
        if (!reloading)
        {
            transform.LookAt(aimPoint);
        }
    }

    public void reload() {
        if (!reloading && bulletsRemainingInMagazine != bulletsPerMagazine)
        {
            AudioManager.instance.playAudio(reloadAudioClip, transform.position, .6f);
            StartCoroutine(animateReload());
        }
    }

    IEnumerator animateReload(){
        reloading = true;
        laserLine.enabled = false;
        yield return new WaitForSeconds(.5f);

        float reloadSpeed = 1f / reloadTime;
        float percent = 0;
        Vector3 initialRotation = transform.localEulerAngles;
        float maxReloadAngle = 45;

        //rotating up and rotating down again 0----> 1 and 1----->0 same thing in the Enemy attacks
        while (percent <= 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = 4 * (-percent * percent + percent);
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRotation + Vector3.left * reloadAngle;

            yield return null;
        }

        bulletsRemainingInMagazine = bulletsPerMagazine;
        laserLine.enabled = true;
        reloading = false;
    }
}
