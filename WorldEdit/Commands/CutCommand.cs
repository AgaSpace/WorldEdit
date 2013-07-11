﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands
{
	public class CutCommand : WECommand
	{
		public CutCommand(int x, int y, int x2, int y2, TSPlayer plr)
			: base(x, y, x2, y2, plr)
		{
		}

		public override void Execute()
		{
			string clipboardPath = Tools.GetClipboardPath(plr);
			using (BinaryWriter writer = new BinaryWriter(new FileStream(clipboardPath, FileMode.Create)))
			{
				writer.Write(x2 - x + 1);
				writer.Write(y2 - y + 1);
				for (int i = x; i <= x2; i++)
				{
					for (int j = y; j <= y2; j++)
					{
						Tools.WriteTile(writer, Main.tile[i, j]);
					}
				}
			}
			Tools.PrepareUndo(x, y, x2, y2, plr);
			int edits = 0;
			for (int i = x; i <= x2; i++)
			{
				for (int j = y; j <= y2; j++)
				{
					if (Main.tile[i, j].active || Main.tile[i, j].wall > 0 || Main.tile[i, j].liquid > 0 || Main.tile[i, j].wire)
					{
						Main.tile[i, j].active = false;
						Main.tile[i, j].lava = false;
						Main.tile[i, j].liquid = 0;
						Main.tile[i, j].type = 0;
						Main.tile[i, j].wall = 0;
						Main.tile[i, j].wire = false;
						edits++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage(String.Format("Cut selection. ({0})", edits));
		}
	}
}
