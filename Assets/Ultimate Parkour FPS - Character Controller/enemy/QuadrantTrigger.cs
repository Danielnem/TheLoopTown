using UnityEngine;

public class QuadrantTrigger : MonoBehaviour
{
    public string quadrantName; // E.g. "QuadrantA"

    private bool isPlayerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlayerInside && other.CompareTag("Player"))
        {
            isPlayerInside = true;
            QuadrantManager.Instance.TriggerOppositeQuadrant(quadrantName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayerInside && other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}
