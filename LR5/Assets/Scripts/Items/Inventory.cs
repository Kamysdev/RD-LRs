using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] public List<InventorySlot> slots;
    [SerializeField] public int size;
    [SerializeField] public UnityEvent onInventoryChanged;

    private void Start()
    {
        onInventoryChanged?.Invoke();
    }

    public int addItem(ItemInstance item, int amount)
    {
        if (item == null || item.itemData == null || amount <= 0)
            return amount;

        foreach (InventorySlot slot in slots)
        {
            if (slot.item == null || slot.item.itemData == null)
                continue;

            if (slot.item.itemData.id == item.itemData.id)
            {
                if (slot.amount >= item.itemData.max_stack)
                    continue;

                int canAdd = item.itemData.max_stack - slot.amount;
                int added = Mathf.Min(canAdd, amount);
                slot.amount += added;
                amount -= added;

                if (amount <= 0)
                {
                    onInventoryChanged?.Invoke();
                    return 0;
                }
            }
        }

        while (slots.Count < size && amount > 0)
        {
            ItemInstance inst = new ItemInstance();
            inst.itemData = item.itemData;
            inst.damage = item.damage;
            int stackAmount = Mathf.Min(amount, item.itemData.max_stack);
            slots.Add(new InventorySlot(inst, stackAmount));
            amount -= stackAmount;
        }

        onInventoryChanged?.Invoke();
        return amount;
    }

    public ItemInstance getItem(int i)
    {
        return i < slots.Count ? slots[i].item : null;
    }

    public int getAmount(int i)
    {
        return i < slots.Count ? slots[i].amount : 0;
    }

    public int getSize()
    {
        return slots.Count;
    }

    public void removeItem(int i)
    {
        if (i < slots.Count)
        {
            slots[i].amount--;
            if (slots[i].amount <= 0)
                slots.RemoveAt(i);
            onInventoryChanged?.Invoke();
        }
    }

    public void dropItem(int i)
    {
        if (i < slots.Count)
        {
            GameObject pref = slots[i].item.itemData.prefab;
            GameObject obj = Instantiate(pref, transform.position + transform.forward * 3, pref.transform.rotation);
            obj.GetComponent<ItemContainer>().item = slots[i].item;
            obj.GetComponent<ItemContainer>().amount = slots[i].amount;
            slots.RemoveAt(i);
            onInventoryChanged?.Invoke();
        }
    }

    public void destroyItem(int i)
    {
        if (i < slots.Count)
        {
            slots.RemoveAt(i);
            onInventoryChanged?.Invoke();
        }
    }

    public void removeOne(int i)
    {
        removeItem(i);
    }

    public void removeAmount(int i, int amount)
    {
        if (i < 0 || i >= slots.Count || amount <= 0)
            return;

        slots[i].amount -= amount;
        if (slots[i].amount <= 0)
            slots.RemoveAt(i);
        onInventoryChanged?.Invoke();
    }
}
