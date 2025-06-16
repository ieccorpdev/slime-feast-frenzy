using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class HomeScreenTutorial : _BaseBehaviour, ICanvasRaycastFilter, IController, IEventListener, IEventAwake, IEventStart
    {
        Dictionary<string, EventYieldWait> m_DictYieldWait = new Dictionary<string, EventYieldWait>();
        public Animator handTutAnimator;
        public Image tutorialScreenImage;
        public Material maskMaterial;
        public Canvas canvas;

        [Header("Tut Text")] 
        public GameObject tutTextGroup;
        public TextMeshProUGUI tutText;

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
            var character = Config.allCharactersConfig.GetListCharacterConfigNotDefault()[0];
            Locator.Service.GameResourceDisplay.HideDisplayUnderBG();
            tutorialScreenImage.gameObject.SetActive(true);
            var characterTabBtn = Locator.Main.ScreenMain.bottomBar.GetTab(TabType.Character)
                .GetComponent<RectTransform>();
            HighlightTarget(characterTabBtn, new Vector2(0, 0.01f));
            Locator.Main.HighLightCanvas.HighLightUI(characterTabBtn, true, true, null, false);
            SetTutText("Open the\nCharacter Shop", "Character Shop", characterTabBtn.transform.position + Vector3.up);
            while (!Locator.Main.CharacterHomeTab.gameObject.activeInHierarchy)
                yield return null;
            Locator.Main.HighLightCanvas.RemoveCurrentHighLight();
            ResetHole();
            yield return new WaitForSeconds(1f);

            var levelUpBtn = Locator.Main.CharacterHomeTab.characterSelectionTab.levelUpCharacterButton
                .GetComponent<RectTransform>();
            //HighlightTarget(levelUpBtn);
            Locator.Main.HighLightCanvas.HighLightUI(levelUpBtn, true, true, null, false);
            // Locator.Main.HighLightCanvas.HighLightUI(
            //     Locator.Main.CharacterHomeTab.characterSelectionTab.mainCharacterImage.GetComponent<RectTransform>()
            //     , false, false, null, false);
            SetTutText("Tap to collect\nJackie", "Jackie", levelUpBtn.transform.position + Vector3.up);
            //yield return WaitForEvent("EventList.GamePlay.UPGRADE_CHARACTER");
            while (!Locator.Main.PopupUnlockCharacter.gameObject.activeInHierarchy)
                yield return null;
            Locator.Main.HighLightCanvas.RemoveCurrentHighLight();
            ResetHole();
            yield return new WaitForSeconds(1f);
            
            
            //HighlightTarget(levelUpBtn);
            Locator.Main.HighLightCanvas.HighLightUI(levelUpBtn, true, true, null, false);
            PlayerData.AddCharacterShards
                (character.characterName, 
                    character.GetCharacterUpgrade(1).shardCost);
            Locator.Main.CharacterHomeTab.characterSelectionTab.UpdateCurrentCharacterUI();
            SetTutText("Use Jackie Shards\nto unlock its ability", "Jackie Shards", levelUpBtn.transform.position + Vector3.up);
            // while (!Locator.Main.PopupUnlockCharacter.gameObject.activeInHierarchy)
            //     yield return null;
            while (PlayerData.GetCharacterLevel(character.characterName) == 0)
                yield return null;
            Locator.Main.HighLightCanvas.RemoveCurrentHighLight();
            //yield return WaitForEvent("EventList.GamePlay.UPGRADE_CHARACTER");
            ResetHole();
            yield return new WaitForSeconds(1f);

            var selectBtn = Locator.Main.CharacterHomeTab.characterSelectionTab.selectCharacterButton
                .GetComponent<RectTransform>();
            Locator.Main.HighLightCanvas.HighLightUI(selectBtn, true, true, null, false);
            SetTutText("Switch to\nJackie", "Jackie", selectBtn.transform.position + Vector3.up);
            //HighlightTarget(selectBtn);
            //yield return WaitForEvent("EventList.GamePlay.SET_NEW_CHARACTER");
            while (PlayerData.playerCharacterName.Equals(Config.allCharactersConfig.GetDefaultCharacter()
                       .characterName))
                yield return null;
            ResetHole();
            // yield return new WaitForSeconds(0.5f);
            
            // HighlightTarget(Locator.Main.CharacterHomeTab.characterSelectionTab.selectionScroller.GetComponent<RectTransform>());
            //
            // var scroller = Locator.Main.CharacterHomeTab.characterSelectionTab.selectionScroller.scroller;
            //
            // var initialScrollPos = scroller.ScrollPosition;
            //
            // while (Mathf.Abs(scroller.ScrollPosition - initialScrollPos) <= Mathf.Epsilon)
            //     yield return null;
            
            tutorialScreenImage.gameObject.SetActive(false);
            
            yield return null;
        }
        
        #endregion

        private void SetTutText(string text, string highlightText, Vector3 position)
        {
            tutTextGroup.SetActive(true);
            tutTextGroup.transform.position = position;
            tutText.text = text;
        }

        public void ResetHole()
        {
            // Collapse the hole: set min == max so no area is discarded
            maskMaterial.SetVector("_HoleRect", new Vector4(0, 0, 0, 0));
            tutTextGroup.SetActive(false);
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

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourStart()
        {
            throw new NotImplementedException();
        }

        public void BehaviourAwake()
        {
            tutorialScreenImage.gameObject.SetActive(false);
            //hand.gameObject.SetActive(false);
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            if (m_DictYieldWait.TryGetValue(gameEvent, out var yieldWait))
            {
                yieldWait.isKeepWaiting = false;
                m_DictYieldWait.Remove(gameEvent);
            }
        }
    }
}
