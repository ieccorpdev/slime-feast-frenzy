using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class HomeScreenTutorial : _BaseBehaviour, ICanvasRaycastFilter
    {
        Dictionary<string, EventYieldWait> m_DictYieldWait = new Dictionary<string, EventYieldWait>();
        public Animator handTutAnimator;
        public Image tutorialScreenImage;
        public Material maskMaterial;
        public Canvas canvas;

        [Header("Tut Text")] public GameObject tutTextGroup;
        public GameObject aboveTutImg;
        public GameObject belowTutImg;
        public TextMeshProUGUI tutText;
        public float highlightTextSizeIncrease;
        public Color highlightTextColor;

        private Coroutine characterShopTutorialCoroutine;

        private RectTransform holeTarget;
        
        private bool _isAllowTutInteract;

        private void Start()
        {
            
        }
        
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_isAllowTutInteract)
                {
                    _isAllowTutInteract = false;
                }
            }
        }

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
            PlayerData.AddCharacterShards
            (character.characterName,
                character.GetTotalShardForMaxLevel());
            Locator.Service.GameResourceDisplay.HideDisplayUnderBG();
            tutorialScreenImage.gameObject.SetActive(true);
            var characterTabBtn = Locator.Main.ScreenMain.bottomBar.GetTab(TabType.Character)
                .GetComponent<RectTransform>();
            HighlightTarget(characterTabBtn, new Vector2(0, 0.01f));
            Locator.Main.HighLightCanvas.HighLightUI(characterTabBtn, true, true, null, false);
            SetTutText("Open the\nCharacter Shop", "Character Shop",
                characterTabBtn.transform.position + Vector3.up * 1.3f + Vector3.left * 0.2f);
            while (!Locator.Main.CharacterHomeTab.gameObject.activeInHierarchy)
                yield return null;
            Locator.Main.HighLightCanvas.RemoveCurrentHighLight();
            ResetHole();
            yield return new WaitForSeconds(1f);

            var levelUpBtn = Locator.Main.CharacterHomeTab.characterSelectionTab.levelUpCharacterButton
                .GetComponent<RectTransform>();
            //HighlightTarget(Locator.Main.CharacterHomeTab.characterSelectionTab.mainCharacterImage.GetComponent<RectTransform>(), new Vector2(-0.1f, -0.03f));
            var mainCharImg = Locator.Main.CharacterHomeTab.characterSelectionTab.mainCharacterImage
                .GetComponent<RectTransform>();
            Locator.Main.HighLightCanvas.TempHighlightUI(
                mainCharImg,
                true,
                false,
                null,
                false
            );
            Locator.Main.HighLightCanvas.HighLightUI(levelUpBtn, true, true, null, false);
            // Locator.Main.HighLightCanvas.HighLightUI(
            //     Locator.Main.CharacterHomeTab.characterSelectionTab.mainCharacterImage.GetComponent<RectTransform>()
            //     , false, false, null, false);
            SetTutText("Tap to collect\nJackie", "Jackie", levelUpBtn.transform.position + Vector3.up * 1.2f);
            //yield return WaitForEvent("EventList.GamePlay.UPGRADE_CHARACTER");
            while (!Locator.Main.PopupUnlockCharacter.gameObject.activeInHierarchy)
                yield return null;
            Locator.Main.HighLightCanvas.RemoveCurrentHighLight();
            ResetHole();
            yield return new WaitForSeconds(0.5f);

            while (Locator.Main.PopupUnlockCharacter.gameObject.activeInHierarchy)
                yield return null;

            yield return new WaitForSeconds(0.5f);

            //SetTutInteractButton();
            var groupInfo = Locator.Main.CharacterHomeTab.characterSelectionTab.groupCharacterInfo;
            HighlightTarget(groupInfo.GetComponent<RectTransform>());
            SetTutText(
                "Jackie is a shark\nwith special skill",
                new List<string>() { "Jackie", "special skill" },
                groupInfo.transform.position + Vector3.down * 1.5f,
                false);

            //yield return new WaitForSeconds(2f);

            _isAllowTutInteract = true;
            while (_isAllowTutInteract)
            {
                yield return null;
            }

            Locator.Main.HighLightCanvas.RemoveTempHighlight(mainCharImg);
            ResetHole();
            yield return new WaitForSeconds(0.5f);

            //HighlightTarget(levelUpBtn);
            Locator.Main.HighLightCanvas.HighLightUI(levelUpBtn, true, true, null, false);
            Locator.Main.CharacterHomeTab.characterSelectionTab.UpdateCurrentCharacterUI();
            SetTutText("Use Jackie Shards\nto upgrade its ability", "Jackie Shards",
                levelUpBtn.transform.position + Vector3.up * 1.2f);
            // while (!Locator.Main.PopupUnlockCharacter.gameObject.activeInHierarchy)
            //     yield return null;
            while (PlayerData.GetCharacterLevel(character.characterName) == 0)
                yield return null;
            Locator.Main.HighLightCanvas.RemoveCurrentHighLight();
            //yield return WaitForEvent("EventList.GamePlay.UPGRADE_CHARACTER");
            ResetHole();
            yield return new WaitForSeconds(0.5f);

            var selectBtn = Locator.Main.CharacterHomeTab.characterSelectionTab.selectCharacterButton
                .GetComponent<RectTransform>();
            Locator.Main.HighLightCanvas.HighLightUI(selectBtn, true, true, null, false);
            SetTutText("Switch to\nJackie", "Jackie", selectBtn.transform.position + Vector3.up * 1.2f);
            //HighlightTarget(selectBtn);
            //yield return WaitForEvent("EventList.GamePlay.SET_NEW_CHARACTER");
            while (PlayerData.playerCharacterName.Equals(Config.allCharactersConfig.GetDefaultCharacterConfig()
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

        private void SetTutText(string text, string highlightText, Vector3 position, bool isAbove = true)
        {
            tutTextGroup.SetActive(true);
            tutTextGroup.transform.position = position;

            aboveTutImg.SetActive(isAbove);
            belowTutImg.SetActive(!isAbove);
            //tutText.text = text;

            var highlightTextSize = tutText.fontSize + highlightTextSizeIncrease;

            string formattedHighlight = $"<color={ColorToHex(highlightTextColor)}>{highlightText}</size></color>";

            string highlightedText = text.Replace(highlightText, formattedHighlight);

            tutText.text = highlightedText;
        }

        private void SetTutText(string text, List<string> listHighlightText, Vector3 position, bool isAbove = true)
        {
            tutTextGroup.SetActive(true);
            tutTextGroup.transform.position = position;

            aboveTutImg.SetActive(isAbove);
            belowTutImg.SetActive(!isAbove);
            //tutText.text = text;

            var highlightTextSize = tutText.fontSize + highlightTextSizeIncrease;
            string highlightedText = "";

            foreach (var txt in listHighlightText)
            {

                string formattedHighlight = $"<color={ColorToHex(highlightTextColor)}>{txt}</size></color>";

                highlightedText = text.Replace(txt, formattedHighlight);
            }

            tutText.text = highlightedText;
        }

        public string ColorToHex(Color color)
        {
            Color32 color32 = color; // Converts to 0–255 range
            return $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}";
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
    }
}
