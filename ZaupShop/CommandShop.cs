using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace ZaupShop
{
    // Token: 0x02000005 RID: 5
    public class CommandShop : IRocketCommand
    {
        // Token: 0x17000013 RID: 19
        // (get) Token: 0x0600001A RID: 26 RVA: 0x0000211B File Offset: 0x0000031B
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        // Token: 0x17000014 RID: 20
        // (get) Token: 0x0600001B RID: 27 RVA: 0x0000211E File Offset: 0x0000031E
        public string Name => "shop";

        // Token: 0x17000015 RID: 21
        // (get) Token: 0x0600001C RID: 28 RVA: 0x00002125 File Offset: 0x00000325
        public string Help => "Allows admins to change, add, or remove items/vehicles from the shop.";

        // Token: 0x17000016 RID: 22
        // (get) Token: 0x0600001D RID: 29 RVA: 0x0000212C File Offset: 0x0000032C
        public string Syntax => "<add | rem | chng | buy> [v.]<itemid> <cost>";

        // Token: 0x17000017 RID: 23
        // (get) Token: 0x0600001E RID: 30 RVA: 0x00002133 File Offset: 0x00000333
        public List<string> Aliases => new List<string>();

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x0600001F RID: 31 RVA: 0x0000213C File Offset: 0x0000033C
        public List<string> Permissions =>
            new List<string>
            {
                "shop.*",
                "shop.add",
                "shop.rem",
                "shop.chng",
                "shop.buy"
            };

        // Token: 0x06000020 RID: 32 RVA: 0x00002188 File Offset: 0x00000388
        public void Execute(IRocketPlayer caller, string[] msg)
        {
            bool flag = caller is ConsolePlayer;
            string[] array = new string[]
            {
                "shop.*",
                "shop.add",
                "shop.rem",
                "shop.chng",
                "shop.buy"
            };
            bool[] array2 = new bool[5];
            bool flag2 = false;
            foreach (var name in caller.GetPermissions().Select(permission => permission.Name)
                .Where(name => name != null))
            {
                if (name != "shop.*")
                {
                    if (name != "shop.add")
                    {
                        if (name != "shop.rem")
                        {
                            if (name != "shop.chng")
                            {
                                if (name != "shop.buy")
                                {
                                    if (name == "*")
                                    {
                                        array2[0] = true;
                                        array2[1] = true;
                                        array2[2] = true;
                                        array2[3] = true;
                                        array2[4] = true;
                                        flag2 = true;
                                    }
                                }
                                else
                                {
                                    array2[4] = true;
                                    flag2 = true;
                                }
                            }
                            else
                            {
                                array2[3] = true;
                                flag2 = true;
                            }
                        }
                        else
                        {
                            array2[2] = true;
                            flag2 = true;
                        }
                    }
                    else
                    {
                        array2[1] = true;
                        flag2 = true;
                    }
                }
                else
                {
                    array2[0] = true;
                    flag2 = true;
                }
            }

            if (!flag && ((UnturnedPlayer) caller).IsAdmin)
            {
                array2[0] = true;
                array2[1] = true;
                array2[2] = true;
                array2[3] = true;
                array2[4] = true;
                flag2 = true;
            }

            if (!flag2)
            {
                UnturnedChat.Say(caller, "You don't have permission to use the /shop command.");
                return;
            }

            if (msg.Length == 0)
            {
                string message = ZaupShop.Instance.Translate("shop_command_usage");
                sendMessage(caller, message, flag);
                return;
            }

            if (msg.Length < 2)
            {
                string message = ZaupShop.Instance.Translate("no_itemid_given");
                sendMessage(caller, message, flag);
                return;
            }

            if (msg.Length == 2 && msg[0] != "rem")
            {
                string message = ZaupShop.Instance.Translate("no_cost_given");
                sendMessage(caller, message, flag);
                return;
            }

            if (msg.Length >= 2)
            {
                string[] componentsFromSerial = Parser.getComponentsFromSerial(msg[1], '.');
                string message;
                if (componentsFromSerial.Length > 1 && componentsFromSerial[0] != "v")
                {
                    message = ZaupShop.Instance.Translate("v_not_provided");
                    sendMessage(caller, message, flag);
                    return;
                }

                ushort num;
                if (componentsFromSerial.Length > 1)
                {
                    if (!ushort.TryParse(componentsFromSerial[1], out num))
                    {
                        message = ZaupShop.Instance.Translate("invalid_id_given");
                        sendMessage(caller, message, flag);
                        return;
                    }
                }
                else if (!ushort.TryParse(componentsFromSerial[0], out num))
                {
                    message = ZaupShop.Instance.Translate("invalid_id_given");
                    sendMessage(caller, message, flag);
                    return;
                }

                bool change = false;
                bool flag3 = false;
                string text = msg[0];
                if (text != null)
                {
                    if (!(text == "chng"))
                    {
                        if (!(text == "add"))
                        {
                            if (!(text == "rem"))
                            {
                                if (!(text == "buy"))
                                {
                                    goto IL_85B;
                                }

                                if (!array2[4] && !array2[0])
                                {
                                    message = ZaupShop.Instance.Translate("no_permission_shop_buy");
                                    sendMessage(caller, message, flag);
                                    return;
                                }

                                if (!IsAsset(num, "i"))
                                {
                                    message = ZaupShop.Instance.Translate("invalid_id_given");
                                    sendMessage(caller, message, flag);
                                    return;
                                }

                                ItemAsset itemAsset = (ItemAsset) Assets.find((EAssetType) 1, num);
                                decimal cost;
                                decimal.TryParse(msg[2], out cost);
                                message = ZaupShop.Instance.Translate("set_buyback_price", itemAsset.itemName, cost.ToString());
                                if (!ZaupShop.Instance.ShopDB.SetBuyPrice(num, cost))
                                {
                                    message = ZaupShop.Instance.Translate("not_in_shop_to_buyback", itemAsset.itemName);
                                }

                                sendMessage(caller, message, flag);
                                return;
                            }
                            else
                            {
                                if (!array2[2] && !array2[0])
                                {
                                    message = ZaupShop.Instance.Translate("no_permission_shop_rem");
                                    sendMessage(caller, message, flag);
                                    return;
                                }

                                string text2 = componentsFromSerial[0];
                                if (text2 != null)
                                {
                                    if (text2 == "v")
                                    {
                                        if (!IsAsset(num, "v"))
                                        {
                                            message = ZaupShop.Instance.Translate("invalid_id_given");
                                            sendMessage(caller, message, flag);
                                            return;
                                        }

                                        VehicleAsset vehicleAsset = (VehicleAsset) Assets.find((EAssetType) 5, num);
                                        message = ZaupShop.Instance.Translate("removed_from_shop", vehicleAsset.vehicleName);
                                        if (!ZaupShop.Instance.ShopDB.DeleteVehicle(num))
                                        {
                                            message = ZaupShop.Instance.Translate("not_in_shop_to_remove", vehicleAsset.vehicleName);
                                        }

                                        sendMessage(caller, message, flag);
                                        goto IL_759;
                                    }
                                }

                                if (!IsAsset(num, "i"))
                                {
                                    message = ZaupShop.Instance.Translate("invalid_id_given");
                                    sendMessage(caller, message, flag);
                                    return;
                                }

                                ItemAsset itemAsset2 = (ItemAsset) Assets.find((EAssetType) 1, num);
                                message = ZaupShop.Instance.Translate("removed_from_shop", itemAsset2.itemName);
                                if (!ZaupShop.Instance.ShopDB.DeleteItem(num))
                                {
                                    message = ZaupShop.Instance.Translate("not_in_shop_to_remove", itemAsset2.itemName);
                                }

                                sendMessage(caller, message, flag);
                                IL_759:
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (!array2[3] && !array2[0])
                        {
                            message = ZaupShop.Instance.Translate("no_permission_shop_chng");
                            sendMessage(caller, message, flag);
                            return;
                        }

                        change = true;
                        flag3 = true;
                    }

                    if (!flag3 && !array2[1] && !array2[0])
                    {
                        message = ZaupShop.Instance.Translate("no_permission_shop_add");
                        sendMessage(caller, message, flag);
                        return;
                    }

                    string text3 = (!flag3)
                        ? ZaupShop.Instance.Translate("added")
                        : ZaupShop.Instance.Translate("changed");
                    string text4 = componentsFromSerial[0];
                    if (text4 != null)
                    {
                        if (text4 == "v")
                        {
                            if (!IsAsset(num, "v"))
                            {
                                message = ZaupShop.Instance.Translate("invalid_id_given");
                                sendMessage(caller, message, flag);
                                return;
                            }

                            VehicleAsset vehicleAsset2 = (VehicleAsset) Assets.find((EAssetType)5, num);
                            message = ZaupShop.Instance.Translate("changed_or_added_to_shop", text3, vehicleAsset2.vehicleName, msg[2]);
                            if (!ZaupShop.Instance.ShopDB.AddVehicle(num, vehicleAsset2.vehicleName,
                                decimal.Parse(msg[2]), change))
                            {
                                message = ZaupShop.Instance.Translate("error_adding_or_changing", vehicleAsset2.vehicleName);
                            }

                            sendMessage(caller, message, flag);
                            goto IL_5A3;
                        }
                    }

                    if (!IsAsset(num, "i"))
                    {
                        message = ZaupShop.Instance.Translate("invalid_id_given");
                        sendMessage(caller, message, flag);
                        return;
                    }

                    ItemAsset itemAsset3 = (ItemAsset) Assets.find((EAssetType)1, num);
                    message = ZaupShop.Instance.Translate("changed_or_added_to_shop", text3, itemAsset3.itemName, msg[2]);
                    if (!ZaupShop.Instance.ShopDB.AddItem(num, itemAsset3.itemName, decimal.Parse(msg[2]),
                        change))
                    {
                        message = ZaupShop.Instance.Translate("error_adding_or_changing", itemAsset3.itemName);
                    }

                    sendMessage(caller, message, flag);
                    IL_5A3:
                    return;
                }

                IL_85B:
                message = ZaupShop.Instance.Translate("not_in_shop_to_remove");
                sendMessage(caller, message, flag);
                return;
            }
        }

        // Token: 0x06000021 RID: 33 RVA: 0x00002A24 File Offset: 0x00000C24
        private bool IsAsset(ushort id, string type)
        {
            switch (type)
            {
                case null:
                    return false;
                case "i":
                    return Assets.find((EAssetType) 1, id) != null;
                case "v":
                    return Assets.find((EAssetType) 5, id) != null;
                default:
                    return false;
            }
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00002A7D File Offset: 0x00000C7D
        private void sendMessage(IRocketPlayer caller, string message, bool console)
        {
            if (console)
            {
                Logger.Log(message);
            }
            else
            {
                UnturnedChat.Say(caller, message);
            }
        }
    }
}