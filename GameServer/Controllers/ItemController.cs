using Core.Config;
using GameServer.Controllers.Attributes;
using GameServer.Models;
using GameServer.Network;
using Protocol;

namespace GameServer.Controllers;
internal class ItemController : Controller
{
    public ItemController(PlayerSession session) : base(session)
    {
        // ItemController.
    }

    // >> Item ID: 43020004 - exp: 20000, cost: 8000
    // >> Item ID: 43020003 - exp: 8000, cost: 3200
    // >> Item ID: 43020002 - exp: 3000, cost: 1200
    // >> Item ID: 43020001 - exp: 1000, cost: 400

    // Level id info
    // LevelId: 1, QualityId: 1
    // LevelId: 2, QualityId: 2
    // LevelId: 3, QualityId: 3
    // LevelId: 4, QualityId: 4
    // LevelId: 5, QualityId: 4
    [NetEvent(MessageId.WeaponLevelUpRequest)]
    public RpcResult OnWeaponLevelUpRequest(WeaponLevelUpRequest request, ModelManager modelManager)
    {
        WeaponConsumeItem[] consumeItems = request.ConsumeList.ToArray();
        WeaponItem? weapon = modelManager.Inventory.GetWeaponById(request.IncId);
        List<ItemInfoConfig> ListItems = modelManager.Config.Enumerate<ItemInfoConfig>().ToList();
        List<WeaponConfig> ListWeapons = modelManager.Config.Enumerate<WeaponConfig>().ToList();

        int costCredits = 0;
        int Exp = 0;
        int lvl = weapon!.WeaponLevel;

        WeaponConfig weaponItemInfo = ListWeapons.Single(x => x.ItemId == weapon.Id);
        WeaponLevelModel weaponLevel = WeaponLevelModel.getRootAsWeaponLevel(
            WeaponLevelConfig.WeaponLevelById(weaponItemInfo!.LevelId, weapon!.WeaponLevel)
        );

        foreach (WeaponConsumeItem consumeItem in consumeItems)
        {

            ItemInfoConfig itemInfo = ListItems.Single(x => x.Id == consumeItem.ItemId);
            Console.WriteLine($">> Item ID: {itemInfo.Id}");

            WeaponExpItemModel weaponExpItem = WeaponExpItemModel.getRootAsWeaponExp(
                WeaponLevelConfig.WeaponExpById(consumeItem.ItemId)
            );

            if (weaponExpItem != null)
            {
                // TODO: Find a way to calculate the cost of materials to level up a weapon
                costCredits += weaponExpItem.Cost * consumeItem.Count;
                Exp += weaponExpItem.BasicExp * consumeItem.Count;

                modelManager.Inventory.RemoveItem(consumeItem.ItemId, consumeItem.Count);
            }
        }

        // calculate level correctly based on exp
        // DON'T WORK CORRECTLY
        while (Exp >= weaponLevel.Exp)
        {
            Console.WriteLine($">> Exp: {Exp} - Level: {lvl} - WeaponLevel: {weaponLevel.Exp}");
            lvl++;
            weaponLevel = WeaponLevelModel.getRootAsWeaponLevel(
                WeaponLevelConfig.WeaponLevelById(weaponItemInfo!.LevelId, lvl)
            );
        }


        modelManager.Player.SpendCoin(costCredits);

        if (weaponLevel == null)
        {
            return Response(MessageId.WeaponLevelUpResponse, new WeaponLevelUpResponse()
            {
                ErrorCode = (int)ErrorCode.ErrWeaponResonConfigNotFound
            });
        }

        Session.Push(MessageId.PlayerAttrNotify, new PlayerAttrNotify
        {
            Attributes = { modelManager.Player.Attributes }
        });
        Session.Push(MessageId.NormalItemResponse, new NormalItemResponse
        {
            NormalItemList = { modelManager.Inventory.ItemList }
        });

        return Response(MessageId.WeaponLevelUpResponse, new WeaponLevelUpResponse()
        {
            IncId = request.IncId,
            WeaponExp = Exp,
            WeaponLevel = lvl,
        });
    }

}