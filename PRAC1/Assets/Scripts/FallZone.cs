using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FallZone : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void Reset()
    {
        Collider zoneCollider = GetComponent<Collider>();
        zoneCollider.isTrigger = true;
    }

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DroppableItem item = other.GetComponent<DroppableItem>();

        if (item == null || !item.MarkDropped())
        {
            return;
        }

        if (gameManager != null)
        {
            gameManager.AddScore();
        }

        other.gameObject.SetActive(false);
    }
}
