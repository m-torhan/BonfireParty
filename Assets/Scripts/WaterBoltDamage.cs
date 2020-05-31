using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBoltDamage : MonoBehaviour
{
    public float damage = 1.0f;
    public float startForce = 2.0f;

    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(transform.forward * startForce);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(rigidbody.velocity, transform.up);
    }

    void OnTriggerEnter(Collider collider)
    {
        //Destroy(gameObject);
    }
}
