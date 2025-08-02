using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cameraToFace;

    void Update()
    {
        if (cameraToFace == null)
        {
            cameraToFace = Camera.main.transform;
        }

        Vector3 direction = transform.position - cameraToFace.position;
        direction.y = 0; // Optional: locks rotation so it only rotates around the Y-axis
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
