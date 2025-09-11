using System;
using System.Collections.Generic;
using System.IO;
using static LADXHD_Patcher.XDelta3;

namespace LADXHD_Patcher
{
    internal class Functions
    {
        private static bool patchFromBackup;
        private static string Executable;
        private static string MD5Hash;

        private static Dictionary<string, object> resources = ResourceHelper.GetAllResources();

        private static string[] langFiles  = new[] { "esp.lng", "fre.lng", "ita.lng", "por.lng", "rus.lng" };
        private static string[] langDialog = new[] { "dialog_esp.lng", "dialog_fre.lng", "dialog_ita.lng", "dialog_por.lng", "dialog_rus.lng" };
        private static string[] smallFonts = new[] { "smallFont_redux.xnb", "smallFont_vwf.xnb", "smallFont_vwf_redux.xnb" };
        private static string[] backGround = new[] { "menuBackgroundB.xnb", "menuBackgroundC.xnb" };
        private static string[] npcImages  = new[] { "npcs_redux.png" };
        private static string[] itemImages = new[] { "items_redux.png" };

        private static readonly Dictionary<string, string[]> fileTargets = new Dictionary<string, string[]>
        {
            { "eng.lng",             langFiles },
            { "dialog_eng.lng",     langDialog },
            { "smallFont.xnb",      smallFonts },
            { "menuBackground.xnb", backGround },
            { "npcs.png",            npcImages },
            { "items.png",          itemImages }
        };

        public static bool InBackup(FileItem fileItem)
        {
            return (fileItem.DirectoryName.IndexOf("Data\\Backup", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static void HandleMultiFilePatches(FileItem fileItem)
        {
            if (!fileTargets.TryGetValue(fileItem.Name, out var targets))
                return;

            foreach (string newFile in targets)
            {
                if (!resources.ContainsKey(newFile))
                    continue;

                string xdelta3File = Path.Combine((Config.tempFolder + "\\patches").CreatePath(), newFile + ".xdelta");
                string patchedFile = Path.Combine((Config.tempFolder + "\\patchedFiles").CreatePath(), newFile);
                string newFilePath  = Path.Combine(fileItem.DirectoryName, newFile);

                File.WriteAllBytes(xdelta3File, (byte[])resources[newFile]);
                XDelta3.Execute(Operation.Apply, fileItem.FullName, xdelta3File, patchedFile, newFilePath);
            }
        }

        private static void PatchGameFiles()
        {
            foreach (string file in Config.baseFolder.GetFiles("*", true))
            {
                FileItem fileItem = new FileItem(file);

                if (fileItem.Name == "xdelta3.exe" || !resources.ContainsKey(fileItem.Name) || InBackup(fileItem))
                    continue;

                string backupFile = Path.Combine(Config.backupPath, fileItem.Name);

                if (backupFile.TestPath())
                    backupFile.CopyPath(fileItem.FullName,true);
                else
                    fileItem.FullName.CopyPath(backupFile, true);

                HandleMultiFilePatches(fileItem);

                string xdelta3File = Path.Combine((Config.tempFolder + "\\patches").CreatePath(), fileItem.Name + ".xdelta");
                string patchedFile = Path.Combine((Config.tempFolder + "\\patchedFiles").CreatePath(), fileItem.Name);

                File.WriteAllBytes(xdelta3File, (byte[])resources[fileItem.Name]);
                XDelta3.Execute(Operation.Apply, fileItem.FullName, xdelta3File, patchedFile, fileItem.FullName);
            }
            string message = patchFromBackup 
                ? "Patching the game from v1.0.0 backup files was successful. The game was updated to v"+ Config.version + "." 
                : "Patching Link's Awakening DX HD v1.0.0 was successful. The game was updated to v"+ Config.version + ".";
            Forms.okayDialog.Display("Patching Complete", 260, 40, 34, 16, 10, message);
        }

        private static void SetSourceFiles()
        {
            string backupExe = Path.Combine(Config.backupPath, "Link's Awakening DX HD.exe");
            patchFromBackup = backupExe.TestPath();
            Executable = patchFromBackup
                ? backupExe
                : Config.zeldaEXE;
            MD5Hash = Executable.CalculateHash("MD5");
        }

        private static bool ValidateExist()
        {
            if (!Executable.TestPath())
            {
                Forms.okayDialog.Display("Game Executable Not Found", 250, 40, 27, 10, 15, 
                    "Could not find \"Link's Awakening DX HD.exe\" to patch. Copy this patcher executable to the folder of the original release of v1.0.0 and run it from there.");
                return false;
            }
            return true;
        }

        private static bool ValidatePatch()
        {
            if (MD5Hash == Config.newHash)
            {
                Forms.okayDialog.Display("Already Patched", 260, 40, 30, 16, 10, 
                    "The game is already at v" + Config.version + " so no patching is needed. Close this patcher and launch the game!");
                return false;
            }
            return true;
        }

        private static bool ValidateKnown()
        {
            if (MD5Hash != Config.oldHash && MD5Hash != Config.newHash)
            {
                Forms.okayDialog.Display("Uknown Version", 260, 40, 26, 24, 10, 
                    "The version you are attempting to patch is unknown!");
                return false;
            }
            return true;
        }
        private static bool ValidateStart()
        {
            return Forms.yesNoDialog.Display("Patch to " + Config.version, 260, 20, 28, 24, true, 
                "Are you sure you wish to patch the game to v" + Config.version + "?");
        }

        public static void StartPatching()
        {
            SetSourceFiles();

            if (!ValidateExist()) return;
            if (!ValidatePatch()) return;
            if (!ValidateKnown()) return;
            if (!ValidateStart()) return;

            Forms.mainDialog.ToggleDialog(false);
            Config.tempFolder.CreatePath(true);

            XDelta3.Create();
            PatchGameFiles();
            XDelta3.Remove();

            Config.tempFolder.RemovePath();
            Forms.mainDialog.ToggleDialog(true);
        }
    }
}
