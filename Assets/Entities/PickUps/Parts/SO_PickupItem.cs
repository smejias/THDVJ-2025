using UnityEngine;

public abstract class SO_PickupItem : ScriptableObject
{
    [Header("Item Info")]
    public string itemName = "Item";

    public abstract bool Use(GameObject player);
}