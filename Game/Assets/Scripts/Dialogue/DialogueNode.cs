using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Nidavellir.FoxIt.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool m_isPlayerSpeaking;
        [SerializeField] private string m_id;
        [SerializeField] private string m_text;
        [SerializeField] private List<string> m_childrenIds;
        [SerializeField] private Rect m_rect;
        [SerializeField] private List<string> m_texts;
        [SerializeField] private string m_nodeEnteredTrigger;
        [SerializeField] private string m_nodeExitedTrigger;
        [SerializeField] private bool m_isRoot;

        public string Id => this.m_id;

        public string Text
        {
            get => this.m_text;
#if UNITY_EDITOR
            set
            {
                if (this.m_text != value)
                {
                    Undo.RecordObject(this, "Update Node Text");
                    this.m_text = value;
                    EditorUtility.SetDirty(this);
                }
            }
#endif
        }

        public IReadOnlyList<string> ChildrenIds => this.m_childrenIds;
        public Rect Rect => this.m_rect;
        public bool IsPlayerSpeaking
        {
            get => this.m_isPlayerSpeaking;
#if UNITY_EDITOR
            set
            {
                Undo.RecordObject(this, "Update Node Text");
                this.m_isPlayerSpeaking = value;  
                EditorUtility.SetDirty(this);
            } 
#endif
        }

        public IReadOnlyList<string> Texts => this.m_texts;

        public string NodeEnteredTrigger => this.m_nodeEnteredTrigger;
        public string NodeExitedTrigger => this.m_nodeExitedTrigger;

        public bool IsRoot => this.m_isRoot;

        public void Init()
        {
            this.m_rect = new Rect(10, 10, 200, 200);
            this.m_id = System.Guid.NewGuid().ToString();
            this.name = this.m_id;
            this.m_childrenIds = new List<string>();
            this.m_texts = new List<string>();
        }
        
#if UNITY_EDITOR
        public void MoveRect(Vector2 offset)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            this.m_rect.position += offset;
            EditorUtility.SetDirty(this);
        }

        public void AddChild(string id)
        {
            Undo.RecordObject(this, "Link Dialogue");
            this.m_childrenIds.Add(id);
            EditorUtility.SetDirty(this);

        }

        public void RemoveChild(string id)
        {
            Undo.RecordObject(this, "Unlink Dialogue");
            this.m_childrenIds.Remove(id);
            EditorUtility.SetDirty(this);
        }

        public void AddText(string text)
        {
            Undo.RecordObject(this, "Add Text to Dialogue");
            this.m_texts.Add(text);
            EditorUtility.SetDirty(this);
        }
        
        public void RemoveText(int index)
        {
            Undo.RecordObject(this, "Remove Text from Dialogue");
            this.m_texts.RemoveAt(index);
            EditorUtility.SetDirty(this);
        }

        public void UpdateText(int index, string newText)
        {
            Undo.RecordObject(this, "Update Text from Dialogue");
            this.m_texts[index] = newText;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}