using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private SO_PickupItem itemData;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && itemData != null)
        {
            bool wasUsed = itemData.Use(other.gameObject);
            if (wasUsed)
            {
                Destroy(gameObject);
            }
        }
    }
}