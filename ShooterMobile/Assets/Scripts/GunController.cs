using UnityEngine;

public class GunController : MonoBehaviour
{
    // Manage things like equiping weapon, shooting etc.
    [SerializeField] private Transform weaponHold;
    [SerializeField] private Gun startingGun;
    Gun equippedGun;
    public float getWeaponHeight
    {
        get { return weaponHold.position.y; }
    }

    public int getRemainingBullets
    {
        get { return equippedGun.bulletsRemainingInMagazine; }
    }

    private void Start()
    {
        if (startingGun != null)
        {
            equipGun(startingGun);
        }    
    }

    void equipGun(Gun gunToEquip) {
        if (equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equippedGun.transform.parent = weaponHold;
    }

    //public void equipGun(int gunIndex) {
    //    equipGun(guns[gunIndex]);
    //}

    public void shoot() {
        if (equippedGun != null)
        {
            equippedGun.shoot();
        }
    }


    // OBSOLETE: when existing crosshair instead of laser line this func is aiming gun to crosshair point
    public void aim(Vector3 aimPoint) {
        if (equippedGun != null)
        {
            equippedGun.aim(aimPoint);
        }
    }

    public void reload(){
        if (equippedGun != null)
        {
            equippedGun.reload();
        }
    }
}
