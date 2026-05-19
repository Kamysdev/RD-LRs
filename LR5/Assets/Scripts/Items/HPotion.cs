using UnityEngine;

[CreateAssetMenu(menuName = "inventory/HPotion")]
public class HPotion : Item
{
    public int healing;

    public override bool use(PlayerScript player, ItemInstance itemData)
    {
        player.stats.health += healing;
        return true;
    }
}
