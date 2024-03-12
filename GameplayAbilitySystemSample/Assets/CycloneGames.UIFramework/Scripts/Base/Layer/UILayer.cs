using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CycloneGames.UIFramework
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UILayer : MonoBehaviour
    {
        private const string DEBUG_FLAG = "[UILayer]";
        
        [SerializeField] private string layerName;

        private Canvas uiCanvas;
        public Canvas UICanvas => uiCanvas;
        private GraphicRaycaster graphicRaycaster;
        private GraphicRaycaster PageGraphicRaycaster => graphicRaycaster;
        public string LayerName => layerName;

        private List<UIPage> uiPagesList = new List<UIPage>();
        private bool bFinishedLayerInit = false;

        protected void Awake()
        {
            uiCanvas = GetComponent<Canvas>();
            graphicRaycaster = GetComponent<GraphicRaycaster>();
            
            PageGraphicRaycaster.blockingMask = LayerMask.GetMask("UI");

            InitLayer();
        }

        private void InitLayer()
        {
            // If there are no children, the initialization is considered complete.
            if (transform.childCount == 0)
            {
                bFinishedLayerInit = true;
                Debug.Log($"{DEBUG_FLAG} Finished init Layer: {LayerName}");
                return;
            }
            
            // Ensure the page's Name matches its associated prefab name,
            // and that the page's Name is defined within the PageName class.
            uiPagesList = GetComponentsInChildren<UIPage>().ToList();
            foreach (UIPage page in uiPagesList)
            {
                page.SetPageName(page.gameObject.name);
                
            }
            
            SortPagesByPriority();
            bFinishedLayerInit = true;
            Debug.Log($"{DEBUG_FLAG} Finished init Layer: {LayerName}");
        }
        public UIPage GetUIPage(string InPageName)
        {
            // Iterate through the list of pages to find the one with the matching name.
            foreach (UIPage page in uiPagesList)
            {
                if (page.PageName.Equals(InPageName, System.StringComparison.OrdinalIgnoreCase))
                {
                    // If a page with the matching name is found, return it.
                    return page;
                }
            }

            // If no matching page is found, return null.
            return null;
        }
        public bool HasPage(string InPageName)
        {
            return uiPagesList.Any(page => page.PageName.Equals(InPageName, System.StringComparison.OrdinalIgnoreCase));
        }
        public void AddPage(UIPage newPage)
        {
            // NOTE: Ensure the uiPageList is sorted before adding.
            if (!bFinishedLayerInit)
            {
                Debug.LogError($"{DEBUG_FLAG} layer not init, current layer: {LayerName}");
                return;
            }
            
            // Check for the existence of a page with the same name before adding.
            if (uiPagesList.Any(page => page.PageName == newPage.PageName))
            {
                Debug.LogError($"{DEBUG_FLAG} Page already exists: {newPage.PageName}");
                return;
            }
            
            newPage.gameObject.name = newPage.PageName;
            Transform pageTransform = newPage.transform;
            pageTransform.SetParent(transform, false);
            
            // If the list is empty, simply add the new page.
            if (uiPagesList.Count == 0)
            {
                uiPagesList.Add(newPage);
                return;
            }
            
            // Reverse iterate through the list to find the last index with Priority equal to the new page's Priority.
            int insertIndex = uiPagesList.Count; // Initialize as the end of the list.

            for (int i = uiPagesList.Count - 1; i >= 0; i--) 
            {
                if (uiPagesList[i].Priority > newPage.Priority) 
                {
                    // Found a page with a greater Priority, insert the new page before it.
                    insertIndex = i;
                } 
                else if (uiPagesList[i].Priority == newPage.Priority) 
                {
                    // Found a page with the same Priority, insert after this page.
                    insertIndex = i + 1;
                    break; // List is sorted so we can break the loop.
                }
            }

            // Insert the new page at the calculated index.
            uiPagesList.Insert(insertIndex, newPage);

            // Only need to update the sibling index for the new page and any after it.
            for (int i = insertIndex; i < uiPagesList.Count; i++) 
            {
                uiPagesList[i].transform.SetSiblingIndex(i);
            }
        }

        public void RemovePage(string InPageName)
        {
            // NOTE: Ensure the uiPageList is initialized.
            if (!bFinishedLayerInit)
            {
                Debug.LogError($"{DEBUG_FLAG} layer not init, current layer: {LayerName}");
                return;
            }
            
            UIPage page = TryGetPageByPageName(InPageName);
            if (!page)
            {
                Debug.LogError($"{DEBUG_FLAG} Remove Page Failure, layer:{LayerName}, PageName: {InPageName}");
                return;
            }
            
            uiPagesList.Remove(page);
            page.ClosePage();
        }

        private UIPage TryGetPageByPageName(string InPageName)
        {
            // Make sure the PageName in uiPageList is Unique.
            foreach (UIPage page in uiPagesList)
            {
                if (page.PageName == InPageName)
                {
                    return page;
                }
            }

            return null;
        }
        
        private void SortPagesByPriority()
        {
            // If there's only one or no page, no need to sort.
            if(uiPagesList.Count <= 1) return;
            
            // The following section can be uncommented for debugging purposes to print original PageList info.
            /*
            for (int i = 0; i < uiPagesList.Count; i++)
            {
                Debug.Log($"{DEBUG_FLAG} pageName: {uiPagesList[i].PageName}, Priority:{uiPagesList[i].Priority}, idx: {i}");
            }
            */
            
            // for (int i = 1; i < uiPagesList.Count; i++)
            // {
            //     UIPage current = uiPagesList[i];
            //     int j = i - 1;
            //
            //     // 更改排序条件，确保Priority较大的在后面
            //     while (j >= 0 && uiPagesList[j].Priority > current.Priority)
            //     {
            //         uiPagesList[j + 1] = uiPagesList[j];
            //         j--;
            //     }
            //
            //     uiPagesList[j + 1] = current;
            // }
            // Sort pages by their Priority using LINQ, ensuring higher Priority pages are later in the list.
            uiPagesList = uiPagesList.OrderBy(page => page.Priority).ToList();

            // The following section can be uncommented for debugging purposes to print sorted PageList info.
            /*
            for (int i = 0; i < uiPagesList.Count; i++)
            {
                Debug.Log($"{DEBUG_FLAG} sorted pageName: {uiPagesList[i].PageName}, Priority:{uiPagesList[i].Priority}, idx: {i}");
            }
            */

            // Update the sibling index according to the new order.
            for (int i = 0; i < uiPagesList.Count; i++)
            {
                uiPagesList[i].transform.SetSiblingIndex(i);
            }
        }
    }
}
