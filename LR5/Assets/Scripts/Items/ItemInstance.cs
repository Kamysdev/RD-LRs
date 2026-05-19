[System.Serializable]
public class ItemInstance
{
    public Item itemData;
    public int damage;

    public bool use(PlayerScript player)
    {
        return itemData != null && itemData.use(player, this);
    }
}
