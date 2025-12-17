using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;
    public float SmoothSpeed = 10f;

    private void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        Vector3 FollowPos = Target.position + Offset;
        transform.position = Vector3.Lerp(transform.position, FollowPos, SmoothSpeed * Time.deltaTime);
    }
}
