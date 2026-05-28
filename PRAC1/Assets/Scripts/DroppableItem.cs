using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DroppableItem : MonoBehaviour
{
    public bool IsDropped { get; private set; }

    private void Reset()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.mass = 1f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;
        rb.useGravity = true;

        Collider itemCollider = GetComponent<Collider>();
        itemCollider.isTrigger = false;
    }

    public bool MarkDropped()
    {
        if (IsDropped)
        {
            return false;
        }

        IsDropped = true;
        return true;
    }
}
