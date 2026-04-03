using UnityEngine;

public class UnitObstacleBlocker : MonoBehaviour
{
    private bool isBlocked;

    public bool IsBlocked()
    {
        return isBlocked;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Building"))
        {
            isBlocked = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Building"))
        {
            isBlocked = false;
        }
    }
}