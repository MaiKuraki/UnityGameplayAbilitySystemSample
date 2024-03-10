using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CycloneGames.Service
{
    public enum ScreenOrientation
    {
        Landscape = 0,
        Portrait = 1
    }
    public interface IGraphicsSettingService
    {
        void SetQualityLevel(int newQualityLevel);
        int CurrentQualityLevel { get; }
        List<string> QualityLevels { get; }
        void ChangeRenderResolution(int NewShortEdgeResolution, ScreenOrientation screenOrientation = ScreenOrientation.Landscape);
        void ChangeApplicationFrameRate(int TargetFramerate);
    }
    public class GraphicsSettingService : IGraphicsSettingService, IInitializable
    {
        private const string DEBUG_FLAG = "[GraphicsSetting]";
        private int currentQualityLevel;
        public int CurrentQualityLevel => currentQualityLevel;
        private List<string> qualitySettingsList = new List<string>();
        public List<string> QualityLevels => qualitySettingsList;
        private CancellationTokenSource Cancel_ChangeResolution;

        public void SetQualityLevel(int newQualityLevel)
        {
            // Debug.Log($"{DEBUG_FLAG} CurrentQualityLevel: {CurrentQualityLevel}, NewQualityLevel{newQualityLevel}");
            QualitySettings.SetQualityLevel(newQualityLevel, true);
        }

        public void Initialize()
        {
            qualitySettingsList = QualitySettings.names.ToList();
            currentQualityLevel = QualitySettings.GetQualityLevel();
        }
        public void ChangeRenderResolution(int NewShortEdgeResolution, ScreenOrientation screenOrientation = ScreenOrientation.Landscape)
        {
            if (Cancel_ChangeResolution != null)
            {
                if (Cancel_ChangeResolution.Token.CanBeCanceled)
                {
                    Cancel_ChangeResolution.Cancel();
                }
                Cancel_ChangeResolution.Dispose();
            }
            Cancel_ChangeResolution = new CancellationTokenSource();
            
            ChangeScreenResolutionAsync(Cancel_ChangeResolution.Token, NewShortEdgeResolution).Forget();
        }

        public void ChangeApplicationFrameRate(int TargetFramerate)
        {
            Application.targetFrameRate = TargetFramerate;
        }

        async UniTask ChangeScreenResolutionAsync(CancellationToken CancelToken, int NewShortEdgeResolution, ScreenOrientation screenOrientation = ScreenOrientation.Landscape)
        {
            float aspectRatio = Screen.width / (float)Screen.height;
            int newScreenHeight = 0;
            int newScreenWidth = 0;
            switch (screenOrientation)
            {
                case ScreenOrientation.Landscape:
                    newScreenHeight = NewShortEdgeResolution;
                    newScreenWidth = (int)(newScreenHeight * aspectRatio);
                    break;
                case ScreenOrientation.Portrait:
                    newScreenWidth = NewShortEdgeResolution;
                    newScreenHeight = (int)(newScreenWidth / aspectRatio);
                    break;
            }

            Screen.SetResolution(newScreenWidth, newScreenHeight, true);
            Debug.Log($"{DEBUG_FLAG} screenW: {Screen.width}, screenH: {Screen.height}, newScreenW: {newScreenWidth}, newScreenH: {newScreenHeight}, TotalQualityLevel: {QualityLevels.Count}, CurrentQualityLevel: {CurrentQualityLevel}");
            await UniTask.Delay(500, DelayType.Realtime, PlayerLoopTiming.Update, CancelToken);
            // Debug.Log($"{DEBUG_FLAG} newResW: {Screen.currentResolution.width}, newResH: {Screen.currentResolution.height}");
        }
    }
}