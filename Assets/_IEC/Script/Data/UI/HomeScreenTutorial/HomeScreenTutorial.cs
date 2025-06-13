using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class HomeScreenTutorial : _BaseBehaviour, ICanvasRaycastFilter
    {
        Dictionary<string, EventYieldWait> m_DictYieldWait = new Dictionary<string, EventYieldWait>();
        public Image tutorialScreenImage;
        public Material maskMaterial;
        public Canvas canvas;

        private Coroutine characterShopTutorialCoroutine;

        private RectTransform holeTarget;
        
        public void HighlightTarget(RectTransform target, Vector2 padding = default)
        {
            Camera cam = canvas.worldCamera;
            if (cam == null) return;

            holeTarget = target;

            Vector3[] corners = new Vector3[4];
            target.GetWorldCorners(corners);

            // Convert world corners to viewport space (0–1)
            Vector2 min = cam.WorldToViewportPoint(corners[0]);
            Vector2 max = cam.WorldToViewportPoint(corners[2]);

            // Make sure min < max
            Vector4 holeRect = new Vector4(
                Mathf.Min(min.x, max.x),
                Mathf.Min(min.y, max.y),
                Mathf.Max(min.x, max.x),
                Mathf.Max(min.y, max.y)
            );
            
            holeRect = new Vector4(
                holeRect.x - padding.x,
                holeRect.y - padding.y,
                holeRect.z + padding.x,
                holeRect.w + padding.y
            );

            maskMaterial.SetVector("_HoleRect", holeRect);
        }

        #region Character Shop
        
        public void DoTutorialCharacterShop()
        {
            if (characterShopTutorialCoroutine != null)
            {
                StopCoroutine(characterShopTutorialCoroutine);
            }

            characterShopTutorialCoroutine = StartCoroutine(IETutorialCharacterShop());
        }

        IEnumerator IETutorialCharacterShop()
        {
            Locator.Service.GameResourceDisplay.HideDisplayUnderBG();
            tutorialScreenImage.gameObject.SetActive(true);
            HighlightTarget(Locator.Main.ScreenMain.bottomBar.GetTab(TabType.Character).GetComponent<RectTransform>(), new Vector2(0, 0.01f));
            while (!Locator.Main.CharacterHomeTab.gameObject.activeInHierarchy)
                yield return null;
            ResetHole();
            yield return new WaitForSeconds(1f);
            HighlightTarget(Locator.Main.CharacterHomeTab.characterSelectionTab.levelUpCharacterButton.GetComponent<RectTransform>());
            yield return WaitForEvent(EventList.GamePlay.UPGRADE_CHARACTER);
            
            tutorialScreenImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            HighlightTarget(Locator.Main.CharacterHomeTab.characterSelectionTab.selectCharacterButton.GetComponent<RectTransform>());
            yield return WaitForEvent(EventList.GamePlay.SET_NEW_CHARACTER);
            yield return new WaitForSeconds(0.5f);
            
            yield return null;
        }
        
        #endregion

        public void ResetHole()
        {
            // Collapse the hole: set min == max so no area is discarded
            maskMaterial.SetVector("_HoleRect", new Vector4(0, 0, 0, 0));
        }
        
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (holeTarget == null || canvas == null)
                return true; // block everything if no hole defined

            // Check if the screen point is inside the hole target
            return !RectTransformUtility.RectangleContainsScreenPoint(holeTarget, sp, eventCamera);
        }
        
        EventYieldWait WaitForEvent(string gameEvent)
        {
            var waitEvent = new EventYieldWait();
            m_DictYieldWait[gameEvent] = waitEvent;
            return waitEvent;
        }
    }
}
