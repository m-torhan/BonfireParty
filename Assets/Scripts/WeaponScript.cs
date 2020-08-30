using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public WeaponProperties weaponProperties;

    public GameObject barrelEnd;

    private float reloadTime = 0f;
    private float drawTime = 0f;
    private float shotTime = 0f;
    private float magazineAmmo = 0f;

    private Vector3 prevPos;
    private Vector3 velocity;

    [SerializeField]
    private GameObject waterProjectilePrefab;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        prevPos = transform.position;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    // Update is called once per frame
    void Update()
    {
        velocity = (transform.position - prevPos) / Time.deltaTime;
        prevPos = transform.position;

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
                magazineAmmo = player.GetComponent<PlayerMovement>().PutAmmoToWeapon(weaponProperties.magazineSize);
            }
        }
    }

    public void Shot()
    {
        if (shotTime < 0 && reloadTime < 0 && drawTime < 0 && magazineAmmo > 0 && barrelEnd != null)
        {
            float u, v, S;

            do
            {
                u = 2.0f * UnityEngine.Random.value - 1.0f;
                v = 2.0f * UnityEngine.Random.value - 1.0f;
                S = u * u + v * v;
            }
            while (S >= 1.0f);

            // standard normal distribution
            float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

            GameObject projectile = Instantiate(waterProjectilePrefab, barrelEnd.transform.position, barrelEnd.transform.rotation);
            projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * weaponProperties.projectileSpeed + velocity;
            projectile.GetComponent<WaterProjectileScript>().damage = weaponProperties.damageAvg + std * weaponProperties.damageStdDev;

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
