using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private List<Image> icons = new();
    [SerializeField] private List<TMPro.TMP_Text> amounts = new();
    [SerializeField] private ItemMenu menu;
    [SerializeField] private Color defaultItemColor = Color.white;
    [SerializeField] private Color activeWeaponColor = new Color(1f, 0.9f, 0.35f, 1f);
    [SerializeField] private Color equippedArmorColor = new Color(0.55f, 0.85f, 1f, 1f);
    private PlayerScript player;
    private Chest transferChest;
    private bool inventoryBelongsToChest;

    private void Awake()
    {
        if (inventory != null)
            player = inventory.GetComponent<PlayerScript>();
    }

    public void updateUI()
    {
        if (inventory == null)
            return;

        int slotCount = Mathf.Min(icons.Count, amounts.Count);

        for (int i = 0; i < slotCount; i++)
        {
            bool hasItem = i < inventory.getSize();
            ItemInstance item = hasItem ? inventory.getItem(i) : null;

            if (item == null || item.itemData == null)
            {
                icons[i].sprite = null;
                icons[i].color = new Color(1f, 1f, 1f, 0f);
                amounts[i].text = "";
                continue;
            }

            icons[i].sprite = item.itemData.icon;
            icons[i].color = GetItemColor(item);
            amounts[i].text = inventory.getAmount(i) > 1 ? inventory.getAmount(i).ToString() : "";
        }
    }

    private Color GetItemColor(ItemInstance item)
    {
        if (player == null || item == null)
            return defaultItemColor;

        if (player.activeItem == item)
            return activeWeaponColor;

        if (player.equippedArmor == item)
            return equippedArmorColor;

        return defaultItemColor;
    }

    public void showMenu(int index)
    {
        if (inventory == null || index < 0 || inventory.getItem(index) == null)
            return;

        if (transferChest != null && player != null)
        {
            if (inventoryBelongsToChest)
                transferChest.TransferToPlayer(player, index);
            else
                transferChest.TransferFromPlayer(player, index);
            return;
        }

        if (menu == null)
            return;

        if (player != null)
            menu.player = player;

        RectTransform target = null;
        if (index < icons.Count)
            target = icons[index].transform as RectTransform;

        menu.show(target, index);
    }

    public void SetInventory(Inventory sourceInventory)
    {
        inventory = sourceInventory;

        if (inventory != null && player == null)
            player = inventory.GetComponent<PlayerScript>();

        updateUI();
    }

    public void SetPlayer(PlayerScript sourcePlayer)
    {
        player = sourcePlayer;
        updateUI();
    }

    public void SetTransferContext(Chest chest, PlayerScript sourcePlayer, bool sourceIsChestInventory)
    {
        transferChest = chest;
        player = sourcePlayer;
        inventoryBelongsToChest = sourceIsChestInventory;
        updateUI();
    }

    public void ClearTransferContext()
    {
        transferChest = null;
        inventoryBelongsToChest = false;
        updateUI();
    }
}
