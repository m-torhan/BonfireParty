using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public WeaponProperties weaponProperties { private set; get; } = new WeaponProperties(2.0f, 1.0f, 0.5f, 5.0f, 1.0f, 2.0f, 10.0f);

    public GameObject barrelEnd;

    private float reloadTime = 0f;
    private float drawTime = 0f;
    private float shotTime = 0f;
    private float magazineAmmo = 0f;
    private float ammo = 0f;

    [SerializeField]
    private GameObject waterBoltPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shotTime >= 0)
        {
            shotTime -= Time.deltaTime;
        }

        if (drawTime >= 0)
        {
            drawTime -= Time.deltaTime;
        }

        if (reloadTime >= 0)
        {
            reloadTime -= Time.deltaTime;
            if (reloadTime < 0)
            {
                magazineAmmo = ammo >= weaponProperties.magazineSize ? weaponProperties.magazineSize : ammo;
            }
        }
    }

    public void Shot()
    {
        if (shotTime < 0 && reloadTime < 0 && drawTime < 0 && magazineAmmo > 0 && barrelEnd != null)
        {
            Instantiate(waterBoltPrefab, barrelEnd.transform.position, barrelEnd.transform.rotation);
            shotTime = weaponProperties.rateOfFire;
            magazineAmmo -= weaponProperties.ammoUsage;
        }
    }

    public void Reload()
    {
        if (reloadTime < 0 && barrelEnd != null)
        {
            reloadTime = weaponProperties.reloadTime;
        }
    }

    public void Draw()
    {
        drawTime = weaponProperties.drawTime;
    }
}
