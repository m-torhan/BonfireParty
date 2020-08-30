using UnityEngine;

public static class Ai 
{

    public const int playerLayerMask = 1 << 9;

    public static bool IsInLineOfSight(Vector3 eyePos, Vector3 target)
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(eyePos, target - eyePos);

        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.transform.position == target)
                return true;
        }
        return false;
    }
}
