using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public struct Panel
    {
        public GameObject panelObject;
        public Action onClose;
        
        public Panel(GameObject panelObject, Action onClose)
        {
            this.panelObject = panelObject;
            this.onClose = onClose;
        }
    }
    
    public class PanelsManager : MonoBehaviour
    {
        private Stack<Panel> panelsStack = new();

        public float lastTimeClosed = float.MinValue;

        public int ActivePanelsCount => panelsStack.Count;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && ActivePanelsCount > 0)
            {
                ClosePanel(panelsStack.Peek().panelObject);
            }
        }
        
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
        
        public void OpenPanel(GameObject panelGameObject, Action onClose)
        {
            OpenPanel(new Panel(panelGameObject, onClose));
        }

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
        
        private void OpenPanel(Panel panel)
        {
            panel.panelObject.SetActive(true);
            panelsStack.Push(panel);
        }
    }
}
