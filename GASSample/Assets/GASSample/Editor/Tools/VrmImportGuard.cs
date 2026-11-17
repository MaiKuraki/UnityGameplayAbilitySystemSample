using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GASSample.Editor
{
    /// <summary>
    /// Reimports VRM models whose import produced no materials.
    /// <para>
    /// A freshly cloned project imports .vrm files while the UniVRM packages are still being fetched
    /// and compiled. When the importer is not available yet, Unity falls back to the default importer,
    /// gets a single sub-asset and caches that result. The character prefabs reference materials as
    /// sub-assets of the .vrm, so every slot ends up null and the models render gray. Reimporting with
    /// the importer in place regenerates the sub-assets under the same file ids, which restores the
    /// existing references.
    /// </para>
    /// </summary>
    [InitializeOnLoad]
    internal static class VrmImportGuard
    {
        private const string PassKey = "GASSample.VrmImportGuard.Pass";
        private const int MaxPasses = 3;

        static VrmImportGuard()
        {
            EditorApplication.delayCall += Run;
        }

        private static void Run()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += Run;
                return;
            }

            List<string> broken = FindVrmWithoutMaterials();
            if (broken.Count == 0)
            {
                SessionState.EraseInt(PassKey);
                return;
            }

            int pass = SessionState.GetInt(PassKey, 0);
            if (pass >= MaxPasses)
            {
                SessionState.EraseInt(PassKey);
                Debug.LogWarning(
                    $"[VrmImportGuard] {broken.Count} VRM model(s) still import without materials. " +
                    $"Reimport them manually: {string.Join(", ", broken)}");
                return;
            }

            SessionState.SetInt(PassKey, pass + 1);
            foreach (string assetPath in broken)
            {
                Debug.Log($"[VrmImportGuard] '{assetPath}' imported without materials, reimporting.");
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }

            // Verify the reimport actually fixed it; a missing importer would reproduce the same result.
            EditorApplication.delayCall += Run;
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
