using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Armor")]
public class Armor : Item
{
    public int defense;

    public override bool use(PlayerScript player, ItemInstance itemData)
    {
        if (player == null || itemData == null || itemData.itemData == null)
            return false;

        player.equippedArmor = itemData;
        player.EquipArmorItem(itemData.itemData.prefab);
        return false;
    }
}
