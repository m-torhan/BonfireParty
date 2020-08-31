using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponScript : MonoBehaviour
{
    public Sprite weaponIcon;

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

    private bool sprint = false;
    private float sprintTime = 0.0f;

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
        sprint = player.GetComponent<PlayerMovement>().GetIsRunning();

        if (sprint && sprintTime < 0.2)
        {
            sprintTime += Time.deltaTime;
        }
        if (!sprint && sprintTime > 0)
        {
            sprintTime -= Time.deltaTime;
        }

        if (sprintTime > 0)
        {
            gameObject.transform.localEulerAngles = new Vector3(30.0f, -20.0f, 0.0f) * 5 *sprintTime;
            gameObject.transform.localPosition = new Vector3(0.0f, -0.3f, 0.0f) * 5 * sprintTime;
        }
        else
        {
            gameObject.transform.localEulerAngles = Vector3.zero;
            gameObject.transform.localPosition = Vector3.zero;
        }

        if (shotTime >= 0)
        {
            shotTime -= Time.deltaTime;
        }

        if (drawTime >= 0)
        {
            drawTime -= Time.deltaTime;
            if (drawTime > 0)
            {
                gameObject.transform.localEulerAngles = new Vector3(30f * (1 - Mathf.Cos((Mathf.PI * drawTime) / (2 * weaponProperties.drawTime))), 0f, 0f);
                gameObject.transform.localPosition = new Vector3(0f, -0.5f * (1 - Mathf.Cos((Mathf.PI * drawTime) / (2 * weaponProperties.drawTime))), 0f);
            }
            else
            {
                gameObject.transform.localEulerAngles = Vector3.zero;
                gameObject.transform.localPosition = Vector3.zero;
            }
        }

        if (reloadTime >= 0)
        {
            reloadTime -= Time.deltaTime;
            if (reloadTime > 0.9f * weaponProperties.reloadTime || reloadTime < 0.1f * weaponProperties.reloadTime)
            {
                gameObject.transform.localEulerAngles = new Vector3(30f * Mathf.Sin((Mathf.PI * reloadTime) / (0.2f * weaponProperties.reloadTime)), 0f, 0f);
                gameObject.transform.localPosition = new Vector3(0f, -0.5f * Mathf.Sin((Mathf.PI * reloadTime) / (0.2f * weaponProperties.reloadTime)), 0f);
            }
            else
            {
                gameObject.transform.localEulerAngles = new Vector3(30f, 0f, 0f);
                gameObject.transform.localPosition = new Vector3(0f, -0.5f, 0f);
            }
            if (reloadTime < 0)
            {
                magazineAmmo = player.GetComponent<PlayerMovement>().PutAmmoToWeapon(weaponProperties.magazineSize);
                gameObject.transform.localEulerAngles = Vector3.zero;
                gameObject.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void Shot()
    {
        if (shotTime < 0 && reloadTime < 0 && drawTime < 0 && magazineAmmo > 0 && barrelEnd != null && sprintTime <= 0)
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

    public float GetMagazineAmmo()
    {
        return magazineAmmo;
    }

    public float GetReloadTime()
    {
        return reloadTime;
    }
}
