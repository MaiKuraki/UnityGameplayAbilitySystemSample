using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GASSample.Editor
{
    /// <summary>
    /// Reimports VRM models whose import produced no materials.
    /// <para>
    /// A freshly cloned project imports .vrm files while the UniVRM packages and their shaders are
    /// still being imported. The importer then fails to build materials and caches that result, which
    /// leaves every material slot on the character prefab null and the models render gray.
    /// </para>
    /// <para>
    /// The check waits until the editor has been idle for a moment, because running it during the
    /// initial import only repeats the same failure.
    /// </para>
    /// </summary>
    [InitializeOnLoad]
    internal static class VrmImportGuard
    {
        private const double IdleSecondsBeforeCheck = 5.0;
        private const double RetryDelaySeconds = 2.0;
        private const int MaxPasses = 3;

        private static double busySince;
        private static double nextCheckTime;
        private static int pass;

        static VrmImportGuard()
        {
            busySince = EditorApplication.timeSinceStartup;
            EditorApplication.update += Tick;
        }

        private static void Tick()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                busySince = EditorApplication.timeSinceStartup;
                return;
            }

            double now = EditorApplication.timeSinceStartup;
            if (now - busySince < IdleSecondsBeforeCheck || now < nextCheckTime)
            {
                return;
            }

            EditorApplication.update -= Tick;
            Run();
        }

        private static void Run()
        {
            List<string> broken = FindVrmWithoutMaterials();
            if (broken.Count == 0)
            {
                return;
            }

            if (pass >= MaxPasses)
            {
                Debug.LogWarning(
                    $"[VrmImportGuard] {broken.Count} VRM model(s) still import without materials. " +
                    $"Reimport them manually: {string.Join(", ", broken)}");
                return;
            }

            pass++;
            foreach (string assetPath in broken)
            {
                Debug.Log($"[VrmImportGuard] '{assetPath}' imported without materials, reimporting.");
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }

            nextCheckTime = EditorApplication.timeSinceStartup + RetryDelaySeconds;
            EditorApplication.update += Tick;
        }

        private static List<string> FindVrmWithoutMaterials()
        {
            var broken = new List<string>();
            string assetsRoot = Application.dataPath;

            foreach (string file in Directory.GetFiles(assetsRoot, "*.vrm", SearchOption.AllDirectories))
            {
                string assetPath = "Assets" + file.Substring(assetsRoot.Length).Replace('\\', '/');
                if (!HasMaterialSubAsset(assetPath))
                {
                    broken.Add(assetPath);
                }
            }

            return broken;
        }

        private static bool HasMaterialSubAsset(string assetPath)
        {
            foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(assetPath))
            {
                if (asset is Material)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
