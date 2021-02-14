using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Nidavellir.FoxIt.Dialogue;
using Nidavellir.FoxIt.EventArgs;
using Nidavellir.FoxIt.InputController;
using Nidavellir.FoxIt.Interfaces;
using UnityEngine;

namespace Nidavellir.FoxIt.UI
{
    public class DialogueManager : MonoBehaviour
    {
        private static readonly WaitForSeconds s_waitForSeconds = new WaitForSeconds(0.2f);

        [SerializeField] private DialogueUI m_dialogueUI;
        [SerializeField] private DialogueInputProcessor m_dialogueInputProcessor;
        [SerializeField] private BoxCollider m_dialogueCollider;
        [SerializeField] private int m_rayCount;
        [SerializeField] private CinemachineFreeLook m_freeLookCam;

        private float m_anglePerRay;

        private Transform m_camera;

        private DialogueData m_currentDialogueData;
        private DialogueNode m_currentDialogueNode;

        private IReadOnlyList<string> m_currentNodeTexts;
        private ITalkable m_currentTalkable;
        private int m_currentTextIndex;
        private Coroutine m_detectionCoroutine;
        private EventHandler m_dialogueEnded;

        private EventHandler m_dialogueStarted;

        private bool m_isConversating;

        private List<Ray> m_raysToDetect;

        public event EventHandler DialogueEnded
        {
            add => this.m_dialogueEnded += value;
            remove => this.m_dialogueEnded -= value;
        }

        public event EventHandler DialogueStarted
        {
            add => this.m_dialogueStarted += value;
            remove => this.m_dialogueStarted -= value;
        }

        private void Awake()
        {
            this.m_anglePerRay = 180f / this.m_rayCount;
            this.m_dialogueUI.PlayerMadeChoice += this.OnPlayerMadeChoice;
        }

        private void Start()
        {
            this.m_detectionCoroutine = this.StartCoroutine(this.DetectTalkables());
            this.m_camera = Camera.main.transform;
        }

        private void Update()
        {
            if (!this.m_dialogueInputProcessor.ContinueDialogueTriggered)
                return;

            if (this.m_currentDialogueNode != null && this.m_currentDialogueNode.IsPlayerSpeaking)
                return;

            if (this.m_currentTalkable != null && !this.m_isConversating)
                this.StartDialogue(this.m_currentTalkable.GetDiaglogueData(), this.m_currentTalkable);
            else if (this.m_isConversating)
                this.ShowNextNpcText();
        }

        public void EndDialogue()
        {
            this.m_currentTalkable = null;
            this.m_dialogueEnded?.Invoke(this, System.EventArgs.Empty);
            this.m_isConversating = false;
            this.m_dialogueUI.Close();

            this.m_freeLookCam.enabled = true;
            this.m_camera.SetParent(null);
        }

        public void ShowNextNpcText()
        {
            if (this.m_dialogueUI.IsCurrentlyFadingTextIn())
            {
                this.m_dialogueUI.ShowTextImmediatly(this.m_currentNodeTexts[this.m_currentTextIndex], this.m_currentTalkable);
            }
            else if (this.m_currentTextIndex < this.m_currentNodeTexts.Count - 1)
            {
                this.m_currentTextIndex++;
                this.m_dialogueUI.ShowDialogueText(this.m_currentNodeTexts[this.m_currentTextIndex], this.m_currentTalkable);

                if (this.m_currentTextIndex == this.m_currentNodeTexts.Count - 1)
                    this.m_currentTalkable.TriggerAction(this.m_currentDialogueNode.NodeExitedTrigger);
            }
            else if (this.m_currentDialogueNode.ChildrenIds.Count == 1)
            {
                var followingNode = this.m_currentDialogueData.IdToNode[this.m_currentDialogueNode.ChildrenIds[0]];
                if (followingNode.IsPlayerSpeaking)
                    this.m_dialogueUI.ShowPlayerChoices(new[] {followingNode});
                else
                    this.ShowNewNode(followingNode);
            }
            else if (this.m_currentDialogueNode.ChildrenIds.Count > 0)
            {
                var childrenNodes = new List<DialogueNode>();
                foreach (var childrenId in this.m_currentDialogueNode.ChildrenIds)
                    childrenNodes.Add(this.m_currentDialogueData.IdToNode[childrenId]);

                if (childrenNodes.All(n => n.IsPlayerSpeaking))
                    this.m_dialogueUI.ShowPlayerChoices(childrenNodes);
            }
            else
            {
                this.EndDialogue();
            }
        }

        public void StartDialogue(DialogueData data, ITalkable current)
        {
            this.m_dialogueStarted?.Invoke(this, System.EventArgs.Empty);
            this.m_isConversating = true;
            this.m_currentTalkable = current;
            this.m_currentDialogueData = data;

            this.m_freeLookCam.enabled = false;
            this.m_camera.SetParent(current.Viewpoint);
            this.m_camera.localRotation = Quaternion.identity;
            this.m_camera.localPosition = Vector3.zero;

            this.ShowNewNode(data.Root);
        }

        private IEnumerator DetectTalkables()
        {
            while (true)
            {
                var hitObjects = Physics.OverlapBox(this.m_dialogueCollider.transform.position, this.m_dialogueCollider.size / 2f, this.transform.rotation, LayerMask.NameToLayer("Talkable"));
                var hitTalkables = hitObjects.Where(c => c.GetComponent<ITalkable>() != null).ToList();
                if (hitTalkables.Count == 0 && this.m_currentTalkable != null)
                {
                    this.m_currentTalkable?.HideUI();
                    this.m_currentTalkable = null;
                }

                if (hitTalkables.Count == 1 && hitTalkables[0].GetComponent<ITalkable>() != this.m_currentTalkable)
                {
                    Debug.Log("I Hit a new Talker!");
                    this.m_currentTalkable?.HideUI();
                    this.m_currentTalkable = hitTalkables[0].GetComponent<ITalkable>();
                    this.m_currentTalkable.ShowUI();
                }
                else if (hitTalkables.Count > 1)
                {
                    Debug.Log($"I hit {hitTalkables.Count} potential talkers.");
                    Collider closestCollider = null;
                    var closestDistance = 500f;
                    foreach (var hitTalkable in hitTalkables)
                    {
                        var distance = (hitTalkable.transform.position - this.transform.position).sqrMagnitude;
                        if (distance <= closestDistance)
                        {
                            closestDistance = distance;
                            closestCollider = hitTalkable;
                        }
                    }

                    this.m_currentTalkable?.HideUI();
                    this.m_currentTalkable = closestCollider.GetComponent<ITalkable>();
                    this.m_currentTalkable.ShowUI();
                }

                yield return s_waitForSeconds;
            }
        }

        private void OnPlayerMadeChoice(object sender, PlayerMadeChoiceEvent e)
        {
            this.m_dialogueUI.HidePlayerChoices();
            var node = this.m_currentDialogueData.IdToNode[e.ChoiceId];

            if (node.ChildrenIds.Count == 0)
            {
                this.EndDialogue();
                return;
            }

            var childrenId = node.ChildrenIds[0];
            this.ShowNewNode(this.m_currentDialogueData.IdToNode[childrenId]);
        }

        private void ShowNewNode(DialogueNode nodeToShow)
        {
            this.m_currentDialogueNode = nodeToShow;
            this.m_currentNodeTexts = this.m_currentDialogueNode.Texts;
            this.m_currentTextIndex = 0;
            this.m_dialogueUI.ShowDialogueText(this.m_currentNodeTexts[this.m_currentTextIndex], this.m_currentTalkable);
            this.m_currentTalkable.TriggerAction(this.m_currentDialogueNode.NodeEnteredTrigger);
        }
    }
}