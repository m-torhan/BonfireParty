using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[System.Serializable]
public class Projectile : MonoBehaviour
{

    private Vector3 startingPos;
    private Vector3 target;
    private float speed;

    private float t = 0;
    private SphereCollider sphereCollider;
    private float health = 2f;

    [SerializeField, Range(10f, 200f)]
    float maxDistanceToTravel = 50f; 

    public float Damage { private set; get; }


    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
    public void Setup(Vector3 startingPos, Vector3 destination, float speed, float health = 2f, float damage = 5f)
    {
        this.startingPos = startingPos;

        Vector3 direction = Vector3.Normalize(destination - startingPos);
        
        this.target = startingPos + direction * maxDistanceToTravel;

        this.speed = speed;
        this.health = health;
        this.Damage = damage;

        t = 0;
        transform.position = startingPos;
    }
    void Update()
    {
        transform.position = Vector3.Lerp(startingPos, target, t);
        t += speed * Time.deltaTime;
        //if (Physics.CheckSphere(transform.position, sphereCollider.radius))
        //{
        //    //Debug.Log("Projectile bum (wczesniejsza kolizja)");
        //    Destroy(gameObject);
        //}
        if (t >= 1.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Destroy(gameObject);
    }



    public void ReceiveDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
            Destroy(gameObject);
    }
}
