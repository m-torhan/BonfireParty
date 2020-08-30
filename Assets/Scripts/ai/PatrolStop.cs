using UnityEngine;


public class PatrolStop : MonoBehaviour
{

    private void Awake()
    {
        if (Application.isPlaying)
        {
            Destroy(GetComponent<MeshRenderer>());
            Destroy(GetComponent<BoxCollider>());

        }
    }
}
