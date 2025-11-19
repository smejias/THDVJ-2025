using UnityEngine;

[CreateAssetMenu(fileName = "Magazine", menuName = "Game/Items/Magazine")]
public class SO_Magazine : SO_PickupItem
{
    [Header("Magazine Settings")]
    public int magazinesAmount = 1;

    public override bool Use(GameObject player)
    {
        GunController gun = player.GetComponentInChildren<GunController>();
        if (gun != null)
        {
            bool added = gun.AddMagazines(magazinesAmount);
            if (added)
            {
                Debug.Log($"[Magazine] Added {magazinesAmount} magazine(s)");
                return true;
            }
            else
            {
                Debug.Log($"[Magazine] Already at max magazines");
                return false;
            }
        }
        return false;
    }
}