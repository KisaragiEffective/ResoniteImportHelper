#nullable enable
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace KisaragiMarine.ResoniteImportHelper.Bootstrap.Logic
{
    /// <summary>
    /// <see cref="UnityEditor.PackageManager.Client" /> を模倣するプロキシ。<br />
    /// </summary>
    [UsedImplicitly]
    public static partial class PackageManagerProxy
    {
        // TODO: https://github.com/KisaragiEffective/ResoniteImportHelper/issues/265
        private const string UnmanagedArchiveInstallSource =
            "https://github.com/vrm-c/UniVRM/releases/download/v" + SupportedUniGLTFVersion +
            "/VRM-" + SupportedUniGLTFVersion + "_" +
            BuildHash + ".unitypackage";

        private static HttpClient? _httpClient;

        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        public static void InstallUniGLTF()
        {
            if (HasSystemWideGitInstallation())
            {
                // Gitがある場合はGitを使う
                // UnityEditor.PackageManager.Clientは非同期の操作を提供するが、
                // その同期的な待機にスピンロックを必要とするため、低レベルの操作を行って迂回する。
                Debug.Log("Bootstrap: using Git.");
                var lowLevelPath = Application.dataPath + "/../Packages/manifest.json";
                var json = File.ReadAllText(lowLevelPath);
                // this implements dynamic field selector.
                dynamic? jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                if (jsonObj is null)
                {
                    throw new Exception("Unity's package manifest is corrupted");
                }

                var uniGltf = $"https://github.com/vrm-c/UniVRM.git?path=/Assets/UniGLTF#v{SupportedUniGLTFVersion}";
                jsonObj["dependencies"]["com.vrmc.gltf"] = uniGltf;
                if (jsonObj["dependencies"]["com.vrmc.univrm"] != null)
                {
                    var vrm0 = $"https://github.com/vrm-c/UniVRM.git?path=/Assets/VRM#v{SupportedUniGLTFVersion}";
                    jsonObj["dependencies"]["com.vrmc.univrm"] = vrm0;
                }

                if (jsonObj["dependencies"]["com.vrmc.vrm"] != null)
                {
                    var vrm1 = $"https://github.com/vrm-c/UniVRM.git?path=/Assets/VRM10#v{SupportedUniGLTFVersion}";
                    jsonObj["dependencies"]["com.vrmc.vrm"] = vrm1;
                }

                string outcome =
                    Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(lowLevelPath, outcome);
                // refresh
                Client.Resolve();
            }
            else
            {
                Debug.Log("Bootstrap: using UnityPackage.");

                _httpClient ??= new HttpClient();
                string path;
                {
                    var t = Task.Run(DownloadUnmanagedArchive);
                    t.Wait();
                    path = t.Result;
                }

                Debug.Log($"Downloaded UnityPackage is allocated on {path}");

                var hasVRM10Installation = AssetDatabase.IsValidFolder("Assets/VRM10");

                try
                {
                    Debug.Log("Import");
                    {
                        // XXX: mitigate confusing `ImportPackage` method
                        var m = typeof(AssetDatabase).GetMethod("ImportPackageImmediately",
                            BindingFlags.NonPublic | BindingFlags.Static);

                        m!.Invoke(null, new object[] { path });
                    }

                    AssetDatabase.StartAssetEditing();
                    // 間違ってAssetsのサブフォルダを削除する事故を防ぐために埋め込みパッケージ化
                    Debug.Log("Make UniGLTF embedded");
                    Debug.Log($"known subdirectories: {string.Join('\n', Directory.GetDirectories("Assets"))}");

                    Directory.Move("Assets/UniGLTF", "Packages/com.vrmc.gltf");
                    File.Delete("Assets/UniGLTF.meta");

                    if (hasVRM10Installation)
                    {
                        Directory.Move("Assets/VRM10", "Packages/com.vrmc.vrm");
                    }
                    else
                    {
                        Directory.Delete("Assets/VRM10", true);
                    }
                    File.Delete("Assets/VRM10.meta");
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                    AssetDatabase.Refresh();
                }

                try
                {
                    File.Delete(path);
                } catch (IOException) {}
            }
        }

        private static async Task<string> DownloadUnmanagedArchive()
        {
            var r = await _httpClient!.GetAsync(UnmanagedArchiveInstallSource);
            var content = r.Content;

            var temp = Path.GetTempFileName();
            {
                await using var f = File.OpenWrite(temp);
                await content.CopyToAsync(f);
            }

            return temp;
        }

        private static bool HasSystemWideGitInstallation()
        {
            var psi = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = "git",
                Arguments = "--version"
            };

            using var process = new Process();
            process.StartInfo = psi;

            try
            {
                process.Start();
            }
            catch (Win32Exception)
            {
                return false;
            }
            process.WaitForExit();

            return true;
        }
    }
}
