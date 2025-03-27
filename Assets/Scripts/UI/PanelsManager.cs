using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Represents a UI panel with its associated game object and close action
    /// </summary>
    public struct Panel
    {
        /// <summary>
        /// The GameObject representing the panel in the scene
        /// </summary>
        public GameObject panelObject;
        
        /// <summary>
        /// Action to execute when the panel is closed
        /// </summary>
        public Action onClose;
        
        /// <summary>
        /// Creates a new Panel with the specified game object and close action
        /// </summary>
        /// <param name="panelObject">The GameObject representing the panel</param>
        /// <param name="onClose">Action to execute when the panel is closed</param>
        public Panel(GameObject panelObject, Action onClose)
        {
            this.panelObject = panelObject;
            this.onClose = onClose;
        }
    }
    
    /// <summary>
    /// Manages UI panels with a stack-based approach
    /// Handles showing and hiding panels while maintaining proper navigation order
    /// </summary>
    public class PanelsManager : MonoBehaviour
    {
        /// <summary>
        /// Stack of currently active panels, with the top being the most recently opened
        /// </summary>
        private Stack<Panel> panelsStack = new();

        /// <summary>
        /// Timestamp of when the last panel was closed
        /// Used to prevent immediate re-opening of panels
        /// </summary>
        public float lastTimeClosed = float.MinValue;

        /// <summary>
        /// Number of currently active panels
        /// </summary>
        public int ActivePanelsCount => panelsStack.Count;

        /// <summary>
        /// Handles input updates, such as closing panels with the Escape key
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && ActivePanelsCount > 0)
            {
                ClosePanel(panelsStack.Peek().panelObject);
            }
        }
        
        /// <summary>
        /// Opens a panel with a default close action
        /// </summary>
        /// <param name="panelGameObject">The panel to open</param>
        public void OpenPanel(GameObject panelGameObject)
        {
            OpenPanel(
                new Panel(
                    panelGameObject,
                    () =>
                        {
                            panelGameObject.gameObject.SetActive(false);
                        }
                    )
                );
        }
        
        /// <summary>
        /// Opens a panel with a custom close action
        /// </summary>
        /// <param name="panelGameObject">The panel to open</param>
        /// <param name="onClose">Custom action to execute when the panel is closed</param>
        public void OpenPanel(GameObject panelGameObject, Action onClose)
        {
            OpenPanel(new Panel(panelGameObject, onClose));
        }

        /// <summary>
        /// Closes the specified panel and any panels opened after it
        /// </summary>
        /// <param name="panelGameObject">The panel to close</param>
        public void ClosePanel(GameObject panelGameObject)
        {
            while (panelsStack.TryPop(out Panel currentPanel))
            {
                lastTimeClosed = Time.time;
                if (currentPanel.panelObject.activeInHierarchy)
                {
                    currentPanel.onClose?.Invoke();
                }

                if (currentPanel.panelObject == panelGameObject)
                {
                    return;
                }
            }
        }
        
        /// <summary>
        /// Internal helper to open a panel and add it to the stack
        /// </summary>
        /// <param name="panel">The panel to open</param>
        private void OpenPanel(Panel panel)
        {
            panel.panelObject.SetActive(true);
            panelsStack.Push(panel);
        }
    }
}
