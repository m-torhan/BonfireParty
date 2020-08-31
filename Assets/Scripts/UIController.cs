using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private PlayerMovement playerScript;

    [SerializeField]
    private Image healthIcon;
    [SerializeField]
    private Text healthText;
    [SerializeField]
    private Image ammoIcon;
    [SerializeField]
    private Text ammoText;
    [SerializeField]
    private Image[] weaponSlotIcon = new Image[3];
    [SerializeField]
    private Image reloadIcon;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = string.Format("{0:0}", playerScript.GetHealth());
        ammoText.text = string.Format("{0:0}/{1:0}", playerScript.GetMagazineAmmo(), playerScript.GetAmmo());
        int activeWeaponSlot = playerScript.GetActiveWeaponSlot();
        for (int i = 0; i < 3; ++i)
        {
            Sprite icon = playerScript.GetWeaponSlotIcon(i);
            if (icon == null)
            {
                weaponSlotIcon[i].enabled = false;
            }
            else
            {
                weaponSlotIcon[i].enabled = true;
                weaponSlotIcon[i].sprite = icon;
                weaponSlotIcon[i].rectTransform.sizeDelta = i == activeWeaponSlot ? new Vector2(64, 64) : new Vector2(48, 48);
            }
        }
        float reloadTime = playerScript.GetReloadTime();
        if (reloadTime > 0)
        {
            reloadIcon.enabled = true;
            reloadIcon.rectTransform.eulerAngles = new Vector3(0f, 0f, reloadTime * 360);
        }
        else
        {
            reloadIcon.enabled = false;
        }
    }
}
