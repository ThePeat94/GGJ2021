using System;
using System.Collections.Generic;
using System.Linq;
using Nidavellir.FoxIt.Dialogue;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Nidavellir.FoxIt.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private const float CANVAS_SIZE = 4000f;
        private const float BACKGROUND_SIZE = 50f;
        private Texture2D m_backgroundTexture;
        private DialogueData m_currentSelected;
        [NonSerialized] private DialogueNode m_draggedNode;
        [NonSerialized] private Vector2 m_draggingOffset;
        [NonSerialized] private Dictionary<string, Vector2> m_idToScrollPosition;
        [NonSerialized] private bool m_isDraggingScrollView;

        [NonSerialized] private Vector2 m_lastMousePosition;
        [NonSerialized] private DialogueNode m_newNodeParent;
        [NonSerialized] private GUIStyle m_nodeStyle;
        [NonSerialized] private GUIStyle m_playerNodeStyle;
        private Vector2 m_scrollPosition;
        private Rect m_texCoords;
        [NonSerialized] private DialogueNode m_toDelete;
        [NonSerialized] private DialogueNode m_toLink;

        private void Awake()
        {
            this.m_backgroundTexture = Resources.Load("background") as Texture2D;
            this.m_texCoords = new Rect(0, 0, CANVAS_SIZE / BACKGROUND_SIZE, CANVAS_SIZE / BACKGROUND_SIZE);
        }

        private void OnEnable()
        {
            Selection.selectionChanged += this.SelectionChanged;
            this.InitNodeStyle();
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
                    if (!this.m_idToScrollPosition.ContainsKey(node.Id))
                        this.m_idToScrollPosition[node.Id] = Vector2.zero;
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

        [OnOpenAsset(1)]
        public static bool OpenAsset(int instanceId, int line)
        {
            var assetObject = EditorUtility.InstanceIDToObject(instanceId) as DialogueData;
            if (assetObject == null)
                return false;

            ShowWindow();
            return true;
        }

        private void InitNodeStyle()
        {
            this.m_nodeStyle = new GUIStyle();
            this.m_nodeStyle.normal.background = Texture2D.linearGrayTexture;
            this.m_nodeStyle.border = new RectOffset(5, 5, 5, 5);
            this.m_nodeStyle.padding = new RectOffset(5, 5, 5, 5);

            this.m_playerNodeStyle = new GUIStyle();
            this.m_playerNodeStyle.normal.background = Texture2D.whiteTexture;
            this.m_playerNodeStyle.border = new RectOffset(5, 5, 5, 5);
            this.m_playerNodeStyle.padding = new RectOffset(5, 5, 5, 5);
        }

        private void ProcessMouseEvents()
        {
            if (Event.current.type == EventType.MouseDown && this.m_draggedNode == null)
            {
                this.m_draggedNode = this.m_currentSelected.Nodes.LastOrDefault(n => n.Rect.Contains(Event.current.mousePosition + this.m_scrollPosition));
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
            else if (Event.current.type == EventType.MouseDrag && this.m_draggedNode != null)
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
            else if (Event.current.type == EventType.MouseUp && this.m_draggedNode != null)
            {
                this.m_draggedNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && this.m_isDraggingScrollView)
            {
                this.m_isDraggingScrollView = false;
            }
        }

        private void RenderConnections(DialogueNode node)
        {
            var start = node.Rect.center;
            start.x = node.Rect.xMax;
            foreach (var childNodeId in node.ChildrenIds)
            {
                var childNode = this.m_currentSelected.IdToNode[childNodeId];
                var end = childNode.Rect.center;
                end.x = childNode.Rect.xMin;
                var offset = end - start;
                offset.y = 0f;
                Handles.DrawBezier(start, end, start + offset, end - offset, Color.white, null, 5f);
            }
        }

        private void RenderNode(DialogueNode node)
        {
            var textFieldWidth = GUILayout.Width(node.Rect.width * 0.9f);
            var deleteButtonWidth = GUILayout.Width(node.Rect.width * 0.05f);
            GUILayout.BeginArea(node.Rect, node.IsPlayerSpeaking ? this.m_playerNodeStyle : this.m_nodeStyle);

            this.m_idToScrollPosition[node.Id] = EditorGUILayout.BeginScrollView(this.m_idToScrollPosition[node.Id], false, true);

            var toDeleteIndex = -1;
            for (var i = 0; i < node.Texts.Count; i++)
            {
                GUILayout.BeginHorizontal();
                node.UpdateText(i, GUILayout.TextArea(node.Texts[i], textFieldWidth));
                if (GUILayout.Button("X", deleteButtonWidth))
                    toDeleteIndex = i;
                GUILayout.EndHorizontal();
            }

            if (toDeleteIndex > -1)
                node.RemoveText(toDeleteIndex);

            if (GUILayout.Button("Add Text"))
                node.AddText("");

            EditorGUILayout.EndScrollView();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("+"))
                this.m_newNodeParent = node;

            if (this.m_toLink == null)
            {
                if (GUILayout.Button("Link"))
                    this.m_toLink = node;
            }
            else
            {
                if (node == this.m_toLink)
                {
                    if (GUILayout.Button("Cancel"))
                        this.m_toLink = null;
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
                this.m_toDelete = node;

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void SelectionChanged()
        {
            var activeDialogueData = Selection.activeObject as DialogueData;
            if (activeDialogueData != null)
            {
                this.m_currentSelected = activeDialogueData;
                this.m_idToScrollPosition = new Dictionary<string, Vector2>();
                this.Repaint();
            }
        }

        [MenuItem("Window/Dialogue Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window.Show();
        }
    }
}