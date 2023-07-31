using TShockAPI;

namespace WorldEdit.Commands
{
	public class Copy : WECommand
	{
        string save;
		public Copy(int x, int y, int x2, int y2, TSPlayer plr, string save, string action)
			: base(x, y, x2, y2, plr, action)
		{
            this.save = save;
		}

		public override void Execute()
		{
			if (!CanUseCommand()) { return; }

			string clipboardPath = Tools.GetClipboardPath(plr.Account.ID);
			
			Tools.SaveWorldSection(x, y, x2, y2, save ?? clipboardPath);

            plr.SendSuccessMessage("Copied selection to {0}.", save == null ? "clipboard" : "schematic");

			base.Execute(); // Is it really necessary?
		}
	}
}