using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LR6.Dialogues
{
    public sealed class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private GameObject dialogueWindow = null!;
        [SerializeField] private TMP_Text messageText = null!;
        [SerializeField] private Transform answersRoot = null!;
        [SerializeField] private Button answerButtonPrefab = null!;
        [SerializeField] private PlayerStats playerStats = null!;

        private readonly List<Button> spawnedAnswers = new();
        private readonly DialogueXmlLoader xmlLoader = new();
        private readonly DialogueActionRunner actionRunner = new();

        private DialogueData? activeDialogue;
        private DialogueInteractable? activeInteractable;

        private void Awake()
        {
            if (dialogueWindow != null)
            {
                dialogueWindow.SetActive(false);
            }
        }

        public void SetAction(string name, System.Action<DialogueRuntimeContext> action)
        {
            actionRunner.SetAction(name, action);
        }

        public void StartDialogue(TextAsset xmlFile, DialogueInteractable interactable)
        {
            activeInteractable = interactable;
            activeDialogue = xmlLoader.LoadFromTextAsset(xmlFile);

            if (dialogueWindow != null)
            {
                dialogueWindow.SetActive(true);
            }

            if (activeDialogue.Messages.Count > 0)
            {
                ShowMessage(activeDialogue.Messages[0].MessageId);
            }
        }

        public void ShowMessage(long messageId)
        {
            if (activeDialogue == null)
            {
                return;
            }

            ClearAnswers();

            var text = activeDialogue.SelectMessage(messageId);
            if (messageText != null)
            {
                messageText.text = text;
            }

            var context = new DialogueRuntimeContext(playerStats, activeInteractable);
            foreach (var answer in activeDialogue.GetAnswers())
            {
                if (!actionRunner.IsAnswerAvailable(answer, context))
                {
                    continue;
                }

                var button = Instantiate(answerButtonPrefab, answersRoot);
                button.gameObject.SetActive(true);

                var label = button.GetComponentInChildren<TMP_Text>();
                if (label != null)
                {
                    label.text = answer.Text;
                }

                var messageIdCopy = messageId;
                var answerIdCopy = answer.AnswerId;
                button.onClick.AddListener(() => SelectAnswer(messageIdCopy, answerIdCopy));
                spawnedAnswers.Add(button);
            }
        }

        public void EndDialogue()
        {
            ClearAnswers();

            if (dialogueWindow != null)
            {
                dialogueWindow.SetActive(false);
            }

            activeDialogue = null;
            activeInteractable = null;
        }

        private void SelectAnswer(long messageId, long answerId)
        {
            if (activeDialogue == null)
            {
                return;
            }

            activeDialogue.SelectAnswer(messageId, answerId);
            var answer = activeDialogue.SelectedAnswer;
            if (answer == null)
            {
                return;
            }

            var context = new DialogueRuntimeContext(playerStats, activeInteractable);
            var shouldEndDialogue = actionRunner.Execute(answer, context);
            if (shouldEndDialogue)
            {
                EndDialogue();
                return;
            }

            if (answer.LinkedMessageId >= 0)
            {
                ShowMessage(answer.LinkedMessageId);
            }
            else
            {
                EndDialogue();
            }
        }

        private void ClearAnswers()
        {
            foreach (var button in spawnedAnswers)
            {
                if (button != null)
                {
                    Destroy(button.gameObject);
                }
            }

            spawnedAnswers.Clear();
        }
    }
}
