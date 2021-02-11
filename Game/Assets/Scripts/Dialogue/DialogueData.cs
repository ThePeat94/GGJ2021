using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nidavellir.FoxIt.Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue", order = 0)]
    public class DialogueData : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> m_nodes = new List<DialogueNode>();

        public IEnumerable<DialogueNode> Nodes => this.m_nodes;
        public Dictionary<string, DialogueNode> IdToNode { get; private set; }
        public DialogueNode Root => this.m_nodes.Where(n => n.IsRoot).FirstOrDefault();
        
        private void OnValidate()
        {
            this.IdToNode = this.m_nodes.ToDictionary(n => n.Id, n => n);
        }

#if UNITY_EDITOR
        public void AddNewNode(DialogueNode parent)
        {
            var newNode = this.CreateNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            if(!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
                Undo.RecordObject(this, "New Dialogue Node");
            this.AddNode(newNode);
        }

        private DialogueNode CreateNode(DialogueNode parent)
        {
            var newNode = CreateInstance<DialogueNode>();
            newNode.Init();
            if (parent != null)
            {                
                var offset = parent.Rect.center;
                offset.x += parent.Rect.width / 2 + 50f;
                newNode.MoveRect(offset);
                parent.AddChild(newNode.Id);
                newNode.IsPlayerSpeaking = !parent.IsPlayerSpeaking;
            }

            return newNode;
        }

        private void AddNode(DialogueNode node)
        {
            this.m_nodes.Add(node);
            this.IdToNode = this.m_nodes.ToDictionary(n => n.Id, n => n);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Delete Dialogue Node");
            this.m_nodes.Remove(nodeToDelete);
            this.IdToNode = this.m_nodes.ToDictionary(n => n.Id, n => n);

            var parentNodes = this.m_nodes.Where(n => n.ChildrenIds.Contains(nodeToDelete.Id));
            foreach (var node in parentNodes)
            {
                node.RemoveChild(nodeToDelete.Id);
            }
            Undo.DestroyObjectImmediate(nodeToDelete);
        }
#endif
      
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (this.m_nodes.Count == 0)
            {
                var newNode = this.CreateNode(null);
                this.AddNode(newNode);   
            }

            if(!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                foreach(var node in this.m_nodes)
                {
                    if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(node)))
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}