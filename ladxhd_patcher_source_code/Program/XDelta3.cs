using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LADXHD_Patcher
{
    internal class XDelta3
    {
        private static string Exe;
        private static string Args;

        private static Dictionary<string, object> resources = ResourceHelper.GetAllResources();

        public static void Initialize()
        {
            XDelta3.Exe = Path.Combine(Config.tempFolder, "xdelta3.exe");
        }

        private static string GetApplyArguments(string OldFile, string PatchFile, string NewFile)
        {
		    string args = string.Empty;
		    args = string.Concat(new string[]
		    {
			    args,
			    " -d -f -s \"",
			    OldFile,
			    "\" \"",
			    PatchFile,
			    "\" \"",
			    NewFile,
			    "\""
		    });
            return args;
        }

        private static void Start()
        {
            Process xDelta = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo 
            {
                WorkingDirectory = Config.baseFolder,
                FileName = XDelta3.Exe,
                Arguments = XDelta3.Args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Normal
            };
            xDelta.StartInfo = startInfo;
            xDelta.Start();
            xDelta.WaitForExit();
        }

        public static void Patch(string filePath, string resourceName, string deltaPatch, string patchedFile, string targetPath)
        {
            File.WriteAllBytes(deltaPatch, (byte[])resources[resourceName]);
            XDelta3.Args = XDelta3.GetApplyArguments(filePath, deltaPatch, patchedFile);
            XDelta3.Start();
            patchedFile.MovePath(targetPath, true);
        }

        public static void Create()
        {
            File.WriteAllBytes(XDelta3.Exe, (byte[])resources["xdelta3.exe"]);
        }
    }
}
