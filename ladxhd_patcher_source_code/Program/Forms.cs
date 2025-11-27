using System.Drawing;
using System.Windows.Forms;

namespace LADXHD_Patcher
{
    internal class Forms
    {
        public static Form_MainForm  mainDialog;
        public static Form_OkayForm  okayDialog; 
        public static Form_YesNoForm yesNoDialog; 

        public static void Initialize()
        {
            mainDialog  = new Form_MainForm();
            okayDialog  = new Form_OkayForm();
            yesNoDialog = new Form_YesNoForm();
        }

        public static void CreatePatcherText()
        {
            // Set the title including the version number.
            mainDialog.Text = "Link's Awakening DX HD Patcher v" + Config.version;

            // Transparent overlay label
            mainDialog.TextBox_NoClick = new TransparentLabel
            {
                Text      = "",
                Size      = new Size(326, 114),
                Location  = new Point(10, 14),
                TabIndex  = 16
            };
            mainDialog.groupBox_Main.Controls.Add(mainDialog.TextBox_NoClick);

            // The Advanced RichTextBox allows for justified text.
            mainDialog.TextBox_Info = new AdvRichTextBox
            {
                Size        = new Size(326, 114),
                Location    = new Point(10, 14),
                TabStop     = false,
                BorderStyle = BorderStyle.None,
                ReadOnly    = true,
                BackColor   = ColorTranslator.FromHtml("#F0F0F0")
            };

            // Build text
            string header = "Legend of Zelda: Link's Awakening DX HD v" + Config.version;
            string body   =
                "\n\nPatches v1.0.0 (or v1.1.4+) to v" + Config.version + " with the \"Patch\" button " +
                "below. All patchers created since v1.1.4 back up the original " +
                "files so that all future patches no longer require v1.0.0. When updating " +
                "with this version of the patcher, future versions of the " +
                "patcher can use the stored backup files. Backups are stored in the " +
                "\"Data\\Backup\" folder. Do not move or delete them! ";

            mainDialog.TextBox_Info.Text = header + body;

            // ----- Bold the header -----
            mainDialog.TextBox_Info.Select(0, header.Length);
            mainDialog.TextBox_Info.SelectionFont = new Font(
                mainDialog.TextBox_Info.Font, FontStyle.Bold);

            // Reset selection so the rest of the text is not bold
            mainDialog.TextBox_Info.Select(mainDialog.TextBox_Info.TextLength, 0);
            mainDialog.TextBox_Info.SelectionFont = new Font(
                mainDialog.TextBox_Info.Font, FontStyle.Regular);

            // Apply justification after text is added
            mainDialog.TextBox_Info.SelectionAlignment = TextAlign.Justify;

            // Add to form
            mainDialog.groupBox_Main.Controls.Add(mainDialog.TextBox_Info);
        }
    }
}