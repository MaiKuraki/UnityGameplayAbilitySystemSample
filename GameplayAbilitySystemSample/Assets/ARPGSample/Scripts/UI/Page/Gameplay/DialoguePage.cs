using CycloneGames.Service;
using CycloneGames.UIFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using Yarn.Unity.UnityLocalization;
using Zenject;

namespace ARPGSample.UI
{
    internal static class DialoguePathBuilder
    {
        public static string GetProviderPrefabPath(string dialogueTarget, string dialogue)
            => $"Assets/ARPGSample/Dialogue/{dialogueTarget}/LocalizationProvider_{dialogue}.prefab";
        public static string GetConfigPath(string dialogueTarget, string dialogue) 
            => $"Assets/ARPGSample/Dialogue/{dialogueTarget}/{dialogueTarget}_{dialogue}.yarnproject";
    }

    public class DialoguePage : UIPage
    {
        [Inject] private IAddressablesService addressablesService;
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private Button Btn_Skip;

        private const string dialogueStartKey = "Start";
        private GameObject dialogueConfigHandler;
        private System.Action currentDialogueFinishedEvent;

        protected override void Awake()
        {
            base.Awake();
            
            Btn_Skip?.onClick.AddListener(SkipDialogue);
        }

        public void StartDialogue(string dialogueTarget, string dialogue, System.Action onDialogueFinishedEvent = null)
        {
            StartDialogueAsync(dialogueTarget, dialogue, onDialogueFinishedEvent).Forget();
        }

        private async UniTask StartDialogueAsync(string dialogueTarget, string dialogue, System.Action onDialogueFinishedEvent = null)
        {
            try
            {
                var configAddress = DialoguePathBuilder.GetConfigPath(dialogueTarget, dialogue);
                var providerAddress = DialoguePathBuilder.GetProviderPrefabPath(dialogueTarget, dialogue);

                var config = await addressablesService.LoadAssetWithAutoReleaseAsync<YarnProject>(configAddress);
                var providerPrefab = await addressablesService.LoadAssetWithAutoReleaseAsync<GameObject>(providerAddress);

                SetupDialogueRunner(config, providerPrefab);

                currentDialogueFinishedEvent = onDialogueFinishedEvent;
                dialogueRunner.onDialogueComplete.AddListener(OnCurrentDialogueFinished);
                dialogueRunner.StartDialogue(dialogueStartKey);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error starting dialogue {dialogue}: {ex.Message}");
                // Handle exceptions, such as logging and user notifications
            }
        }

        private void SetupDialogueRunner(YarnProject config, GameObject providerPrefab)
        {
            dialogueRunner.SetProject(config);
            ResetDialogueProvider(providerPrefab);
        }

        private void ResetDialogueProvider(GameObject providerPrefab)
        {
            if (dialogueConfigHandler != null)
            {
                Destroy(dialogueConfigHandler);
            }

            dialogueConfigHandler = Instantiate(providerPrefab);
            dialogueRunner.lineProvider = dialogueConfigHandler.GetComponent<UnityLocalisedLineProvider>();
        }

        private void OnCurrentDialogueFinished()
        {
            currentDialogueFinishedEvent?.Invoke();
            ResetDialogue();
        }

        private void ResetDialogue()
        {
            dialogueRunner?.StopAllCoroutines();
            dialogueRunner?.onDialogueComplete.RemoveListener(OnCurrentDialogueFinished);
            dialogueRunner.lineProvider = null;

            if (dialogueConfigHandler != null)
            {
                Destroy(dialogueConfigHandler);
                dialogueConfigHandler = null;
            }

            currentDialogueFinishedEvent = null;
        }
        
        private void SkipDialogue()
        {
            dialogueRunner.Stop();
            OnCurrentDialogueFinished();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ResetDialogue();
        }
    }
}