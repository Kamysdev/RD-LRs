using UnityEngine;

public abstract class Item : ScriptableObject
{
    public int id;
    public string item_name;
    public int max_stack = 1;
    public Sprite icon;
    public GameObject prefab;
    public string action;

    public abstract bool use(PlayerScript player, ItemInstance itemData);
}
