using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using CycloneGames.Editor.VersionControl;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using System.Reflection;

namespace CycloneGames.Editor.Build
{
    [Serializable]
    public class VersionInfo
    {
        public string CommitHash { get; set; }
        public string CreatedDate { get; set; }
    }

    public class BuildScript
    {
        private const string DEBUG_FLAG = "<color=cyan>[Game Builder]</color>";
        private const string INVALID_FLAG = "INVALID";

        private const string CompanyName = "CycloneGames";
        private const string ApplicationName = "GASSample";
        private const string ApplicationVersion = "v0.1";
        private const string OutputBasePath = "Build";
        private const string BuildDataConfig = "Assets/GASSample/Editor/Build/BuildData.asset";
        private const string VersionInfoAssetPath = "Assets/Resources/VersionInfoData.asset";

        private static BuildData buildData;

        private static VersionControlType DefaultVersionControlType = VersionControlType.Git;
        private static IVersionControlProvider VersionControlProvider;
        private static void InitializeVersionControl(VersionControlType vcType)
        {
            VersionControlProvider = VersionControlFactory.CreateProvider(vcType);
        }

        [MenuItem("Build/Game(NoHotUpdate)/Print Debug Info", priority = 100)]
        public static void PrintDebugInfo()
        {
            var sceneList = GetBuildSceneList();
            if (sceneList == null || sceneList.Length == 0)
            {
                Debug.LogError(
                    $"{DEBUG_FLAG} Invalid scene list, please check the file <color=cyan>{BuildDataConfig}</color>");
                return;
            }

            foreach (var scene_name in sceneList)
            {
                Debug.Log($"{DEBUG_FLAG} Pre Build Scene: {scene_name}");
            }
        }

        [MenuItem("Build/Game(NoHotUpdate)/Build Android APK (IL2CPP)", priority = 400)]
        public static void PerformBuild_AndroidAPK()
        {
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            PerformBuild(
                BuildTarget.Android,
                NamedBuildTarget.Android,
                ScriptingImplementation.IL2CPP,
                $"{GetPlatformFolderName(BuildTarget.Android)}/{ApplicationName}.apk",
                bCleanBuild: true,
                bDeleteDebugFiles: true,
                bOutputIsFolderTarget: false);
        }

        [MenuItem("Build/Game(NoHotUpdate)/Build Windows (IL2CPP)", priority = 401)]
        public static void PerformBuild_Windows()
        {
            PerformBuild(
                BuildTarget.StandaloneWindows64,
                NamedBuildTarget.Standalone,
                ScriptingImplementation.IL2CPP,
                $"{GetPlatformFolderName(BuildTarget.StandaloneWindows64)}/{ApplicationName}.exe",
                bCleanBuild: true,
                bDeleteDebugFiles: true,
                bOutputIsFolderTarget: false);
        }

        [MenuItem("Build/Game(NoHotUpdate)/Build Mac (IL2CPP)", priority = 402)]
        public static void PerformBuild_Mac()
        {
            PerformBuild(
                BuildTarget.StandaloneOSX,
                NamedBuildTarget.Standalone,
                ScriptingImplementation.IL2CPP,
                $"{GetPlatformFolderName(BuildTarget.StandaloneOSX)}/{ApplicationName}.app",
                bCleanBuild: true,
                bDeleteDebugFiles: true,
                bOutputIsFolderTarget: false);
        }

        [MenuItem("Build/Game(NoHotUpdate)/Export Android Project (IL2CPP)", priority = 404)]
        public static void PerformBuild_AndroidProject()
        {
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            PerformBuild(
                BuildTarget.Android,
                NamedBuildTarget.Android,
                ScriptingImplementation.IL2CPP,
                $"{GetPlatformFolderName(BuildTarget.Android)}/{ApplicationName}",
                bCleanBuild: true,
                bDeleteDebugFiles: true,
                bOutputIsFolderTarget: true);
        }

        [MenuItem("Build/Game(NoHotUpdate)/Build WebGL", priority = 403)]
        public static void PerformBuild_WebGL()
        {
            PerformBuild(
                BuildTarget.WebGL,
                NamedBuildTarget.WebGL,
                ScriptingImplementation.IL2CPP,
                $"{GetPlatformFolderName(BuildTarget.WebGL)}/{ApplicationName}",
                bCleanBuild: true,
                bDeleteDebugFiles: true,
                bOutputIsFolderTarget: true);
        }

        private static string GetPlatformFolderName(BuildTarget TargetPlatform)
        {
            switch (TargetPlatform)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSX:
                    return "Mac";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.NoTarget:
                    return INVALID_FLAG;
            }

            return INVALID_FLAG;
        }

        private static RuntimePlatform GetRuntimePlatformFromBuildTarget(BuildTarget TargetPlatform)
        {
            switch (TargetPlatform)
            {
                case BuildTarget.Android:
                    return RuntimePlatform.Android;
                case BuildTarget.StandaloneWindows64:
                    return RuntimePlatform.WindowsPlayer;
                case BuildTarget.StandaloneOSX:
                    return RuntimePlatform.OSXPlayer;
                case BuildTarget.iOS:
                    return RuntimePlatform.IPhonePlayer;
                case BuildTarget.WebGL:
                    return RuntimePlatform.WebGLPlayer;
            }

            return RuntimePlatform.WindowsPlayer;
        }

        private static BuildData TryGetBuildData()
        {
            return buildData ??= AssetDatabase.LoadAssetAtPath<BuildData>($"{BuildDataConfig}");
        }

        private static string[] GetBuildSceneList()
        {
            if (!TryGetBuildData())
            {
                Debug.LogError(
                    $"{DEBUG_FLAG} Invalid Build Data Config, please check the file <color=cyan>{BuildDataConfig}</color>");
                return default;
            }

            return new[] { TryGetBuildData().GetLaunchScenePath() };
        }

        private static void DeletePlatformBuildFolder(BuildTarget TargetPlatform)
        {
            string platformBuildOutputPath = GetPlatformBuildOutputFolder(TargetPlatform);
            string platformOutputFullPath =
                platformBuildOutputPath != INVALID_FLAG ? Path.GetFullPath(platformBuildOutputPath) : INVALID_FLAG;

            if (Directory.Exists(platformOutputFullPath))
            {
                Debug.Log($"{DEBUG_FLAG} Clean old build {Path.GetFullPath(platformBuildOutputPath)}");
                Directory.Delete(platformOutputFullPath, true);
            }
        }

        private static void DeleteDebugFiles(BuildTarget TargetPlatform)
        {
            string platformBuildOutputPath = GetPlatformBuildOutputFolder(TargetPlatform);
            string platformOutputFullPath =
                platformBuildOutputPath != INVALID_FLAG ? Path.GetFullPath(platformBuildOutputPath) : INVALID_FLAG;

            string BackUpPath = Path.Combine(platformOutputFullPath, $"{ApplicationName}_BackUpThisFolder_ButDontShipItWithYourGame");
            if (Directory.Exists(BackUpPath))
            {
                Debug.Log($"{DEBUG_FLAG} Delete Backup Folder: {Path.GetFullPath(BackUpPath)}");
                Directory.Delete(BackUpPath, true);
            }

            string BurstDebugPath = Path.Combine(platformOutputFullPath, $"{ApplicationName}_BurstDebugInformation_DoNotShip");
            if (Directory.Exists(BurstDebugPath))
            {
                Debug.Log($"{DEBUG_FLAG} Delete Burst Debug Folder: {Path.GetFullPath(BurstDebugPath)}");
                Directory.Delete(BurstDebugPath, true);
            }
        }

        private static string GetOutputTarget(BuildTarget TargetPlatform, string TargetPath,
            bool bTargetIsFolder = true)
        {
            string platformOutFolder = GetPlatformBuildOutputFolder(TargetPlatform);
            string resultPath = Path.Combine(OutputBasePath, TargetPath);

            if (!Directory.Exists(Path.GetFullPath(platformOutFolder)))
            {
                Debug.Log($"{DEBUG_FLAG} result path: {resultPath}, platformFolder: {platformOutFolder}, platform fullPath:{Path.GetFullPath(platformOutFolder)}");
                Directory.CreateDirectory(platformOutFolder);
            }

#if UNITY_IOS
            if (!Directory.Exists($"{resultPath}/Unity-iPhone/Images.xcassets/LaunchImage.launchimage"))
            {
                Directory.CreateDirectory($"{resultPath}/Unity-iPhone/Images.xcassets/LaunchImage.launchimage");
            }
#endif
            return resultPath;
        }

        private static void PerformBuild(BuildTarget TargetPlatform, NamedBuildTarget BuildTargetName,
            ScriptingImplementation BackendScriptImpl, string OutputTarget, bool bCleanBuild = true, bool bDeleteDebugFiles = true,
            bool bOutputIsFolderTarget = true)
        {
            //  cache curernt scene
            var sceneSetup = EditorSceneManager.GetSceneManagerSetup();
            Debug.Log($"{DEBUG_FLAG} Saving current scene setup.");

            //  force save open scenes.
            EditorSceneManager.SaveOpenScenes();

            //  new template scene for build
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            try
            {
                var previousTarget = EditorUserBuildSettings.activeBuildTarget;

                if (bCleanBuild)
                {
                    DeletePlatformBuildFolder(TargetPlatform);
                }

                // If switching platforms, clear platform-specific caches to avoid stale artifacts
                if (previousTarget != TargetPlatform)
                {
                    Debug.Log($"{DEBUG_FLAG} Platform switch detected: {previousTarget} -> {TargetPlatform}. Clearing caches...");
                    TryClearPlatformSwitchCaches();
                }

                // Ensure Android export flag is only set for Android builds
                if (TargetPlatform != BuildTarget.Android)
                {
                    EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                }

                TryBuildalonSyncSolution();

                InitializeVersionControl(DefaultVersionControlType);
                string commitHash = VersionControlProvider?.GetCommitHash();
                VersionControlProvider?.UpdateVersionInfoAsset(VersionInfoAssetPath, commitHash);

                Debug.Log($"{DEBUG_FLAG} Start Build, Platform: {EditorUserBuildSettings.activeBuildTarget}");
                TryGetBuildData();
                if (EditorUserBuildSettings.activeBuildTarget != TargetPlatform)
                {
                    Debug.Log($"{DEBUG_FLAG} Switching active build target to {TargetPlatform}...");
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetName, TargetPlatform);
                }
                else
                {
                    Debug.Log($"{DEBUG_FLAG} Active build target already {TargetPlatform}, skipping switch.");
                }

                // After target switch, refresh assets and optionally sync solution/build scripts
                AssetDatabase.SaveAssets();
                TryBuildalonSyncSolution();
                TryCleanAddressablesPlayerContent();

                string originalVersion = PlayerSettings.bundleVersion;
                string commitShort = string.IsNullOrEmpty(commitHash)
                                            ? string.Empty
                                            : (commitHash.Length < 8 ? commitHash : commitHash.Substring(0, 8));
                string fullBuildVersion = string.IsNullOrEmpty(commitShort) ? $"{ApplicationVersion}.Unknown" : $"{ApplicationVersion}.{commitShort}";

                PlayerSettings.SetScriptingBackend(BuildTargetName, BackendScriptImpl);
                PlayerSettings.companyName = CompanyName;
                PlayerSettings.productName = ApplicationName;
                PlayerSettings.bundleVersion = fullBuildVersion;
                PlayerSettings.SetApplicationIdentifier(BuildTargetName, $"com.{CompanyName}.{ApplicationName}");

                BuildReport buildReport;

                {
                    var buildPlayerOptions = new BuildPlayerOptions();
                    buildPlayerOptions.scenes = GetBuildSceneList();
                    buildPlayerOptions.locationPathName = GetOutputTarget(TargetPlatform, OutputTarget, bOutputIsFolderTarget);
                    buildPlayerOptions.target = TargetPlatform;
                    buildPlayerOptions.options = BuildOptions.CleanBuildCache;
                    buildPlayerOptions.options |= BuildOptions.CompressWithLz4;
                    buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
                }

                var summary = buildReport.summary;
                if (summary.result == BuildResult.Succeeded)
                {
                    if (bDeleteDebugFiles)
                    {
                        string platformNameStr = GetPlatformFolderName(TargetPlatform);
                        if (platformNameStr == "Windows" || platformNameStr == "Mac") // TODO: May Linux
                        {
                            DeleteDebugFiles(TargetPlatform);
                        }
                    }

                    Debug.Log($"{DEBUG_FLAG} Build <color=#29ff50>SUCCESS</color>, size: {summary.totalSize} bytes, path: {summary.outputPath}\n");
                }

                if (summary.result == BuildResult.Failed) Debug.Log($"{DEBUG_FLAG} Build <color=red>FAILURE</color>");

                PlayerSettings.bundleVersion = originalVersion;
                VersionControlProvider?.ClearVersionInfoAsset(VersionInfoAssetPath);
            }
            finally
            {
                Debug.Log($"{DEBUG_FLAG} Restoring original scene setup.");
                //  In batch mode (CI/CD), the initial setup might be empty and invalid for restoration.
                if (sceneSetup != null && sceneSetup.Length > 0)
                {
                    EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);
                }
            }
        }

        private static string GetPlatformBuildOutputFolder(BuildTarget TargetPlatform)
        {
            return $"{OutputBasePath}/{GetPlatformFolderName(TargetPlatform)}";
        }

        public static void CopyAllFilesRecursively(string sourceFolderPath, string destinationFolderPath)
        {
            // Check if the source directory exists
            if (!Directory.Exists(sourceFolderPath))
            {
                throw new DirectoryNotFoundException($"Source directory does not exist: {sourceFolderPath}");
            }

            // Ensure the destination directory exists
            // Note: If the destination path is a network path, network permissions are required to create the directory
            try
            {
                if (!Directory.Exists(destinationFolderPath))
                {
                    Directory.CreateDirectory(destinationFolderPath);
                }
            }
            catch (Exception ex)
            {
                // Handle more specific exceptions, such as UnauthorizedAccessException for lack of permissions on network paths
                throw new Exception($"Error creating destination directory: {destinationFolderPath}. Exception: {ex.Message}");
            }

            // Get the files in the source directory and copy them to the destination directory
            foreach (string sourceFilePath in Directory.GetFiles(sourceFolderPath, "*", SearchOption.AllDirectories))
            {
                // Create a relative path that is the same for both source and destination
                string relativePath = sourceFilePath.Substring(sourceFolderPath.Length + 1);
                string destinationFilePath = Path.Combine(destinationFolderPath, relativePath);

                // Ensure the directory for the destination file exists (since it might be a subdirectory that doesn't exist yet)
                string destinationFileDirectory = Path.GetDirectoryName(destinationFilePath);
                if (!Directory.Exists(destinationFileDirectory))
                {
                    Directory.CreateDirectory(destinationFileDirectory);
                }

                // Copy the file and overwrite if it already exists
                // Note: If the destination path is a network path, network permissions are also required to copy files
                try
                {
                    File.Copy(sourceFilePath, destinationFilePath, true);
                }
                catch (Exception ex)
                {
                    // Handle more specific exceptions as well
                    throw new Exception($"Error copying file: {sourceFilePath} to {destinationFilePath}. Exception: {ex.Message}");
                }
            }
        }

        // Clears common Unity caches that often cause cross-platform build failures (Bee, IL2CPP, Burst, PlayerData)
        private static void TryClearPlatformSwitchCaches()
        {
            try
            {
                string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                string libraryPath = Path.Combine(projectRoot, "Library");
                string tempPath = Path.Combine(projectRoot, "Temp");

                string[] cacheDirs = new[]
                {
                    Path.Combine(libraryPath, "Bee"),
                    Path.Combine(libraryPath, "Il2cppBuildCache"),
                    Path.Combine(libraryPath, "BurstCache"),
                    Path.Combine(libraryPath, "PlayerDataCache"),
                    Path.Combine(libraryPath, "BuildPlayerDataCache"),
                    Path.Combine(tempPath, "gradleOut"),
                    Path.Combine(tempPath, "PlayBackEngine")
                };

                foreach (var dir in cacheDirs)
                {
                    if (Directory.Exists(dir))
                    {
                        Debug.Log($"{DEBUG_FLAG} Deleting cache folder: {dir}");
                        try { Directory.Delete(dir, true); }
                        catch (Exception ex) { Debug.LogWarning($"{DEBUG_FLAG} Failed to delete {dir}: {ex.Message}"); }
                    }
                }

                // Purge Unity build cache if available (reflection to avoid hard dependency)
                TryPurgeUnityBuildCache();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"{DEBUG_FLAG} TryClearPlatformSwitchCaches encountered a non-fatal error: {ex.Message}");
            }
        }

        private static void TryPurgeUnityBuildCache()
        {
            try
            {
                var editorAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (var asm in editorAssemblies)
                {
                    var buildCacheType = asm.GetType("UnityEditor.Build.BuildCache");
                    if (buildCacheType == null) continue;
                    MethodInfo purgeMethod = null;
                    foreach (var m in buildCacheType.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        if (m.Name != "PurgeCache") continue;
                        var parameters = m.GetParameters();
                        if (parameters.Length == 0 ||
                            (parameters.Length == 1 && parameters[0].ParameterType == typeof(bool)))
                        {
                            purgeMethod = m;
                            break;
                        }
                    }
                    if (purgeMethod != null)
                    {
                        if (purgeMethod.GetParameters().Length == 1)
                        {
                            purgeMethod.Invoke(null, new object[] { true });
                        }
                        else
                        {
                            purgeMethod.Invoke(null, null);
                        }
                        Debug.Log($"{DEBUG_FLAG} UnityEditor.Build.BuildCache purged.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"{DEBUG_FLAG} Unable to purge Unity build cache via reflection: {ex.Message}");
            }
        }

        // If Addressables package is present, clean player content to avoid stale catalog/bundles across platform switches
        private static void TryCleanAddressablesPlayerContent()
        {
            try
            {
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (var asm in assemblies)
                {
                    var addrType = asm.GetType("UnityEditor.AddressableAssets.Settings.AddressableAssetSettings");
                    if (addrType == null) continue;

                    // Try zero-parameter CleanPlayerContent first
                    MethodInfo cleanMethod = null;
                    foreach (var m in addrType.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        if (m.Name != "CleanPlayerContent") continue;
                        if (m.GetParameters().Length == 0)
                        {
                            cleanMethod = m;
                            break;
                        }
                    }
                    if (cleanMethod != null)
                    {
                        cleanMethod.Invoke(null, null);
                        Debug.Log($"{DEBUG_FLAG} Addressables CleanPlayerContent executed.");
                        return;
                    }

                    // Fallback: some versions require a settings instance; try to get default settings and invoke overload
                    var getSettingsMethod = addrType.GetMethod("Default", BindingFlags.Public | BindingFlags.Static) ??
                                            addrType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static)?.GetGetMethod();
                    var settingsInstance = getSettingsMethod != null ? getSettingsMethod.Invoke(null, null) : null;
                    if (settingsInstance != null)
                    {
                        MethodInfo cleanWithSettings = null;
                        foreach (var m in addrType.GetMethods(BindingFlags.Public | BindingFlags.Static))
                        {
                            if (m.Name != "CleanPlayerContent") continue;
                            var ps = m.GetParameters();
                            if (ps.Length == 1 && ps[0].ParameterType == addrType)
                            {
                                cleanWithSettings = m;
                                break;
                            }
                        }
                        if (cleanWithSettings != null)
                        {
                            cleanWithSettings.Invoke(null, new[] { settingsInstance });
                            Debug.Log($"{DEBUG_FLAG} Addressables CleanPlayerContent(settings) executed.");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"{DEBUG_FLAG} Addressables clean skipped: {ex.Message}");
            }
        }

        // If Buildalon is installed, sync solution to ensure project files are updated (safe, no exit)
        private static void TryBuildalonSyncSolution()
        {
            try
            {
                Debug.Log($"{DEBUG_FLAG} Probing Buildalon for SyncSolution...");
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                bool toolsFound = false;
                bool invoked = false;
                foreach (var asm in assemblies)
                {
                    var toolsType = asm.GetType("Buildalon.Editor.BuildPipeline.UnityPlayerBuildTools");
                    if (toolsType == null) continue;
                    toolsFound = true;
                    var syncMethod = toolsType.GetMethod("SyncSolution", BindingFlags.Public | BindingFlags.Static);
                    if (syncMethod != null)
                    {
                        syncMethod.Invoke(null, null);
                        Debug.Log($"{DEBUG_FLAG} Buildalon SyncSolution executed.");
                        invoked = true;
                    }
                    return;
                }
                if (!toolsFound)
                {
                    Debug.Log($"{DEBUG_FLAG} Buildalon not detected. Skipping SyncSolution.");
                }
                else if (!invoked)
                {
                    Debug.Log($"{DEBUG_FLAG} Buildalon detected but SyncSolution method not found.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"{DEBUG_FLAG} Buildalon SyncSolution skipped: {ex.Message}");
            }
        }
    }
}
