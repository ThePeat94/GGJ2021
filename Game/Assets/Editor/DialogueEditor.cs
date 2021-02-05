using System;
using System.Linq;
using Nidavellir.FoxIt.Dialogue;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

namespace Nidavellir.FoxIt.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private DialogueData m_currentSelected = null;
        [NonSerialized] private GUIStyle m_nodeStyle;
        [NonSerialized] private GUIStyle m_playerNodeStyle;

        [NonSerialized] private Vector2 m_lastMousePosition;
        [NonSerialized] private DialogueNode m_draggedNode;
        [NonSerialized] private DialogueNode m_newNodeParent;
        [NonSerialized] private DialogueNode m_toDelete;
        [NonSerialized] private DialogueNode m_toLink;
        [NonSerialized] private bool m_isDraggingScrollView;
        [NonSerialized] private Vector2 m_draggingOffset;
        
        private Vector2 m_scrollPosition;
        private Texture2D m_backgroundTexture;
        private Rect m_texCoords;
        
        private const float CANVAS_SIZE = 4000f;
        private const float BACKGROUND_SIZE = 50f;

        [MenuItem("Window/Dialogue Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window.Show();
        }

        private void Awake()
        {
            this.m_backgroundTexture = Resources.Load("background") as Texture2D;
            this.m_texCoords = new Rect(0, 0, CANVAS_SIZE / BACKGROUND_SIZE, CANVAS_SIZE / BACKGROUND_SIZE);
        }

        private void OnGUI()
        {
            if (this.m_currentSelected == null)
            {
                EditorGUILayout.LabelField("No Dialogue selected...");
            }
            else
            {
                this.m_scrollPosition = GUILayout.BeginScrollView(this.m_scrollPosition);
                var rect = GUILayoutUtility.GetRect(CANVAS_SIZE, CANVAS_SIZE);
                GUI.DrawTextureWithTexCoords(rect, this.m_backgroundTexture, this.m_texCoords);
                foreach (var node in this.m_currentSelected.Nodes)
                {
                    this.RenderNode(node);
                    this.RenderConnections(node);
                }
                GUILayout.EndScrollView();

                if (this.m_newNodeParent != null)
                {
                    this.m_currentSelected.AddNewNode(this.m_newNodeParent);
                    this.m_newNodeParent = null;
                }

                if (this.m_toDelete != null)
                {
                    this.m_currentSelected.DeleteNode(this.m_toDelete);
                    this.m_toDelete = null;
                }
            }
            
            this.ProcessMouseEvents();
        }

        private void ProcessMouseEvents()
        {
            if (Event.current.type == EventType.MouseDown && this.m_draggedNode == null)
            {
                this.m_draggedNode = this.m_currentSelected.Nodes.LastOrDefault(n => n.Position.Contains(Event.current.mousePosition + this.m_scrollPosition));
                this.m_lastMousePosition = Event.current.mousePosition;

                if (this.m_draggedNode == null)
                {
                    this.m_isDraggingScrollView = true;
                    this.m_draggingOffset = Event.current.mousePosition + this.m_scrollPosition;
                    Selection.activeObject = this.m_currentSelected;
                }
                else
                {
                    Selection.activeObject = this.m_draggedNode;
                }
            }
            else if (Event.current.type == EventType.MouseDrag  && this.m_draggedNode != null)
            {
                this.m_draggedNode.MoveRect(Event.current.mousePosition - this.m_lastMousePosition);
                this.Repaint();
                this.m_lastMousePosition = Event.current.mousePosition;
            }
            else if (Event.current.type == EventType.MouseDrag && this.m_isDraggingScrollView)
            {
                this.m_scrollPosition = this.m_draggingOffset - Event.current.mousePosition;
                this.Repaint();
            }
            else if(Event.current.type == EventType.MouseUp && this.m_draggedNode != null)
            {
                this.m_draggedNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && this.m_isDraggingScrollView)
            {
                this.m_isDraggingScrollView = false;
            }
        }

        private void ProcessKeyboardEvents()
        {
            
        }
        
        private void RenderNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.Position, node.IsPlayerSpeaking ? this.m_playerNodeStyle : this.m_nodeStyle);
            node.Text = EditorGUILayout.TextField(node.Text);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("+"))
            {
                this.m_newNodeParent = node;
            }
            
            if (this.m_toLink == null)
            {
                if (GUILayout.Button("Link"))
                {
                    this.m_toLink = node;
                }
            }
            else
            {
                if (node == this.m_toLink)
                {
                    if (GUILayout.Button("Cancel"))
                    {
                        this.m_toLink = null;
                    }
                }
                else if (this.m_toLink.ChildrenIds.Contains(node.Id))
                {
                    if (GUILayout.Button("Unlink"))
                    {
                        this.m_toLink.RemoveChild(node.Id);
                        this.m_toLink = null;
                    }
                }
                else
                {
                    if (GUILayout.Button("Child"))
                    {
                        this.m_toLink.AddChild(node.Id);
                        this.m_toLink = null;
                    }
                }
            }

            if (GUILayout.Button("x"))
            {
                this.m_toDelete = node;
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        }

        private void RenderConnections(DialogueNode node)
        {
            var start = node.Position.center;
            start.x = node.Position.xMax;
            foreach (var childNodeId in node.ChildrenIds)
            {
                var childNode = this.m_currentSelected.IdToNode[childNodeId];
                var end = childNode.Position.center;
                end.x = childNode.Position.xMin;
                var offset = end - start;
                offset.y = 0f;
                Handles.DrawBezier(start, end, start + offset, end - offset, Color.white, null, 5f);
            }
        }

        private void OnEnable()
        {
            Selection.selectionChanged += this.SelectionChanged;
            this.InitNodeStyle();
        }

        private void InitNodeStyle()
        {
            this.m_nodeStyle = new GUIStyle();
            this.m_nodeStyle.normal.background = Texture2D.linearGrayTexture;
            this.m_nodeStyle.border = new RectOffset(12, 12, 12, 12);
            
            this.m_playerNodeStyle = new GUIStyle();
            this.m_playerNodeStyle.normal.background = Texture2D.whiteTexture;
            this.m_playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void SelectionChanged()
        {
            var activeDialogueData = Selection.activeObject as DialogueData;
            if (activeDialogueData != null)
            {
                this.m_currentSelected = activeDialogueData;
                this.Repaint();
            }
        }

        [OnOpenAsset(1)]
        public static bool OpenAsset(int instanceId, int line)
        {
            var assetObject = EditorUtility.InstanceIDToObject(instanceId) as DialogueData;
            if (assetObject == null)
                return false;
            
            ShowWindow();
            return true;
        }
    }
}