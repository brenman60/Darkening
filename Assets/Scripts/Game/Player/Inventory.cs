using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Customization")]
    [SerializeField] private float itemSpeed = 10f;
    [SerializeField] private float swayAmount = 0.1f;
    [SerializeField] private float swaySpeed = 2f;
    [SerializeField] private float bobAmount = 0.05f;

    [Header("References")]
    [SerializeField] private Items items;
    [SerializeField] private Transform inventoryHolder;
    [SerializeField] private CanvasGroup itemUI;
    [SerializeField] private TextMeshProUGUI heldItemText;

    private Dictionary<Item, HoldableItem> inventoryItems = new Dictionary<Item, HoldableItem>();
    private int index;

    private Vector3 initialInventoryPos;
    private float inventoryBobTimer;

    private void Start()
    {
        initialInventoryPos = inventoryHolder.localPosition;

        AddItem(items.GetItem("camera_terminal"));
    }

    private void Update()
    {
        SwayInventory();
        ScrollInventory();
        UpdateInventory();
        UpdateUI();
    }

    private void SwayInventory()
    {
        float movement = Player.Instance.speedMultiple;
        if (movement != 0f)
        {
            inventoryBobTimer += Time.deltaTime * swaySpeed * (movement);
            float sway = Mathf.Sin(inventoryBobTimer) * swayAmount;
            float bob = Mathf.Abs(Mathf.Cos(inventoryBobTimer * 2) * bobAmount);
            inventoryHolder.localPosition = Vector3.Lerp(inventoryHolder.localPosition, initialInventoryPos + new Vector3(sway, -bob, 0), Time.deltaTime * 5f);
        }
        else
        {
            inventoryHolder.localPosition = Vector3.Lerp(inventoryHolder.localPosition, initialInventoryPos, Time.deltaTime * 5f);
        }
    }

    private void ScrollInventory()
    {
        Vector2 scroll = Keybinds.scroll.ReadValue<Vector2>() / 120f;
        int scrollY = Mathf.RoundToInt(scroll.y);
        if (index + scrollY >= inventoryItems.Count + 1)
            index = 0;
        else if (index + scrollY < 0)
            index = inventoryItems.Count - 1;
        else
            index += scrollY;
    }

    private void UpdateInventory()
    {
        int currentIndex = 0;
        foreach (KeyValuePair<Item, HoldableItem> item in inventoryItems)
        {
            bool selected = currentIndex == index;
            item.Value.holding = selected;
            currentIndex++;
        }
    }

    private void UpdateUI()
    {
        if (inventoryItems.Count > 0)
        {
            itemUI.alpha = Mathf.Lerp(itemUI.alpha, 1f, Time.deltaTime * itemSpeed);
            if (index != inventoryItems.Count)
                heldItemText.text = inventoryItems.ElementAt(index).Key.displayName;
            else
                heldItemText.text = string.Empty;
        }
        else
            itemUI.alpha = Mathf.Lerp(itemUI.alpha, 0f, Time.deltaTime * itemSpeed);
    }

    public void AddItem(Item item)
    {
        GameObject itemObject = Instantiate(item.itemObject, inventoryHolder);
        HoldableItem holdableItem = itemObject.GetComponent<HoldableItem>();
        itemObject.name = item.name;

        inventoryItems.Add(item, holdableItem);
    }
}
