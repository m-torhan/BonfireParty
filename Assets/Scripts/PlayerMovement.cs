using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    public float stepLength = 1f;
    public float stepHeight = 1f;
    private float stepTime = -1f;
    private Vector3 initCameraPosition;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float reload = 1.0f;
    private float reloadTime = 0.0f;

    private GameObject camera;

    private CharacterController controller;
    
    //[SerializeField]
    //private Animator animator;

    [SerializeField]
    private GameObject waterBoltPrefab;

    [SerializeField]
    private GameObject weapon;
    private GameObject weaponBarrelEnd;

    Vector3 velocity;
    bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();

        weaponBarrelEnd = weapon.transform.Find("BarrelEnd").gameObject;

        camera = transform.Find("MainCamera").gameObject;
        initCameraPosition = camera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetKey(KeyCode.LeftShift) && z > 0 && Mathf.Abs(x) < 0.5)
        {
            move *= 1.5f;
            //animator.SetBool("isRunning", move.magnitude > 0);
        }

        controller.Move(move * speed * Time.deltaTime);

        if (move.magnitude > 0)
        {
            if (stepTime < 0)
            {
                stepTime = Time.time;
            }
            camera.transform.localPosition = initCameraPosition + new Vector3(0.0f, move.magnitude * stepHeight * Mathf.Sin((1.0f / stepLength) * (Time.time - stepTime)), 0.0f);
        }
        else
        {
            stepTime = -1f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        //animator.SetBool("isWalking", move.magnitude > 0);

        if (reloadTime >= 0)
        {
            reloadTime -= Time.deltaTime;
        }
        else if (Input.GetMouseButton(0) && weaponBarrelEnd != null)
        {
            Instantiate(waterBoltPrefab, weaponBarrelEnd.transform.position, weaponBarrelEnd.transform.rotation);
            reloadTime = reload;
        }
    }
}
