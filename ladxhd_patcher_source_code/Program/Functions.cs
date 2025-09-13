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

/*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        FILE MAPPING CODE : NOT ALL FILES AND PATCHES ARE 1:1 FROM ORIGINAL GAME VERSION. NEW FILES NEED A "BASE" TO BE CREATED FROM USING A PATCH
       
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

        // SOME RESOURCES ARE USED TO CREATE MULTIPLE FILES OR PATCHES. EACH ARRAY BELOW HOLDS ALL VERSIONS OF A FILE THAT IS
        // BASED OFF OF ANOTHER FILE. THE "MASTER" FILE THAT CREATES THESE VERSIONS IS LINKED TO THEM IN THE DICTIONARY BELOW

        private static Dictionary<string, object> resources = ResourceHelper.GetAllResources();

        private static string[] langFiles  = new[] { "esp.lng", "fre.lng", "ita.lng", "por.lng", "rus.lng" };
        private static string[] langDialog = new[] { "dialog_esp.lng", "dialog_fre.lng", "dialog_ita.lng", "dialog_por.lng", "dialog_rus.lng" };
        private static string[] smallFonts = new[] { "smallFont_redux.xnb", "smallFont_vwf.xnb", "smallFont_vwf_redux.xnb" };
        private static string[] backGround = new[] { "menuBackgroundB.xnb", "menuBackgroundC.xnb" };
        private static string[] npcImages  = new[] { "npcs_redux.png" };
        private static string[] itemImages = new[] { "items_esp.png", "items_redux.png", "items_redux_esp.png" };
        private static string[] introImage = new[] { "intro_esp.png" };
        private static string[] miniMapImg = new[] { "minimap_esp.png" };
        private static string[] objectsImg = new[] { "objects_esp.png" };
        private static string[] photograph = new[] { "photos_esp.png", "photos_redux.png", "photos_redux_esp.png" };
        private static string[] uiImages   = new[] { "ui_esp.png"};

        // THE "KEY" IS THE MASTER FILE THAT CREATES OTHER FILES FROM IT. THE "VALUE" IS THE STRING ARRAY THAT HOLDS THOSE FILES

        private static readonly Dictionary<string, string[]> fileTargets = new Dictionary<string, string[]>
        {
            { "eng.lng",             langFiles },
            { "dialog_eng.lng",     langDialog },
            { "smallFont.xnb",      smallFonts },
            { "menuBackground.xnb", backGround },
            { "npcs.png",            npcImages },
            { "items.png",          itemImages },
            { "intro.png",          introImage },
            { "minimap.png",        miniMapImg },
            { "objects.png",        objectsImg },
            { "photos.png",         photograph },
            { "ui.png",               uiImages }
        };

/*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        PATCHING CODE : PATCH FILES USING XDELTA PATCHES FROM "Resources.resx" TO UPDATE TO THE LATEST VERSION.
       
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

        public static bool InBackup(FileItem fileItem)
        {
            // A quick check to know if we are currently in the backup directory.
            return (fileItem.DirectoryName.IndexOf("Data\\Backup", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static void HandleMultiFilePatches(FileItem fileItem)
        {
            // Use the file name to get the files that it creates.
            if (!fileTargets.TryGetValue(fileItem.Name, out var targets))
                return;

            // Loop through the target file names.
            foreach (string newFile in targets)
            {
                // Make sure a patch exists in the resources.resx file.
                if (!resources.ContainsKey(newFile))
                    continue;

                // If all has gone well, then patch the file to create a new file with a different name.
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

                // Do not try to patch the patcher or files directly in the backup folder.
                if (fileItem.Name == "xdelta3.exe" || InBackup(fileItem))
                    continue;

                // Get the backup path to test for existing backups and create new ones to it.
                string backupPath = Path.Combine(Config.backupPath, fileItem.Name);

                // Backup file if it has patch and a backup doesn't exist or restore from backup if one does exist.
                if (resources.ContainsKey(fileItem.Name))
                    if (!backupPath.TestPath())
                        fileItem.FullName.CopyPath(backupPath, true);
                    else
                        backupPath.CopyPath(fileItem.FullName, true);

                // If this file creates other files do so now.
                if (fileTargets.ContainsKey(fileItem.Name))
                    HandleMultiFilePatches(fileItem);

                // If this file is not patched directly then move on to the next.
                if (!resources.ContainsKey(fileItem.Name))
                    continue;

                // Patch the file.
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

/*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        SETUP / VALIDATION CODE : SET UP WHETHER PATCHING FROM v1.0.0 OR PATCHING FROM BACKUP FILES AND VERIFY IF PATCHING SHOULD TAKE PLACE.
       
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

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
