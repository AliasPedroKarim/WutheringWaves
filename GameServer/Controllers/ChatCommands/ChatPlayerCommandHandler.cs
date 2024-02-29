using Core.Config;
using GameServer.Controllers.Attributes;
using GameServer.Models;
using GameServer.Models.Chat;
using GameServer.Network;
using GameServer.Systems.Entity;
using Protocol;

namespace GameServer.Controllers.ChatCommands;

[ChatCommandCategory("player")]
internal class ChatPlayerCommandHandler
{
    private readonly ChatRoom _helperRoom;
    private readonly PlayerSession _session;
    private readonly CreatureController _creatureController;
    private readonly ModelManager _modelManager;

    public ChatPlayerCommandHandler(ModelManager modelManager, PlayerSession session, CreatureController creatureController)
    {
        _helperRoom = modelManager.Chat.GetChatRoom(1338);
        _session = session;
        _creatureController = creatureController;
        _modelManager = modelManager;
    }

    [ChatCommand("getpos")]
    [ChatCommandDesc("/player getpos - shows current player coordinates")]
    public void OnPlayerGetPosCommand(string[] _)
    {
        PlayerEntity? entity = _creatureController.GetPlayerEntity();
        if (entity == null) return;

        _helperRoom.AddMessage(1338, 0, $"Your current position: ({entity.Pos.X / 100}, {entity.Pos.Y / 100}, {entity.Pos.Z / 100})");
    }

    [ChatCommand("teleport")]
    [ChatCommandDesc("/player teleport [x] [y] [z] - performing fast travel to specified position")]
    public async Task OnPlayerTeleportCommand(string[] args)
    {
        if (args.Length != 3 || !float.TryParse(args[0], out float x)
            || !float.TryParse(args[1], out float y)
            || !float.TryParse(args[2], out float z))
        {
            _helperRoom.AddMessage(1338, 0, $"Usage: /player teleport [x] [y] [z]");
            return;
        }

        PlayerEntity? entity = _creatureController.GetPlayerEntity();

        if (entity != null)
        {
            await _session.Push(MessageId.TeleportNotify, new TeleportNotify
            {
                PosX = x * 100,
                PosY = y * 100,
                PosZ = z * 100,
                PosA = 0,
                MapId = 8,
                Reason = (int)TeleportReason.Gm,
                TransitionOption = new TransitionOptionPb
                {
                    TransitionType = (int)TransitionType.Empty
                }
            });
        }

        _helperRoom.AddMessage(1338, 0, $"Successfully performed fast travel to ({x}, {y}, {z})");
    }

    // give item command
    [ChatCommand("giveitem")]
    [ChatCommandDesc("/player giveitem [itemId] [amount] - gives specified item to the player")]
    public void OnPlayerGiveItemCommand(string[] args)
    {
        if (args.Length != 2 || !int.TryParse(args[0], out int itemId) || !int.TryParse(args[1], out int amount))
        {
            _helperRoom.AddMessage(1338, 0, $"Usage: /player giveitem [itemId] [amount]");
            return;
        }

        List<ItemInfoConfig> ListItems = _modelManager.Config.Enumerate<ItemInfoConfig>().ToList();
        if (ListItems.Exists(x => x.Id == itemId))
        {
            ItemInfoConfig itemInfo = ListItems.Single(x => x.Id == itemId);
            _modelManager.Inventory.AddItem(itemInfo, amount);
            _helperRoom.AddMessage(1338, 0, $"Successfully gave item with id {itemId} and amount {amount}");
        }
        else
        {
            _helperRoom.AddMessage(1338, 0, $"Item with id {itemId} does not exist");
            return;
        }
    }

    // give weapon command
    [ChatCommand("giveweapon")]
    [ChatCommandDesc("/player giveweapon [weaponId] - gives specified weapon to the player")]
    public void OnPlayerGiveWeaponCommand(string[] args)
    {
        if (args.Length != 1 || !int.TryParse(args[0], out int weaponId))
        {
            _helperRoom.AddMessage(1338, 0, $"Usage: /player giveweapon [weaponId]");
            return;
        }

        List<WeaponConfig> ListWeapons = _modelManager.Config.Enumerate<WeaponConfig>().ToList();
        if (ListWeapons.Exists(x => x.ItemId == weaponId))
        {
            _modelManager.Inventory.AddNewWeapon(weaponId);
            _helperRoom.AddMessage(1338, 0, $"Successfully gave weapon with id {weaponId}");
        }
        else
        {
            _helperRoom.AddMessage(1338, 0, $"Weapon with id {weaponId} does not exist");
            return;
        }
    }
}
