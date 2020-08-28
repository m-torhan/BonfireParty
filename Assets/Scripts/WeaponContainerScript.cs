using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponContainerScript : MonoBehaviour
{
    public Vector3 velocity;
    public GameObject weapon;

    private float gravity = -9.81f;
    private bool stop = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!stop)
        {
            transform.position += velocity * Time.deltaTime;
            velocity.y += gravity * Time.deltaTime;
        }
        transform.Rotate(new Vector3(0, Time.deltaTime * 30, 0));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            stop = true;
        }
    }
}
