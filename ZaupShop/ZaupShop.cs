using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using fr34kyn01535.Uconomy;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace ZaupShop
{
    // Token: 0x02000007 RID: 7
    public class ZaupShop : RocketPlugin<ZaupShopConfiguration>
    {
        // Token: 0x17000019 RID: 25
        // (get) Token: 0x0600002F RID: 47 RVA: 0x00003500 File Offset: 0x00001700
        public override TranslationList DefaultTranslations
        {
            get
            {
                var translationList = new TranslationList
                {
                    {"cost_command_usage", "Usage: /cost [v.]<name or id>."},
                    {"sell_command_usage", "Usage: /sell <name or id> [amount] (optional)."},
                    {
                        "shop_command_usage",
                        "Usage: /shop <add/rem/chng/buy> [v.]<itemid> <cost>  <cost> is not required for rem, buy is only for items."
                    },
                    {"error_giving_item", "There was an error giving you {0}.  You have not been charged."},
                    {"error_getting_cost", "There was an error getting the cost of {0}!"},
                    {"item_cost_msg", "The item {0} costs {1} {2} to buy and gives {3} {4} when you sell it."},
                    {"vehicle_cost_msg", "The vehicle {0} costs {1} {2} to buy."},
                    {"item_buy_msg", "You have bought {5} {0} for {1} {2}.  You now have {3} {4}."},
                    {"vehicle_buy_msg", "You have bought 1 {0} for {1} {2}.  You now have {3} {4}."},
                    {"not_enough_currency_msg", "You do not have enough {0} to buy {1} {2}."},
                    {"buy_items_off", "I'm sorry, but the ability to buy items is turned off."},
                    {"buy_vehicles_off", "I'm sorry, but the ability to buy vehicles is turned off."},
                    {"item_not_available", "I'm sorry, but {0} is not available in the shop."},
                    {"vehicle_not_available", "I'm sorry, but {0} is not available in the shop."},
                    {"could_not_find", "I'm sorry, I couldn't find an id for {0}."},
                    {"sell_items_off", "I'm sorry, but the ability to sell items is turned off."},
                    {"not_have_item_sell", "I'm sorry, but you don't have any {0} to sell."},
                    {"not_enough_items_sell", "I'm sorry, but you don't have {0} {1} to sell."},
                    {"not_enough_ammo_sell", "I'm sorry, but you don't have enough ammo in {0} to sell."},
                    {
                        "sold_items",
                        "You have sold {0} {1} to the shop and receive {2} {3} in return.  Your balance is now {4} {5}."
                    },
                    {"no_sell_price_set", "The shop is not buying {0} right now"},
                    {"no_itemid_given", "An itemid is required."},
                    {"no_cost_given", "A cost is required."},
                    {"invalid_amt", "You have entered in an invalid amount."},
                    {"v_not_provided", "You must specify v for vehicle or just an item id.  Ex. /shop rem/101"},
                    {"invalid_id_given", "You need to provide a valid item or vehicle id."},
                    {"no_permission_shop_chng", "You don't have permission to use the shop chng msg."},
                    {"no_permission_shop_add", "You don't have permission to use the shop add msg."},
                    {"no_permission_shop_rem", "You don't have permission to use the shop rem msg."},
                    {"no_permission_shop_buy", "You don't have permission to use the shop buy msg."},
                    {"changed", "changed"},
                    {"added", "added"},
                    {"changed_or_added_to_shop", "You have {0} the {1} with cost {2} to the shop."},
                    {"error_adding_or_changing", "There was an error adding/changing {0}!"},
                    {"removed_from_shop", "You have removed the {0} from the shop."},
                    {"not_in_shop_to_remove", "{0} wasn't in the shop, so couldn't be removed."},
                    {"not_in_shop_to_set_buyback", "{0} isn't in the shop so can't set a buyback price."},
                    {"set_buyback_price", "You set the buyback price for {0} to {1} in the shop."},
                    {"invalid_shop_command", "You entered an invalid shop command."}
                };
                return translationList;
            }
        }

        // Token: 0x06000030 RID: 48 RVA: 0x00003794 File Offset: 0x00001994
        protected override void Load()
        {
            Instance = this;
            ShopDB = new DatabaseMgr();
        }

        // Token: 0x14000001 RID: 1
        // (add) Token: 0x06000031 RID: 49 RVA: 0x000037A8 File Offset: 0x000019A8
        // (remove) Token: 0x06000032 RID: 50 RVA: 0x000037E0 File Offset: 0x000019E0
        public event PlayerShopBuy OnShopBuy;

        // Token: 0x14000002 RID: 2
        // (add) Token: 0x06000033 RID: 51 RVA: 0x00003818 File Offset: 0x00001A18
        // (remove) Token: 0x06000034 RID: 52 RVA: 0x00003850 File Offset: 0x00001A50
        public event PlayerShopSell OnShopSell;

        // Token: 0x06000035 RID: 53 RVA: 0x00003888 File Offset: 0x00001A88
        public void Buy(UnturnedPlayer playerId, string[] components0)
        {
            string message;
            if (components0.Length == 0)
            {
                message = Instance.Translate("buy_command_usage");
                UnturnedChat.Say(playerId, message);
                return;
            }

            byte b = 1;
            if (components0.Length > 1 && !byte.TryParse(components0[1], out b))
            {
                message = Instance.Translate("invalid_amt");
                UnturnedChat.Say(playerId, message);
                return;
            }

            var componentsFromSerial = Parser.getComponentsFromSerial(components0[0], '.');
            if ((componentsFromSerial.Length == 2 && componentsFromSerial[0].Trim() != "v") ||
                (componentsFromSerial.Length == 1 && componentsFromSerial[0].Trim() == "v") ||
                componentsFromSerial.Length > 2 || components0[0].Trim() == string.Empty)
            {
                message = Instance.Translate("buy_command_usage");
                UnturnedChat.Say(playerId, message);
                return;
            }

            var text = componentsFromSerial[0];
            string text2;
            ushort id;
            decimal num;
            decimal balance;
            decimal num2;
            if (text != null)
            {
                if (text == "v")
                {
                    if (!Instance.Configuration.Instance.CanBuyVehicles)
                    {
                        message = Instance.Translate("buy_vehicles_off");
                        UnturnedChat.Say(playerId, message);
                        return;
                    }

                    text2 = null;
                    if (!ushort.TryParse(componentsFromSerial[1], out id))
                    {
                        var array = Assets.find((EAssetType) 5);
                        foreach (var asset in array)
                        {
                            var vehicleAsset = (VehicleAsset) asset;
                            if (vehicleAsset?.vehicleName == null || !vehicleAsset.vehicleName
                                .ToLower().Contains(componentsFromSerial[1].ToLower())) continue;
                            id = vehicleAsset.id;
                            text2 = vehicleAsset.vehicleName;
                            break;
                        }
                    }

                    if (Assets.find((EAssetType) 5, id) == null)
                    {
                        message = Instance.Translate("could_not_find", componentsFromSerial[1]);
                        UnturnedChat.Say(playerId, message);
                        return;
                    }

                    if (text2 == null && id != 0)
                    {
                        text2 = ((VehicleAsset) Assets.find((EAssetType) 5, id)).vehicleName;
                    }

                    num = Instance.ShopDB.GetVehicleCost(id);
                    balance = Uconomy.Instance.Database.GetBalance(playerId.CSteamID.ToString());
                    if (num <= 0m)
                    {
                        message = Instance.Translate("vehicle_not_available", text2);
                        UnturnedChat.Say(playerId, message);
                        return;
                    }

                    if (balance < num)
                    {
                        message = Instance.Translate("not_enough_currency_msg",
                            Uconomy.Instance.Configuration.Instance.MoneyName, "1", text2);
                        UnturnedChat.Say(playerId, message);
                        return;
                    }

                    //  Vector3 point = player.transform.position + player.transform.forward * 6f;
                    var playerTransform = playerId.Player.transform;
                    // unturned's code calcuation for VehicleTool.giveVehicle:
                    // VehicleTool.cs : 84
                    var pos = playerTransform.position + playerTransform.forward * 6f;

                    if (!playerId.GiveVehicle(id))
                    {
                        message = Instance.Translate("error_giving_item", text2);
                        UnturnedChat.Say(playerId, message);
                        return;
                    }

                    num2 = Uconomy.Instance.Database.IncreaseBalance(playerId.CSteamID.ToString(), num * -1m);
                    message = Instance.Translate("vehicle_buy_msg", text2, num,
                        Uconomy.Instance.Configuration.Instance.MoneyName, num2,
                        Uconomy.Instance.Configuration.Instance.MoneyName);
                    Instance.OnShopBuy?.Invoke(playerId, num, 1, id, "vehicle");

                    playerId.Player.gameObject.SendMessage("ZaupShopOnBuy", new object[]
                    {
                        playerId,
                        num,
                        b,
                        id,
                        "vehicle"
                    }, (SendMessageOptions) 1);
                    UnturnedChat.Say(playerId, message);
                    return;
                }
            }

            if (!Instance.Configuration.Instance.CanBuyItems)
            {
                message = Instance.Translate("buy_items_off");
                UnturnedChat.Say(playerId, message);
                return;
            }

            text2 = null;
            if (!ushort.TryParse(componentsFromSerial[0], out id))
            {
                var array3 = Assets.find((EAssetType) 1);
                foreach (ItemAsset itemAsset in array3)
                {
                    if (itemAsset != null && itemAsset.itemName != null &&
                        itemAsset.itemName.ToLower().Contains(componentsFromSerial[0].ToLower()))
                    {
                        id = itemAsset.id;
                        text2 = itemAsset.itemName;
                        break;
                    }
                }
            }

            if (Assets.find((EAssetType) 1, id) == null)
            {
                message = Instance.Translate("could_not_find", componentsFromSerial[0]);
                UnturnedChat.Say(playerId, message);
                return;
            }

            if (text2 == null && id != 0)
            {
                text2 = ((ItemAsset) Assets.find((EAssetType) 1, id)).itemName;
            }

            num = decimal.Round(Instance.ShopDB.GetItemCost(id) * b, 2);
            balance = Uconomy.Instance.Database.GetBalance(playerId.CSteamID.ToString());
            if (num <= 0m)
            {
                message = Instance.Translate("item_not_available", text2);
                UnturnedChat.Say(playerId, message);
                return;
            }

            if (balance < num)
            {
                message = Instance.Translate("not_enough_currency_msg",
                    Uconomy.Instance.Configuration.Instance.MoneyName, b, text2);
                UnturnedChat.Say(playerId, message);
                return;
            }

            playerId.GiveItem(id, b);
            num2 = Uconomy.Instance.Database.IncreaseBalance(playerId.CSteamID.ToString(), num * -1m);
            message = Instance.Translate("item_buy_msg", text2, num, Uconomy.Instance.Configuration.Instance.MoneyName,
                num2, Uconomy.Instance.Configuration.Instance.MoneyName, b);
            if (Instance.OnShopBuy != null)
            {
                Instance.OnShopBuy(playerId, num, b, id, "item");
            }

            playerId.Player.gameObject.SendMessage("ZaupShopOnBuy", new object[]
            {
                playerId,
                num,
                b,
                id,
                "item"
            }, (SendMessageOptions) 1);
            UnturnedChat.Say(playerId, message);
        }

        // Token: 0x06000036 RID: 54 RVA: 0x00003FA0 File Offset: 0x000021A0
        public void Cost(UnturnedPlayer playerid, string[] components)
        {
            string message;
            if (components.Length == 0 || (components.Length == 1 &&
                                           (components[0].Trim() == string.Empty || components[0].Trim() == "v")))
            {
                message = Instance.Translate("cost_command_usage");
                UnturnedChat.Say(playerid, message);
                return;
            }

            if (components.Length == 2 && (components[0] != "v" || components[1].Trim() == string.Empty))
            {
                message = Instance.Translate("cost_command_usage");
                UnturnedChat.Say(playerid, message);
                return;
            }

            var text = components[0];
            string text2;
            ushort id;
            decimal d;
            if (text != null)
            {
                if (text == "v")
                {
                    text2 = null;
                    if (!ushort.TryParse(components[1], out id))
                    {
                        var array = Assets.find((EAssetType) 5);
                        foreach (VehicleAsset vehicleAsset in array)
                        {
                            if (vehicleAsset != null && vehicleAsset.vehicleName != null &&
                                vehicleAsset.vehicleName.ToLower().Contains(components[1].ToLower()))
                            {
                                id = vehicleAsset.id;
                                text2 = vehicleAsset.vehicleName;
                                break;
                            }
                        }
                    }

                    if (Assets.find((EAssetType) 5, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[1]);
                        UnturnedChat.Say(playerid, message);
                        return;
                    }

                    if (text2 == null && id != 0)
                    {
                        text2 = ((VehicleAsset) Assets.find((EAssetType) 5, id)).vehicleName;
                    }

                    d = Instance.ShopDB.GetVehicleCost(id);
                    message = Instance.Translate("vehicle_cost_msg", text2, d.ToString(),
                        Uconomy.Instance.Configuration.Instance.MoneyName);
                    if (d <= 0m)
                    {
                        message = Instance.Translate("error_getting_cost", text2);
                    }

                    UnturnedChat.Say(playerid, message);
                    return;
                }
            }

            text2 = null;
            if (!ushort.TryParse(components[0], out id))
            {
                var array3 = Assets.find((EAssetType) 1);
                foreach (var asset in array3)
                {
                    var itemAsset = (ItemAsset) asset;
                    if (itemAsset?.itemName == null ||
                        !itemAsset.itemName.ToLower().Contains(components[0].ToLower())) continue;
                    id = itemAsset.id;
                    text2 = itemAsset.itemName;
                    break;
                }
            }

            if (Assets.find((EAssetType) 1, id) == null)
            {
                message = Instance.Translate("could_not_find", components[0]);
                UnturnedChat.Say(playerid, message);
                return;
            }

            if (text2 == null && id != 0)
            {
                text2 = ((ItemAsset) Assets.find((EAssetType) 1, id)).itemName;
            }

            d = Instance.ShopDB.GetItemCost(id);
            var itemBuyPrice = Instance.ShopDB.GetItemBuyPrice(id);
            message = Instance.Translate("item_cost_msg", text2, d.ToString(),
                Uconomy.Instance.Configuration.Instance.MoneyName, itemBuyPrice.ToString(),
                Uconomy.Instance.Configuration.Instance.MoneyName);
            if (d <= 0m)
            {
                message = Instance.Translate("error_getting_cost", text2);
            }

            UnturnedChat.Say(playerid, message);
        }

        // Token: 0x06000037 RID: 55 RVA: 0x00004374 File Offset: 0x00002574
        public void Sell(UnturnedPlayer playerid, string[] components)
        {
            string message;
            if (components.Length == 0 || (components.Length > 0 && components[0].Trim() == string.Empty))
            {
                message = Instance.Translate("sell_command_usage");
                UnturnedChat.Say(playerid, message);
                return;
            }

            byte b = 1;
            if (components.Length > 1 && !byte.TryParse(components[1], out b))
            {
                message = Instance.Translate("invalid_amt");
                UnturnedChat.Say(playerid, message);
                return;
            }

            var b2 = b;
            if (!Instance.Configuration.Instance.CanSellItems)
            {
                message = Instance.Translate("sell_items_off");
                UnturnedChat.Say(playerid, message);
                return;
            }

            string text = null;
            ItemAsset itemAsset = null;
            ushort id;
            if (!ushort.TryParse(components[0], out id))
            {
                var array = Assets.find((EAssetType) 1);
                foreach (var asset in array)
                {
                    var searchAsset = (ItemAsset) asset;
                    if (searchAsset?.itemName == null ||
                        !searchAsset.itemName.ToLower().Contains(components[0].ToLower())) continue;
                    id = searchAsset.id;
                    text = searchAsset.itemName;
                    break;
                }
            }

            if (Assets.find((EAssetType) 1, id) == null)
            {
                message = Instance.Translate("could_not_find", components[0]);
                UnturnedChat.Say(playerid, message);
                return;
            }

            if (text == null && id != 0)
            {
                itemAsset = (ItemAsset) Assets.find((EAssetType) 1, id);
                text = itemAsset.itemName;
            }

            if (playerid.Inventory.has(id) == null)
            {
                message = Instance.Translate("not_have_item_sell", text);
                UnturnedChat.Say(playerid, message);
                return;
            }

            var list = playerid.Inventory.search(id, true, true);
            if (list.Count == 0 || (itemAsset.amount == 1 && list.Count < b))
            {
                message = Instance.Translate("not_enough_items_sell", b.ToString(), text);
                UnturnedChat.Say(playerid, message);
                return;
            }

            if (itemAsset.amount > 1)
            {
                var num = list.Aggregate(0, (current, inventorySearch) => current + inventorySearch.jar.item.amount);

                if (num < b)
                {
                    message = Instance.Translate("not_enough_ammo_sell", text);
                    UnturnedChat.Say(playerid, message);
                    return;
                }
            }

            var itemBuyPrice = Instance.ShopDB.GetItemBuyPrice(id);
            if (itemBuyPrice <= 0.00m)
            {
                message = Instance.Translate("no_sell_price_set", text);
                UnturnedChat.Say(playerid, message);
                return;
            }

            byte value = 100;
            var num2 = 0m;
            var amount = itemAsset.amount;
            if (amount != 1)
            {
                var value2 = b;
                while (b > 0)
                {
                    if (playerid.Player.equipment.checkSelection(list[0].page, list[0].jar.x, list[0].jar.y))
                    {
                        playerid.Player.equipment.dequip();
                    }

                    if (list[0].jar.item.amount >= b)
                    {
                        var b3 = (byte) (list[0].jar.item.amount - b);
                        list[0].jar.item.amount = b3;
                        playerid.Inventory.sendUpdateAmount(list[0].page, list[0].jar.x, list[0].jar.y, b3);
                        b = 0;
                        if (b3 == 0)
                        {
                            playerid.Inventory.removeItem(list[0].page,
                                playerid.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                            list.RemoveAt(0);
                        }
                    }
                    else
                    {
                        b -= list[0].jar.item.amount;
                        playerid.Inventory.sendUpdateAmount(list[0].page, list[0].jar.x, list[0].jar.y, 0);
                        playerid.Inventory.removeItem(list[0].page,
                            playerid.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                        list.RemoveAt(0);
                    }
                }

                var d = decimal.Round(itemBuyPrice * (value2 / itemAsset.amount), 2);
                num2 += d;
            }
            else
            {
                while (b > 0)
                {
                    if (playerid.Player.equipment.checkSelection(list[0].page, list[0].jar.x, list[0].jar.y))
                    {
                        playerid.Player.equipment.dequip();
                    }

                    if (Instance.Configuration.Instance.QualityCounts)
                    {
                        value = list[0].jar.item.durability;
                    }

                    var d = decimal.Round(itemBuyPrice * (value / 100.0m), 2);
                    num2 += d;
                    playerid.Inventory.removeItem(list[0].page,
                        playerid.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                    list.RemoveAt(0);
                    b -= 1;
                }
            }

            var num3 = Uconomy.Instance.Database.IncreaseBalance(playerid.CSteamID.ToString(), num2);
            message = Instance.Translate("sold_items", b2, text, num2,
                Uconomy.Instance.Configuration.Instance.MoneyName, num3,
                Uconomy.Instance.Configuration.Instance.MoneyName);
            if (Instance.OnShopSell != null)
            {
                Instance.OnShopSell(playerid, num2, b2, id);
            }

            playerid.Player.gameObject.SendMessage("ZaupShopOnSell", new object[]
            {
                playerid,
                num2,
                b2,
                id
            }, (SendMessageOptions) 1);
            UnturnedChat.Say(playerid, message);
        }

        // Token: 0x04000001 RID: 1
        public DatabaseMgr ShopDB;

        // Token: 0x04000002 RID: 2
        public static ZaupShop Instance;

        // Token: 0x02000008 RID: 8
        // (Invoke) Token: 0x06000039 RID: 57
        public delegate void PlayerShopBuy(UnturnedPlayer player, decimal amt, byte items, ushort item,
            string type = "item");

        // Token: 0x02000009 RID: 9
        // (Invoke) Token: 0x0600003D RID: 61
        public delegate void PlayerShopSell(UnturnedPlayer player, decimal amt, byte items, ushort item);
    }
}