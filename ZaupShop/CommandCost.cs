using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace ZaupShop
{
	// Token: 0x02000003 RID: 3
	public class CommandCost : IRocketCommand
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002099 File Offset: 0x00000299
		public AllowedCaller AllowedCaller => AllowedCaller.Player;

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000209C File Offset: 0x0000029C
		public string Name => "cost";

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000020A3 File Offset: 0x000002A3
		public string Help => "Tells you the cost of a selected item.";

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000020AA File Offset: 0x000002AA
		public string Syntax => "[v.]<name or id>";

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000020B1 File Offset: 0x000002B1
		public List<string> Aliases => new List<string>();

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000020B8 File Offset: 0x000002B8
		public List<string> Permissions => new List<string>();

		// Token: 0x06000010 RID: 16 RVA: 0x000020BF File Offset: 0x000002BF
		public void Execute(IRocketPlayer playerid, string[] msg)
		{
			ZaupShop.Instance.Cost((UnturnedPlayer)playerid, msg);
		}
	}
}
