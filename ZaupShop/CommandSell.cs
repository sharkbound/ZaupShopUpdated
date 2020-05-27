using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace ZaupShop
{
	// Token: 0x02000004 RID: 4
	public class CommandSell : IRocketCommand
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000020DA File Offset: 0x000002DA
		public AllowedCaller AllowedCaller => AllowedCaller.Player;

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000020DD File Offset: 0x000002DD
		public string Name => "sell";

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000014 RID: 20 RVA: 0x000020E4 File Offset: 0x000002E4
		public string Help => "Allows you to sell items to the shop from your inventory.";

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000020EB File Offset: 0x000002EB
		public string Syntax => "<name or id> [amount]";

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000016 RID: 22 RVA: 0x000020F2 File Offset: 0x000002F2
		public List<string> Aliases => new List<string>();

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000020F9 File Offset: 0x000002F9
		public List<string> Permissions => new List<string>();

		// Token: 0x06000018 RID: 24 RVA: 0x00002100 File Offset: 0x00000300
		public void Execute(IRocketPlayer playerid, string[] msg)
		{
			ZaupShop.Instance.Sell((UnturnedPlayer)playerid, msg);
		}
	}
}
