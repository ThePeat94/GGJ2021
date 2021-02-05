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
        [SerializeField] private Rect m_position;

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

        public List<string> ChildrenIds => this.m_childrenIds;

        public Rect Position => this.m_position;

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

        public void Init()
        {
            this.m_position = new Rect(10, 10, 200, 200);
            this.m_id = System.Guid.NewGuid().ToString();
            this.name = this.m_id;
            this.m_childrenIds = new List<string>();
        }
        
#if UNITY_EDITOR
        public void MoveRect(Vector2 offset)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            this.m_position.position += offset;
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
#endif
    }
}