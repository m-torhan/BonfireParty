using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddleScript : MonoBehaviour
{
    public float volume;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        volume -= Time.deltaTime/4f;
        if (volume > 0)
        {
            transform.localScale = new Vector3(Mathf.Sqrt(volume), transform.localScale.y, Mathf.Sqrt(volume));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddWater(float amount)
    {
        volume = Mathf.Sqrt(volume * volume + amount);
    }
}
