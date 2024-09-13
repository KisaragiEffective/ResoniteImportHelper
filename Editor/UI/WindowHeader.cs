using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace ResoniteImportHelper.UI
{
    internal sealed class WindowHeader: VisualElement
    {
        internal WindowHeader()
        {
            var title = new TextElement
            {
                text = "ResoniteImportHelper", style =
                {
                    alignSelf = Align.Center, fontSize = new StyleLength(new Length(16, LengthUnit.Pixel))
                }
            };
            
            this.Add(title);

            var thisVersion = new TextElement
            {
                text = "v. ???", style =
                {
                    alignSelf = Align.Center
                }
            };
            
            this.Add(thisVersion);
            
            var thisPackageOpt = PackageInfo
                .GetAllRegisteredPackages()
                .SingleOrDefault(p => p.name == "io.github.kisaragieffective.resonite-import-helper");

            if (thisPackageOpt != null)
            {
                AssignVersionInformation(in thisVersion, thisPackageOpt.version, thisPackageOpt.git?.hash);
            }
            else
            {
                // unmanaged install
                var t = new StackTrace(true).GetFrame(0);
                var file = t.GetFileName();

                if (file == null)
                {
                    Debug.Log("Failed to detect installed version");
                    return;
                }

                Debug.Log($"local installation: {file}");

                var declaredVersion = DetectCurrentUnmanagedInstallVersionFromPackageManifest(file);
                var rawRevision = DetectCurrentUnmanagedInstallRevision(new DirectoryInfo(file).Parent!.FullName);
                Debug.Log($"declaredVersion={declaredVersion ?? "null"}, rawRevision={rawRevision ?? "null"}");
                        
                AssignVersionInformation(in thisVersion, declaredVersion, rawRevision);
            }
        }

        [CanBeNull]
        private static string DetectCurrentUnmanagedInstallVersionFromPackageManifest(string file)
        {
            var di = new DirectoryInfo(file);
            while (di != null)
            {
                var packageManifestCandidate = Path.Combine(di.FullName, "package.json");
                if (File.Exists(packageManifestCandidate))
                {
                    var content = File.ReadAllText(packageManifestCandidate);
                    var declaredVersion = JsonUtility.FromJson<PartialPackageManifest>(content).version;

                    return declaredVersion;
                }

                di = di.Parent;
            }

            return null;
        }
        
        [CanBeNull]
        private static string DetectCurrentUnmanagedInstallRevision(string workingDirectory)
        {
            Debug.Log($"CWD: {workingDirectory}");
            try
            {
                var psi = new ProcessStartInfo("git")
                {
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    Arguments = "rev-parse HEAD"
                };

                using var p = Process.Start(psi);
                if (p == null)
                {
                    Debug.LogError("Process could not be started");
                    return null;
                }
                
                p.Start();
                p.WaitForExit();

                var x = p.StandardOutput.ReadToEnd().Trim('\n', '\r');
                return x;
            }
            catch (Win32Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private static void AssignVersionInformation(in TextElement e, string installedVersion,
            [CanBeNull] string gitRevision)
        {
            var rev = gitRevision[..Math.Min(gitRevision.Length - 1, 12)];
            
            var revision = rev != null 
                ? $"<a href=\"https://github.com/KisaragiEffective/ResoniteImportHelper/tree/{rev}\">{rev}</a>"
                : "???";
            
            e.text = $"v. {installedVersion} (commit {revision})";
            e.enableRichText = rev != null;
        }
    }
    
    [Serializable]
    internal struct PartialPackageManifest
    {
        [SerializeField]
        internal string version;
    }
}
