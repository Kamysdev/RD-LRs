using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Inventory))]
public class PlayerScript : MonoBehaviour
{
    public float speed = 10;
    public float angular_speed = 180;
    public float interaction_range = 2;
    public LayerMask items;

    public CharacterController cc;
    public TMPro.TMP_Text description;

    public Transform holder;
    public Transform hand;
    public Transform armorHolder;
    public ItemInstance activeItem;
    public ItemInstance equippedArmor;
    public Chest openedChest;

    public Animator anim;
    public Stats stats = new Stats();
    private Inventory inventory;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
        activeItem = null;
    }

    private void LateUpdate()
    {
        float yRotation = Input.GetAxisRaw("Horizontal");
        float forwardMove = Input.GetAxisRaw("Vertical");

        transform.Rotate(new Vector3(0, yRotation * angular_speed * Time.deltaTime, 0));

        Vector3 dir = new Vector3(0, 0, forwardMove);
        dir.Normalize();
        dir = transform.TransformDirection(dir);
        cc.Move(dir * speed * Time.deltaTime);

        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Ray ray = new Ray(rayOrigin, transform.forward);

        if (Physics.Raycast(ray, out hit, interaction_range, items))
        {
            ItemContainer container = hit.transform.GetComponentInParent<ItemContainer>();
            Chest chest = hit.transform.GetComponentInParent<Chest>();
            Transform currentHolder = holder != null ? holder : hand;

            if (container != null && container.transform.parent != currentHolder && container.item != null && container.item.itemData != null)
                description.text = container.item.itemData.item_name;
            else if (chest != null)
                description.text = "Chest";
            else
                description.text = "";

            if (Input.GetKeyDown(KeyCode.E) && container != null)
            {
                ItemInstance item = container.item;
                int amount = container.amount;
                int remaining = inventory.addItem(item, amount);
                container.pickup(remaining);
            }
            else if (Input.GetKeyDown(KeyCode.E) && chest != null)
            {
                if (openedChest != null && openedChest != chest)
                    openedChest.Close();

                if (openedChest == chest && chest.IsOpen())
                {
                    chest.Close();
                }
                else
                {
                    openedChest = chest;
                    chest.Open(this);
                }
            }
        }
        else
            description.text = "";

        if (Input.GetKeyDown(KeyCode.Escape) && openedChest != null)
            openedChest.Close();

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (activeItem != null)
                anim.SetTrigger(activeItem.itemData.action);
        }
    }

    public void use(int i)
    {
        ItemInstance item = inventory.getItem(i);
        if (item == null)
            return;

        if (item.use(this))
            inventory.removeItem(i);
        else
            inventory.onInventoryChanged?.Invoke();
    }

    public void drop(int i)
    {
        ItemInstance item = inventory.getItem(i);
        if (item == null)
            return;

        if (activeItem == item)
        {
            ClearHeldItem();
            activeItem = null;
        }

        if (equippedArmor == item)
        {
            ClearArmorItem();
            equippedArmor = null;
        }

        inventory.dropItem(i);
    }

    public void destroy(int i)
    {
        ItemInstance item = inventory.getItem(i);
        if (item == null)
            return;

        if (activeItem == item)
        {
            ClearHeldItem();
            activeItem = null;
        }

        if (equippedArmor == item)
        {
            ClearArmorItem();
            equippedArmor = null;
        }

        inventory.destroyItem(i);
    }

    public void RemoveEquippedReferences(ItemInstance item)
    {
        if (item == null)
            return;

        if (activeItem == item)
        {
            ClearHeldItem();
            activeItem = null;
        }

        if (equippedArmor == item)
        {
            ClearArmorItem();
            equippedArmor = null;
        }

        inventory.onInventoryChanged?.Invoke();
    }

    public void EquipHeldItem(GameObject prefab)
    {
        Transform currentHolder = holder != null ? holder : hand;
        if (currentHolder == null || prefab == null)
            return;

        ClearHeldItem();

        GameObject equipped = Instantiate(prefab, currentHolder, false);
        equipped.transform.localPosition = Vector3.zero;
        equipped.transform.localRotation = Quaternion.identity;
    }

    public void ClearHeldItem()
    {
        Transform currentHolder = holder != null ? holder : hand;
        if (currentHolder == null)
            return;

        for (int childIndex = currentHolder.childCount - 1; childIndex >= 0; childIndex--)
            Destroy(currentHolder.GetChild(childIndex).gameObject);
    }

    public void EquipArmorItem(GameObject prefab)
    {
        if (armorHolder == null || prefab == null)
            return;

        ClearArmorItem();

        GameObject equipped = Instantiate(prefab, armorHolder, false);
        equipped.transform.localPosition = Vector3.zero;
        equipped.transform.localRotation = Quaternion.identity;
    }

    public void ClearArmorItem()
    {
        if (armorHolder == null)
            return;

        for (int childIndex = armorHolder.childCount - 1; childIndex >= 0; childIndex--)
            Destroy(armorHolder.GetChild(childIndex).gameObject);
    }

    public void useItem(int index)
    {
        use(index);
    }

    public void dropItem(int index)
    {
        drop(index);
    }

    public void destroyItem(int index)
    {
        destroy(index);
    }
}
