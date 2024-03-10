using Zenject;

namespace ARPGSample.Gameplay
{
    public interface IDialogueService
    {
        void StartDialogue(string dialogueTarget, string dialogue, System.Action OnDialogueStartEvent, System.Action OnDialogueFinishedEvent);
    }
    public class DialogueService : IInitializable, IDialogueService
    {
        private static readonly string DEBUG_FLAG = "[DialogueService]";
        
        [Inject] private DiContainer diContainer;

        private DialogueSystem dialogueSystem;
        
        public void Initialize()
        {
            dialogueSystem = diContainer.InstantiateComponentOnNewGameObject<DialogueSystem>("DialogueService");
        }


        public void StartDialogue(string dialogueTarget, string dialogue, System.Action OnDialogueStartEvent, System.Action OnDialogueFinishedEvent)
        {
            if (!dialogueSystem)
            {
                UnityEngine.Debug.LogError($"{DEBUG_FLAG} Invalid DialogueSystem");
                return;
            }
            dialogueSystem.StartDialogue(dialogueTarget, dialogue, OnDialogueStartEvent, OnDialogueFinishedEvent);
        }
    }
}