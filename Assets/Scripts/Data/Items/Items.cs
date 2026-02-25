using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item List")]
public class Items : ScriptableObject
{
    public List<Item> items
    {
        get { return itemsList; }
    }
    [SerializeField] private List<Item> itemsList;

    private Dictionary<string, Item> itemIds = new Dictionary<string, Item>();
    private bool initialized = false;

    private void Init()
    {
        foreach (Item item in items)
            itemIds[item.id] = item;
    }

    public Item GetItem(string id)
    {
        if (!initialized) Init();

        if (itemIds.TryGetValue(id, out Item item)) return item;
        return null;
    }
}
