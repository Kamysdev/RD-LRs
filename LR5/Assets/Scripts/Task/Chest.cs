using UnityEngine;

public class Chest : MonoBehaviour
{
    public Inventory inventory;
    public GameObject chestPanel;
    public InventoryUI chestInventoryUI;
    public InventoryUI playerInventoryUI;
    private PlayerScript currentPlayer;

    public void TransferToPlayer(PlayerScript player, int index)
    {
        if (player == null || inventory == null)
            return;

        ItemInstance item = inventory.getItem(index);
        if (item == null)
            return;

        int amount = inventory.getAmount(index);
        Inventory playerInventory = player.GetComponent<Inventory>();
        if (playerInventory == null)
            return;

        int remaining = playerInventory.addItem(item, amount);
        int transferred = amount - remaining;
        if (transferred > 0)
            inventory.removeAmount(index, transferred);

        RefreshUIs();
    }

    public void TransferFromPlayer(PlayerScript player, int index)
    {
        if (player == null || inventory == null)
            return;

        Inventory playerInventory = player.GetComponent<Inventory>();
        if (playerInventory == null)
            return;

        ItemInstance item = playerInventory.getItem(index);
        if (item == null)
            return;

        int amount = playerInventory.getAmount(index);
        int remaining = inventory.addItem(item, amount);
        int transferred = amount - remaining;
        if (transferred > 0)
        {
            playerInventory.removeAmount(index, transferred);
            if (remaining == 0)
                player.RemoveEquippedReferences(item);
        }

        RefreshUIs();
    }

    public void Open(PlayerScript player)
    {
        currentPlayer = player;

        if (chestPanel != null)
            chestPanel.SetActive(true);

        if (chestInventoryUI != null)
        {
            chestInventoryUI.SetInventory(inventory);
            chestInventoryUI.SetTransferContext(this, player, true);
        }

        if (playerInventoryUI != null)
            playerInventoryUI.SetTransferContext(this, player, false);

        RefreshUIs();
    }

    public void Close()
    {
        if (chestPanel != null)
            chestPanel.SetActive(false);

        if (chestInventoryUI != null)
            chestInventoryUI.ClearTransferContext();

        if (playerInventoryUI != null)
            playerInventoryUI.ClearTransferContext();

        if (currentPlayer != null && currentPlayer.openedChest == this)
            currentPlayer.openedChest = null;

        currentPlayer = null;
    }

    public bool IsOpen()
    {
        return chestPanel != null && chestPanel.activeSelf;
    }

    private void RefreshUIs()
    {
        if (chestInventoryUI != null)
            chestInventoryUI.updateUI();

        if (playerInventoryUI != null)
            playerInventoryUI.updateUI();
    }
}
