using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace ZaupShop
{
	// Token: 0x02000002 RID: 2
	public class CommandBuy : IRocketCommand
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public AllowedCaller AllowedCaller => AllowedCaller.Player;

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000205B File Offset: 0x0000025B
		public string Name => "buy";

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002062 File Offset: 0x00000262
		public string Help => "Allows you to buy items from the shop.";

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002069 File Offset: 0x00000269
		public string Syntax => "[v.]<name or id> [amount] [25 | 50 | 75 | 100]";

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002070 File Offset: 0x00000270
		public List<string> Aliases => new List<string>();

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002077 File Offset: 0x00000277
		public List<string> Permissions => new List<string>();

		// Token: 0x06000008 RID: 8 RVA: 0x0000207E File Offset: 0x0000027E
		public void Execute(IRocketPlayer playerid, string[] msg)
		{
			ZaupShop.Instance.Buy((UnturnedPlayer)playerid, msg);
		}
	}
}
