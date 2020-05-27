using System;
using fr34kyn01535.Uconomy;
using I18N.West;
using MySql.Data.MySqlClient;
using Rocket.Core.Logging;

namespace ZaupShop
{
    // Token: 0x02000006 RID: 6
    public class DatabaseMgr
    {
        // Token: 0x06000023 RID: 35 RVA: 0x00002A9C File Offset: 0x00000C9C
        internal DatabaseMgr()
        {
            var cp = new CP1250();
            CheckSchema();
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00002ABC File Offset: 0x00000CBC
        internal void CheckSchema()
        {
            try
            {
                var mySqlConnection = createConnection();
                var mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = "show tables like '" +
                                           ZaupShop.Instance.Configuration.Instance.ItemShopTableName + "'";
                mySqlConnection.Open();
                if (mySqlCommand.ExecuteScalar() == null)
                {
                    mySqlCommand.CommandText = "CREATE TABLE `" +
                                               ZaupShop.Instance.Configuration.Instance.ItemShopTableName +
                                               "` (`id` int(6) NOT NULL,`itemname` varchar(32) NOT NULL,`cost` decimal(15,2) NOT NULL DEFAULT '20.00',`buyback` decimal(15,2) NOT NULL DEFAULT '0.00',PRIMARY KEY (`id`)) ";
                    mySqlCommand.ExecuteNonQuery();
                }

                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }

            try
            {
                var mySqlConnection2 = createConnection();
                var mySqlCommand2 = mySqlConnection2.CreateCommand();
                mySqlCommand2.CommandText = "show tables like '" +
                                            ZaupShop.Instance.Configuration.Instance.VehicleShopTableName + "'";
                mySqlConnection2.Open();
                if (mySqlCommand2.ExecuteScalar() == null)
                {
                    mySqlCommand2.CommandText = "CREATE TABLE `" +
                                                ZaupShop.Instance.Configuration.Instance.VehicleShopTableName +
                                                "` (`id` int(6) NOT NULL,`vehiclename` varchar(32) NOT NULL,`cost` decimal(15,2) NOT NULL DEFAULT '100.00',PRIMARY KEY (`id`)) ";
                    mySqlCommand2.ExecuteNonQuery();
                }

                mySqlConnection2.Close();
            }
            catch (Exception ex2)
            {
                Logger.LogException(ex2, null);
            }

            try
            {
                var mySqlConnection3 = createConnection();
                var mySqlCommand3 = mySqlConnection3.CreateCommand();
                mySqlCommand3.CommandText = "show columns from `" +
                                            ZaupShop.Instance.Configuration.Instance.ItemShopTableName +
                                            "` like 'buyback'";
                mySqlConnection3.Open();
                if (mySqlCommand3.ExecuteScalar() == null)
                {
                    mySqlCommand3.CommandText = "ALTER TABLE `" +
                                                ZaupShop.Instance.Configuration.Instance.ItemShopTableName +
                                                "` ADD `buyback` decimal(15,2) NOT NULL DEFAULT '0.00'";
                    mySqlCommand3.ExecuteNonQuery();
                }

                mySqlConnection3.Close();
            }
            catch (Exception ex3)
            {
                Logger.LogException(ex3, null);
            }
        }

        // Token: 0x06000025 RID: 37 RVA: 0x00002CB4 File Offset: 0x00000EB4
        private MySqlConnection createConnection()
        {
            MySqlConnection result = null;
            try
            {
                if (Uconomy.Instance.Configuration.Instance.DatabasePort == 0)
                {
                    Uconomy.Instance.Configuration.Instance.DatabasePort = 3306;
                }

                result = new MySqlConnection(
                    $"SERVER={Uconomy.Instance.Configuration.Instance.DatabaseAddress};" +
                    $"DATABASE={Uconomy.Instance.Configuration.Instance.DatabaseName};" +
                    $"UID={Uconomy.Instance.Configuration.Instance.DatabaseUsername};" +
                    $"PASSWORD={Uconomy.Instance.Configuration.Instance.DatabasePassword};" +
                    $"PORT={Uconomy.Instance.Configuration.Instance.DatabasePort};");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }

            return result;
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00002DA8 File Offset: 0x00000FA8
        public bool AddItem(int id, string name, decimal cost, bool change)
        {
            bool result;
            try
            {
                var mySqlConnection = createConnection();
                var mySqlCommand = mySqlConnection.CreateCommand();
                if (!change)
                {
                    mySqlCommand.CommandText = string.Concat("Insert into `",
                        ZaupShop.Instance.Configuration.Instance.ItemShopTableName,
                        "` (`id`, `itemname`, `cost`) VALUES ('", id.ToString(), "', '", name, "', '", cost.ToString(),
                        "');");
                }
                else
                {
                    mySqlCommand.CommandText = string.Concat("update `",
                        ZaupShop.Instance.Configuration.Instance.ItemShopTableName, "` set itemname='", name,
                        "', cost='", cost.ToString(), "' where id='", id.ToString(), "';");
                }

                mySqlConnection.Open();
                var num = mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
                if (num > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
                result = false;
            }

            return result;
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002F10 File Offset: 0x00001110
        public bool AddVehicle(int id, string name, decimal cost, bool change)
        {
            bool result;
            try
            {
                var mySqlConnection = createConnection();
                var mySqlCommand = mySqlConnection.CreateCommand();
                if (!change)
                {
                    mySqlCommand.CommandText = string.Concat("Insert into `",
                        ZaupShop.Instance.Configuration.Instance.VehicleShopTableName,
                        "` (`id`, `vehiclename`, `cost`) VALUES ('", id.ToString(), "', '", name, "', '",
                        cost.ToString(), "');");
                }
                else
                {
                    mySqlCommand.CommandText = string.Concat("update `",
                        ZaupShop.Instance.Configuration.Instance.VehicleShopTableName, "` set vehiclename='", name,
                        "', cost='", cost.ToString(), "' where id='", id.ToString(), "';");
                }

                mySqlConnection.Open();
                var num = mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
                if (num > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
                result = false;
            }

            return result;
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00003078 File Offset: 0x00001278
        public decimal GetItemCost(int id)
        {
            var result = 0m;
            try
            {
                var mySqlConnection = createConnection();
                var mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat("select `cost` from `",
                    ZaupShop.Instance.Configuration.Instance.ItemShopTableName, "` where `id` = '", id.ToString(),
                    "';");
                mySqlConnection.Open();
                var obj = mySqlCommand.ExecuteScalar();
                if (obj != null)
                {
                    decimal.TryParse(obj.ToString(), out result);
                }

                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }

            return result;
        }

        // Token: 0x06000029 RID: 41 RVA: 0x00003138 File Offset: 0x00001338
        public decimal GetVehicleCost(int id)
        {
            var result = 0m;
            try
            {
                var mySqlConnection = createConnection();
                var mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat("select `cost` from `",
                    ZaupShop.Instance.Configuration.Instance.VehicleShopTableName, "` where `id` = '", id.ToString(),
                    "';");
                mySqlConnection.Open();
                var obj = mySqlCommand.ExecuteScalar();
                if (obj != null)
                {
                    decimal.TryParse(obj.ToString(), out result);
                }

                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }

            return result;
        }

        // Token: 0x0600002A RID: 42 RVA: 0x000031F8 File Offset: 0x000013F8
        public bool DeleteItem(int id)
        {
            bool result;
            try
            {
                var mySqlConnection = createConnection();
                var mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat("delete from `",
                    ZaupShop.Instance.Configuration.Instance.ItemShopTableName, "` where id='", id.ToString(), "';");
                mySqlConnection.Open();
                var num = mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
                if (num > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
                result = false;
            }

            return result;
        }

        // Token: 0x0600002B RID: 43 RVA: 0x000032B0 File Offset: 0x000014B0
        public bool DeleteVehicle(int id)
        {
            bool result;
            try
            {
                var mySqlConnection = createConnection();
                var mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat("delete from `",
                    ZaupShop.Instance.Configuration.Instance.VehicleShopTableName, "` where id='", id.ToString(), "';");
                mySqlConnection.Open();
                var num = mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
                if (num > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
                result = false;
            }

            return result;
        }

        // Token: 0x0600002C RID: 44 RVA: 0x00003368 File Offset: 0x00001568
        public bool SetBuyPrice(int id, decimal cost)
        {
            bool result;
            try
            {
                var mySqlConnection = createConnection();
                var mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat("update `",
                    ZaupShop.Instance.Configuration.Instance.ItemShopTableName, "` set `buyback`='", cost.ToString(),
                    "' where id='", id.ToString(), "';");
                mySqlConnection.Open();
                var num = mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
                if (num > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
                result = false;
            }

            return result;
        }

        // Token: 0x0600002D RID: 45 RVA: 0x00003438 File Offset: 0x00001638
        public decimal GetItemBuyPrice(int id)
        {
            var result = 0m;
            try
            {
                var mySqlConnection = createConnection();
                var mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat("select `buyback` from `",
                    ZaupShop.Instance.Configuration.Instance.ItemShopTableName, "` where `id` = '", id.ToString(),
                    "';");
                mySqlConnection.Open();
                var obj = mySqlCommand.ExecuteScalar();
                if (obj != null)
                {
                    decimal.TryParse(obj.ToString(), out result);
                }

                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, null);
            }

            return result;
        }
    }
}