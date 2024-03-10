using System;
using ARPGSample.GameSubSystem;
using ARPGSample.UI;
using CycloneGames.UIFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class DialogueSystem : MonoBehaviour
    {
        [Inject] private IUIService uiService;
        [Inject] private IInputService inputService;

        private DialoguePage dialoguePage;
        private bool IsDialoguePageOpening = false;
        
        public void StartDialogue(string dialogueTarget, string dialogue, Action OnDialogueStartEvent = null,  Action OnDialogueFinishedEvent = null)
        {
            OnDialogueStartEvent?.Invoke();
            
            if (!IsDialoguePageOpening && !dialoguePage)
            {
                IsDialoguePageOpening = true;
                uiService.OpenUI(PageName.DialoguePage, OnDialoguePageCreated);
            }

            StartDialogueAsync(dialogueTarget, dialogue, OnDialogueFinishedEvent).Forget();
        }

        async UniTask StartDialogueAsync(string dialogueTarget, string dialogue, Action OnDialogueFinishedEvent = null)
        {
            await UniTask.WaitUntil(() => dialoguePage != null);
            dialoguePage.StartDialogue(dialogueTarget, dialogue, OnDialogueFinishedEvent);
        }
        
        void OnDialoguePageCreated(UIPage uiPage)
        {
            dialoguePage = uiPage as DialoguePage;
            IsDialoguePageOpening = false;
        }
    }
}