using UnityEngine;

[CreateAssetMenu(menuName = "inventory/Weapon")]
public class Weapon : Item
{
    public int min_damage;
    public int max_damage;

    public override bool use(PlayerScript player, ItemInstance itemData)
    {
        if (player == null || itemData == null || itemData.itemData == null)
            return false;

        player.activeItem = itemData;
        player.EquipHeldItem(itemData.itemData.prefab);

        return false;
    }
}
