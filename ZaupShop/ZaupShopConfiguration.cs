using System;
using Rocket.API;

namespace ZaupShop
{
	// Token: 0x0200000A RID: 10
	public class ZaupShopConfiguration : IRocketPluginConfiguration, IDefaultable
	{
		// Token: 0x06000041 RID: 65 RVA: 0x00004B38 File Offset: 0x00002D38
		public void LoadDefaults()
		{
			ItemShopTableName = "uconomyitemshop";
			VehicleShopTableName = "uconomyvehicleshop";
			CanBuyItems = true;
			CanBuyVehicles = false;
			CanSellItems = true;
			QualityCounts = true;
		}

		// Token: 0x04000005 RID: 5
		public string ItemShopTableName;

		// Token: 0x04000006 RID: 6
		public string VehicleShopTableName;

		// Token: 0x04000007 RID: 7
		public bool CanBuyItems;

		// Token: 0x04000008 RID: 8
		public bool CanBuyVehicles;

		// Token: 0x04000009 RID: 9
		public bool CanSellItems;

		// Token: 0x0400000A RID: 10
		public bool QualityCounts;
	}
}
