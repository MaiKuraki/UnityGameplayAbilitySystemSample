using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using BuildResult = UnityEditor.Build.Reporting.BuildResult;

namespace CycloneGames.Editor.Build
{
    public class BuildScriptNoHotUpdate
    {
        private const string DEBUG_FLAG = "<color=cyan>[Game Builder]</color>";
        private const string INVALID_FLAG = "INVALID";
        
        private const string OutputBasePath = "Build";
        private const string ApplicationName = "ARPGSample";
        private const string BuildDataConfig = "Assets/ARPGSample/ScriptableObject/BuildConfig/BuildData.asset";

        private static BuildData buildData;

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
                BuildTargetGroup.Android,
                ScriptingImplementation.IL2CPP,
                $"{GetPlatformFolderName(BuildTarget.Android)}/{ApplicationName}.apk",
                bCleanBuild: true,
                bOutputIsFolderTarget: false);
        }
        
        [MenuItem("Build/Game(NoHotUpdate)/Build Windows (IL2CPP)", priority = 401)]
        public static void PerformBuild_Windows()
        {
            PerformBuild(
                BuildTarget.StandaloneWindows64,
                BuildTargetGroup.Standalone,
                ScriptingImplementation.IL2CPP,
                $"{GetPlatformFolderName(BuildTarget.StandaloneWindows64)}/{ApplicationName}.exe",
                bCleanBuild: true,
                bOutputIsFolderTarget: false);
        }
        
        [MenuItem("Build/Game(NoHotUpdate)/Build Mac (IL2CPP)", priority = 402)]
        public static void PerformBuild_Mac()
        {
            PerformBuild(
                BuildTarget.StandaloneOSX,
                BuildTargetGroup.Standalone,
                ScriptingImplementation.IL2CPP,
                $"{GetPlatformFolderName(BuildTarget.StandaloneOSX)}/{ApplicationName}.app",
                bCleanBuild: true,
                bOutputIsFolderTarget: false);
        }
        
        [MenuItem("Build/Game(NoHotUpdate)/Export Android Project (IL2CPP)", priority = 403)]
        public static void PerformBuild_AndroidProject()
        {
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            PerformBuild(
                BuildTarget.Android,
                BuildTargetGroup.Android,
                ScriptingImplementation.IL2CPP,
                $"{GetPlatformFolderName(BuildTarget.Android)}/{ApplicationName}",
                bCleanBuild: true,
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

            return new[] { $"{TryGetBuildData().BuildSceneBasePath}/{TryGetBuildData().LaunchScene.name}.unity" };
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
        
        private static void PerformBuild(BuildTarget TargetPlatform, BuildTargetGroup TargetGroup,
            ScriptingImplementation BackendScriptImpl, string OutputTarget, bool bCleanBuild = true,
            bool bOutputIsFolderTarget = true)
        {
            if (bCleanBuild)
            {
                DeletePlatformBuildFolder(TargetPlatform);
            }
            
            Debug.Log($"{DEBUG_FLAG} Start Build, Platform: {EditorUserBuildSettings.activeBuildTarget}");
            EditorUserBuildSettings.SwitchActiveBuildTarget(TargetGroup, TargetPlatform);
            
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetBuildSceneList();
            buildPlayerOptions.locationPathName = GetOutputTarget(TargetPlatform, OutputTarget, bOutputIsFolderTarget);
            buildPlayerOptions.target = TargetPlatform;
            buildPlayerOptions.options = BuildOptions.CleanBuildCache;
            buildPlayerOptions.options |= BuildOptions.CompressWithLz4;
            PlayerSettings.SetScriptingBackend(TargetGroup, BackendScriptImpl);
            
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;
            
            if (summary.result == BuildResult.Succeeded)
                Debug.Log($"{DEBUG_FLAG} Build <color=#29ff50>SUCCESS</color>, size: {summary.totalSize} bytes, path: {summary.outputPath}");
            
            if (summary.result == BuildResult.Failed) Debug.Log($"{DEBUG_FLAG} Build <color=red>FAILURE</color>");
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
            // 注意：如果目标路径是局域网路径，则需要网络权限才能创建目录
            try
            {
                if (!Directory.Exists(destinationFolderPath))
                {
                    Directory.CreateDirectory(destinationFolderPath);
                }
            }
            catch (Exception ex)
            {
                // 可以处理更加具体的异常，例如对于网络路径无权限访问时的UnauthorizedAccessException
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
                // 注意：如果目标路径是局域网路径，也需要网络权限才能复制文件
                try
                {
                    File.Copy(sourceFilePath, destinationFilePath, true);
                }
                catch (Exception ex)
                {
                    // 同样可以处理更加具体的异常
                    throw new Exception($"Error copying file: {sourceFilePath} to {destinationFilePath}. Exception: {ex.Message}");
                }
            }
        }
    }
}