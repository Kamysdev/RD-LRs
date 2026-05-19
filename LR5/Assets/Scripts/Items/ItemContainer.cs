using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField] public ItemInstance item;
    [SerializeField] public int amount = 1;

    public void pickup(int remaining)
    {
        if (remaining > 0)
            amount = remaining;
        else
            Destroy(gameObject);
    }
}
