// 

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

    // >> Item ID: 43020004 
    // >> Item ID: 43020003
    // >> Item ID: 43020002
    // >> Item ID: 43020001
    [NetEvent(MessageId.WeaponLevelUpRequest)]
    public RpcResult OnWeaponLevelUpRequest(WeaponLevelUpRequest request, ModelManager modelManager)
    {
        WeaponConsumeItem[] consumeItems = request.ConsumeList.ToArray();
        WeaponItem? weapon = modelManager.Inventory.GetWeaponById(request.IncId);
        List<ItemInfoConfig> ListItems = modelManager.Config.Enumerate<ItemInfoConfig>().ToList();

        int costCredits = 0;
        int Exp = 0;

        foreach (WeaponConsumeItem consumeItem in consumeItems)
        {
            ItemInfoConfig itemInfo = ListItems.Single(x => x.Id == consumeItem.ItemId);

            Console.WriteLine($">> Item ID: {itemInfo.Id}");
            // TODO: Find a way to calculate the cost of materials to level up a weapon
            costCredits += 100 * consumeItem.Count;
            Exp += 100 * consumeItem.Count;

            modelManager.Inventory.RemoveItem(consumeItem.ItemId, consumeItem.Count);
        }

        modelManager.Player.SpendCoin(costCredits);

        return Response(MessageId.WeaponLevelUpResponse, new WeaponLevelUpResponse()
        {
            IncId = request.IncId,
            WeaponExp = Exp,
            // WeaponLevel = weapon!.WeaponLevel + 1
        });
    }

}