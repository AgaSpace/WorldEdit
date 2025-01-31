﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands
{
	public class Outline : WECommand
	{
		private Expression expression;
		private TilePlaceID tileType;
		private int color;
		private bool active;

		public Outline(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, 
			TilePlaceID tileType, int color, bool active, Expression expression, string action)
			: base(x, y, x2, y2, magicWand, plr, action)
		{
			this.tileType = tileType;
			this.color = color;
			this.active = active;
			this.expression = expression ?? new TestExpression(new Test(t => true));
		}

		public override void Execute()
		{
			if (x < 1) x = 1;
			else if (x > (Main.maxTilesX - 2)) x = (Main.maxTilesX - 2);
			if (y < 1) y = 1;
			else if (y > (Main.maxTilesY - 2)) y = (Main.maxTilesY - 2);
			if (x2 < 1) x2 = 1;
			else if (x2 > (Main.maxTilesX - 2)) x2 = (Main.maxTilesX - 2);
			if (y2 < 1) y2 = 1;
			else if (y2 > (Main.maxTilesY - 2)) y2 = (Main.maxTilesY - 2);

            if (!CanUseCommand(x - 1, y - 1, x2 + 1, y2 + 1)) { return; }
            Tools.PrepareUndo(x - 1, y - 1, x2 + 1, y2 + 1, plr);
			int edits = 0;
			List<Point> tiles = new List<Point>();

			for (int i = x; i <= x2; i++)
			{
				for (int j = y; j <= y2; j++)
				{
					var tile = Main.tile[i, j];
					bool XY = tile.active();

					bool mXmY = Main.tile[i - 1, j - 1].active();
					bool mXpY = Main.tile[i - 1, j + 1].active();
					bool mXY = Main.tile[i - 1, j].active();
					bool pXmY = Main.tile[i + 1, j - 1].active();
					bool pXpY = Main.tile[i + 1, j + 1].active();
					bool pXY = Main.tile[i + 1, j].active();
					bool XmY = Main.tile[i, j - 1].active();
					bool XpY = Main.tile[i, j + 1].active();

					if (XY && expression.Evaluate(Main.tile[i, j]) && magicWand.InSelection(i, j))
					{
						if (!mXmY)
						{
							tiles.Add(new Point(i - 1, j - 1));
							edits++;
						}
						if (!XmY)
						{
							tiles.Add(new Point(i, j - 1));
							edits++;
						}
						if (!pXmY)
						{
							tiles.Add(new Point(i + 1, j - 1));
							edits++;
						}
						if (!mXY)
						{
							tiles.Add(new Point(i - 1, j));
							edits++;
						}
						if (!pXY)
						{
							tiles.Add(new Point(i + 1, j));
							edits++;
						}
						if (!mXpY)
						{
							tiles.Add(new Point(i - 1, j + 1));
							edits++;
						}
						if (!XpY)
						{
							tiles.Add(new Point(i, j + 1));
							edits++;
						}
						if (!pXpY)
						{
							tiles.Add(new Point(i + 1, j + 1));
							edits++;
						}
					}
				}
			}

			foreach (Point p in tiles)
			{
				var tile = Main.tile[p.X, p.Y];
				tile.color((byte)color);
				tile.inActive(!active);
				tileType.SetTile(p.X, p.Y);
			}

			ResetSection();
			plr.SendSuccessMessage($"Outlined with {tileType.Name}. ({edits})");
			base.Execute();
		}
	}
}