using UnityEngine;

[CreateAssetMenu(menuName = "Items/New Item")]
public class Item : ScriptableObject
{
    [Header("Customization")]
    public string id = "example_item";
    public string displayName = "Example Item";
    public bool saveable = true;

    [Header("Item Reference")]
    public GameObject itemObject;
}
