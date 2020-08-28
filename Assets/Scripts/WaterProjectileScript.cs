using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterProjectileScript : MonoBehaviour
{
    public float damage;
    public float size = 1.0f;

    [SerializeField]
    private GameObject puddlePrefab;

    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        size = damage;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(rigidbody.velocity, transform.up);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Puddle"))
        {
            other.GetComponent<PuddleScript>().AddWater(size * size * size);
        }
        if (other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
            RaycastHit hit;
            other.Raycast(new Ray(transform.position, transform.forward), out hit, 10);
            GameObject puddle = Instantiate(puddlePrefab, new Vector3(hit.point.x, 0.02f, hit.point.z), Quaternion.identity);
            puddle.GetComponent<PuddleScript>().volume = size * size * size;
        }
    }
    /*
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(rigidbody.velocity.magnitude);
        if (rigidbody.velocity.magnitude < 0.1f)
        {
            Destroy(gameObject);
        } else if (other.gameObject.tag != "Player" && rigidbody.velocity.magnitude > 0.5f && size > 0.1f)
        {
            Vector3 dir = other.transform.position - transform.position;

            dir.Normalize();

            int newBoltsCount = (int)Random.Range(3, 5);
            float newBoltsSize = size * .6f / newBoltsCount;
            for (int i = 0; i < newBoltsCount; ++i)
            {
                Vector3 newBoltDir = dir + new Vector3(Random.Range(0f, .5f), Random.Range(0f, .5f), 0f);
                Quaternion newBoltRot = Quaternion.LookRotation(newBoltDir, transform.up);
                GameObject newBolt = Instantiate(waterBoltPrefab, transform.position + newBoltDir, newBoltRot);
                WaterBoltDamage newBoltScript = newBolt.GetComponent<WaterBoltDamage>();
                newBoltScript.size = newBoltsSize;
                newBolt.GetComponent<Rigidbody>().velocity = newBoltDir * rigidbody.velocity.magnitude * newBoltsSize;
            }
            size *= .4f;
        }
    }*/
}
