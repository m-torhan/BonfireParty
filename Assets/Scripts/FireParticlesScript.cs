using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticlesScript : MonoBehaviour
{
    private float hp = 10f;

    [SerializeField]
    private ParticleSystem redFire;
    [SerializeField]
    private ParticleSystem orangeFire;
    [SerializeField]
    private ParticleSystem smoke;

    ParticleSystem.MainModule redFireMain;
    ParticleSystem.MainModule orangeFireMain;
    ParticleSystem.MainModule smokeMain;

    // Start is called before the first frame update
    void Start()
    {
        redFireMain = redFire.main;
        orangeFireMain = orangeFire.main;
        smokeMain = smoke.main;
    }

    // Update is called once per frame
    void Update()
    {
        redFireMain.startLifetime = hp/10f;
        orangeFireMain.startLifetime = hp / 10f;
        smokeMain.startLifetime = hp / 2f + 5f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WaterProjectile"))
        {
            if (hp > 0)
            {
                hp -= other.gameObject.GetComponent<WaterProjectileScript>().damage;
            }
            Destroy(other);
        }
    }
}
