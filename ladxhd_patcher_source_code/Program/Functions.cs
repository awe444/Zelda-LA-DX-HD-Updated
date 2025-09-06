using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace LADXHD_Patcher
{
    internal class Functions
    {
        private static bool patchFromBackup;
        private static Dictionary<string, object> resources = ResourceHelper.GetAllResources();

        private static string[] languageFiles  = new[] {        "esp",        "fre",        "ita",        "por",        "rus" };
        private static string[] languageDialog = new[] { "dialog_esp", "dialog_fre", "dialog_ita", "dialog_por", "dialog_rus" };
        private static void LanguagePatches(FileItem fileItem)
        {
            // Get the kind of patch file we want to create.
            string[] target = null;
            if (fileItem.Name == "eng.lng")
                target = languageFiles;
            else if (fileItem.Name == "dialog_eng.lng")
                target = languageDialog;

            if (target == null) return;

            foreach (string lang in target)
            {
                // Create the patched language file.
                string langFile = lang + fileItem.Extension;
                string xdelta3File = Path.Combine((Config.tempFolder + "\\patches").CreatePath(), langFile + ".xdelta");
                string patchedFile = Path.Combine((Config.tempFolder + "\\patchedFiles").CreatePath(), langFile);
                string targetPath  = Path.Combine(fileItem.DirectoryName, langFile);
                XDelta3.Patch(fileItem.FullName, langFile, xdelta3File, patchedFile, targetPath);
            }
        }

        private static string[] smallFonts = new[] { "smallFont_redux", "smallFont_vwf", "smallFont_vwf_redux" };
        private static void HandleSmallFonts(FileItem fileItem)
        {
            // The "smallFonts" array contains the alternate variations.
            if (fileItem.BaseName == "smallFont")
            {
                foreach (string sfont in smallFonts)
                {
                    // Create the patched language file.
                    string sfontFile = sfont + fileItem.Extension;
                    string xdelta3File = Path.Combine((Config.tempFolder + "\\patches").CreatePath(), sfontFile + ".xdelta");
                    string patchedFile = Path.Combine((Config.tempFolder + "\\patchedFiles").CreatePath(), sfontFile);
                    string targetPath  = Path.Combine(fileItem.DirectoryName, sfontFile);
                    XDelta3.Patch(fileItem.FullName, sfontFile, xdelta3File, patchedFile, targetPath);
                }
            }
        }

        private static string[] specialFile   = new[] {    "menuBackground.xnb",       "npcs.png",       "items.png" };
        private static string[] specialTarget = new[] { "menuBackgroundAlt.xnb", "npcs_redux.png", "items_redux.png" };
        private static void HandleSpecialCases(FileItem fileItem)
        {
            // The "specialFile" array contains files that have alternate variations.
            if (specialFile.Contains(fileItem.Name))
            {
                // Use the index of the file in the "specialFile" array to pull the target out of "specialTarget" array.
                int index = Array.IndexOf(specialFile, fileItem.Name);
                string sFile = specialTarget[index];

                // Create the patched file.
                string xdelta3File = Path.Combine((Config.tempFolder + "\\patches").CreatePath(), sFile + ".xdelta");
                string patchedFile = Path.Combine((Config.tempFolder + "\\patchedFiles").CreatePath(), sFile);
                string targetPath  = Path.Combine(fileItem.DirectoryName, sFile);
                XDelta3.Patch(fileItem.FullName, sFile, xdelta3File, patchedFile, targetPath);
            }
        }

        private static void PatchGameFiles()
        {
            foreach (string file in Config.baseFolder.GetFiles("*", true))
            {
                // My "fileItem" class is great for grabbing specific file/folder information.
                FileItem fileItem = new FileItem(file);
                FileItem folderItem = new FileItem(fileItem.DirectoryName);

                // If we have a file that is required to create an alternate version of the file.
                HandleSpecialCases(fileItem);

                // We skip backup files and languages other than english here as they are patched from english files. We
                // also skip the xdelta3 executable for obvious reasons, and we skip the file if there is no patch for it.
                if (languageFiles.Contains(fileItem.BaseName) || languageDialog.Contains(fileItem.BaseName) ||
                    folderItem.Name == "Backup" || fileItem.Name == "xdelta3.exe" || (!resources.ContainsKey(fileItem.Name)))
                    continue;

                // If a backup file exists, restore it. If it doesn't exist, create one.
                string backupFile = Path.Combine(Config.backupPath, fileItem.Name);
                if (backupFile.TestPath())
                    backupFile.CopyPath(fileItem.FullName,true);
                else
                    fileItem.FullName.CopyPath(backupFile, true);

                // When we find english files, run a sub-routine to create other language files.
                if (fileItem.Name == "eng.lng" || fileItem.Name == "dialog_eng.lng")
                    LanguagePatches(fileItem);

                // There are multiple variations of the small font to create.
                HandleSmallFonts(fileItem);

                // Create the patched file.
                string xdelta3File = Path.Combine((Config.tempFolder + "\\patches").CreatePath(), fileItem.Name + ".xdelta");
                string patchedFile = Path.Combine((Config.tempFolder + "\\patchedFiles").CreatePath(), fileItem.Name);
                XDelta3.Patch(fileItem.FullName, fileItem.Name, xdelta3File, patchedFile, fileItem.FullName);
            }
            string message = patchFromBackup 
                ? "Patching the game from v1.0.0 backup files was successful. The game was updated to v"+ Config.version + "." 
                : "Patching Link's Awakening DX HD v1.0.0 was successful. The game was updated to v"+ Config.version + ".";
            Forms.okayDialog.Display("Patching Complete", 260, 40, 34, 16, 10, message);
        }

        private static bool ValidateStart()
        {
            string md5Hash = Config.zeldaEXE.CalculateHash("MD5");

            if (!Config.zeldaEXE.TestPath())
            {
                Forms.okayDialog.Display("Game Executable Not Found", 250, 40, 27, 10, 15, 
                    "Could not find \"Link's Awakening DX HD.exe\" to patch. Copy this patcher executable to the folder of the original release of v1.0.0 and run it from there.");
                return false;
            }
            if (md5Hash == Config.newHash)
            {
                Forms.okayDialog.Display("Already Patched", 260, 40, 30, 16, 10, 
                    "The game is already at v" + Config.version + " so no patching is needed. Close this patcher and launch the game!");
                return false;
            }
            if (!Config.backupPath.IsPathEmpty())
            {
                string backupExe = Path.Combine(Config.backupPath,"Link's Awakening DX HD.exe");
                if (backupExe.TestPath())
                {
                    md5Hash = backupExe.CalculateHash("MD5");
                    patchFromBackup = true;
                }
            }
            if (md5Hash != Config.oldHash && md5Hash != Config.newHash)
            {
                Forms.okayDialog.Display("Uknown Version", 260, 40, 26, 24, 10, 
                    "The version you are attempting to patch is unknown!");
                return false;
            }
            if (md5Hash == Config.oldHash)
                return true;

            Forms.okayDialog.Display("Unknown Error", 260, 40, 30, 16, 10, 
                "Something unexpected went wrong. Make sure the game is actually the original v1.0.0 release.");
            return false;
        }

        private static bool ValidateInput()
        {
            return Forms.yesNoDialog.Display("Patch to " + Config.version, 260, 20, 28, 24, true, 
                "Are you sure you wish to patch the game to v" + Config.version + "?");
        }

        public static void StartPatching()
        {
            if (!ValidateStart()) return;
            if (!ValidateInput()) return;

            Forms.mainDialog.ToggleDialog(false);
            Config.tempFolder.CreatePath(true);

            XDelta3.Create();
            PatchGameFiles();

            Config.tempFolder.RemovePath();
            Forms.mainDialog.ToggleDialog(true);
        }
    }
}
