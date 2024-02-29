using Core.Config;
using Protocol;

namespace GameServer.Models;
internal class InventoryModel
{
    private int _itemIncrId;

    public List<NormalItem> ItemList { get; } = [];
    public List<WeaponItem> WeaponList { get; } = [];

    public WeaponItem? GetEquippedWeapon(int roleId) => WeaponList.SingleOrDefault(weapon => weapon.RoleId == roleId);

    public WeaponItem? GetWeaponById(int incrId) => WeaponList.SingleOrDefault(weapon => weapon.IncrId == incrId);

    public NormalItem? GetItemById(int itemId) => ItemList.SingleOrDefault(item => item.Id == itemId);

    public int GetItemCount(int itemId) => ItemList.SingleOrDefault(item => item.Id == itemId)?.Count ?? 0;

    public bool TryUseItem(ItemInfoConfig itemInfo, int amount)
    {
        int currentAmount = GetItemCount(itemInfo.Id);
        if (amount > currentAmount) return false;

        AddItem(itemInfo, -amount);
        return true;
    }

    public void AddItem(ItemInfoConfig itemInfo, int amount)
    {
        NormalItem? item = ItemList.SingleOrDefault(item => item.Id == itemInfo.Id);
        if (item != null)
        {
            item.Count += amount;
            return;
        }

        ItemList.Add(new NormalItem
        {
            Id = itemInfo.Id,
            Count = amount
        });
    }

    public WeaponItem AddNewWeapon(int weaponId)
    {
        WeaponItem weapon = new()
        {
            Id = weaponId,
            IncrId = ++_itemIncrId,
            WeaponLevel = 1,
            WeaponResonLevel = 1
        };

        WeaponList.Add(weapon);
        return weapon;
    }

    public void RemoveItem(int itemId, int amount)
    {
        NormalItem? item = ItemList.SingleOrDefault(item => item.Id == itemId);
        if (item == null) return;

        item.Count -= amount;
        if (item.Count <= 0)
        {
            ItemList.Remove(item);
        }
    }
}
