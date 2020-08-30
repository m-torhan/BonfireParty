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

    Vector3 velocity;
    bool isGrounded;
    bool isRunning;

    private GameObject camera;

    private CharacterController controller;

    //[SerializeField]
    //private Animator animator;

    [SerializeField]
    private GameObject hand;
    [SerializeField]
    private GameObject hiddenSpot;

    private GameObject weapon;

    
    [SerializeField]
    private GameObject[] weaponSlots = new GameObject[3];
    [SerializeField]
    private GameObject weaponContainerPrefab;

    private int activeWeaponSlot = 0;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();

        camera = transform.Find("MainCamera").gameObject;
        initCameraPosition = camera.transform.localPosition;

        for (int i = 0; i < 3; ++i)
        {
            if (weaponSlots[i] != null)
            {
                GameObject tempWeapon = Instantiate(weaponSlots[i]);
                tempWeapon.transform.parent = hiddenSpot.transform;
                tempWeapon.transform.localPosition = Vector3.zero;
                weaponSlots[i] = tempWeapon;
            }
        }
        SwitchWeapon(0);
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
            isRunning = true;
            //animator.SetBool("isRunning", move.magnitude > 0);
        }
        else
        {
            isRunning = false;
        }
        
        controller.Move(move * speed * Time.deltaTime);

        if (move.magnitude > 0 && isGrounded)
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

        // shot
        if (Input.GetMouseButton(0) && weapon != null)
        {
            weapon.GetComponent<WeaponScript>().Shot();
        }

        // reload
        if (Input.GetKeyDown(KeyCode.R) && weapon != null)
        {
            weapon.GetComponent<WeaponScript>().Reload();
        }

        // discard
        if (Input.GetKeyDown(KeyCode.Q) && weapon != null)
        {
            DiscardWeapon();
        }

        // weapon switch
        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponSlots[0] != null && activeWeaponSlot != 0) {
            SwitchWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && weaponSlots[1] != null && activeWeaponSlot != 1)
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && weaponSlots[2] != null && activeWeaponSlot != 2)
        {
            SwitchWeapon(2);
        }
    }

    private void SwitchWeapon(int weaponSlot)
    {
        if (weapon != null)
        {
            weapon.transform.parent = hiddenSpot.transform;
            weapon.transform.localPosition = Vector3.zero;
        }

        activeWeaponSlot = weaponSlot;
        weapon = weaponSlots[weaponSlot];
        weapon.GetComponent<WeaponScript>().Draw();

        weapon.transform.parent = hand.transform;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        //animator.SetBool("weaponDraw", true);
    }

    public void PickUpWeapon(GameObject weapon)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (weaponSlots[i] == null)
            {
                weaponSlots[i] = weapon;
                weapon.transform.parent = hiddenSpot.transform;
                weapon.transform.localPosition = Vector3.zero;
                SwitchWeapon(i);
                break;
            }
        }
    }

    private void DiscardWeapon()
    {
        // check if have more than one weapon
        if ((weaponSlots[0] == null ? 0 : 1) + (weaponSlots[1] == null ? 0 : 1) + (weaponSlots[2] == null ? 0 : 1) >= 2)
        {
            // throw away active weapon
            weaponSlots[activeWeaponSlot] = null;
            GameObject weaponContainer = Instantiate(weaponContainerPrefab, transform);
            weaponContainer.transform.parent = null;
            weaponContainer.GetComponent<WeaponContainerScript>().velocity = camera.transform.forward*10f;
            weaponContainer.GetComponent<WeaponContainerScript>().weapon = weapon;
            weapon.transform.parent = weaponContainer.transform;
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weapon = null;

            // switch to another weapon
            for (int i = 0; i < 3; ++i)
            {
                if (weaponSlots[i] != null)
                {
                    SwitchWeapon(i);
                    break;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Container"))
        {
            PickUpWeapon(other.gameObject.GetComponent<WeaponContainerScript>().weapon);
            Destroy(other.gameObject);
        }
    }
}
