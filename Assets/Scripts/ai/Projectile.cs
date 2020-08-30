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

    [SerializeField, Range(10f, 200f)]
    float maxDistanceToTravel = 50f;  // max distance to travel

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
    public void Setup(Vector3 startingPos, Vector3 destination, float speed)
    {
        this.startingPos = startingPos;

        Vector3 direction = Vector3.Normalize(destination - startingPos);
        
        this.target = startingPos + direction * maxDistanceToTravel;

        this.speed = speed;

        t = 0;
        transform.position = startingPos;
    }
    void Update()
    {
        transform.position = Vector3.Lerp(startingPos, target, t);
        t += speed * Time.deltaTime;
        if (Physics.CheckSphere(transform.position, sphereCollider.radius))
        {
            Debug.Log("Projectile bum (wczesniejsza kolizja)");
            Destroy(gameObject);
        }
        if (t >= 1.0f)
        {
            Debug.Log("Projectile samozniszenie (niczego nie trafil)");
            Destroy(gameObject);
        }
    }

}
