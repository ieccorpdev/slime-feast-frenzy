using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _IECModules
{
    public class CheatCode : MonoBehaviour
    {
        public class CheatCodeParam
        {
            public CheatCodeType type;
            public Action<CheatCodeParam> action;
            public string buttonText;
            public List<InputParam> inputParams;
            public DropdownParam dropdownParam;
            public List<object> extraParameters;
        }

        public class DropdownParam
        {
            public int value;

            public string selected => options[value];

            public List<string> options;

            internal bool isShowOption;
            internal Vector2 scrollViewVector;
            internal Rect dropdownRect;
        }

        public class InputParam
        {
            public string label = "";
            public object value = "";
        }

        public enum CheatCodeType
        {
            Button,
            ButtonWithInputText,
            ButtonWithDropdown,
        }

        public enum ShowCheatCodeState
        {
            Hide,
            ShowPasswordInput,
            ShowCheatCode
        }

        private ShowCheatCodeState m_ShowCheatCodeState;

        public ShowCheatCodeState showCheatCodeState => m_ShowCheatCodeState;

        [NonSerialized] public UnityEvent onCheatCodeOpen = new UnityEvent();
        [NonSerialized] public UnityEvent onCheatCodeClose = new UnityEvent();
        [NonSerialized] public UnityEvent onCheatCodeStateChange = new UnityEvent();
        [NonSerialized] public bool haveOpenedCheatCode;

        public readonly List<CheatCodeParam> m_CheatCodeParamList = new List<CheatCodeParam>();

        private int m_TestIndex;
        private bool m_HaveInputPassword;
        private Vector2 m_ScrollviewVector2;

        string m_PasswordInputEditString = "";
        string m_InputSearchString = "";


        private static readonly int FontSize = Screen.width / 27;
        private static readonly Vector2 ButtonSize = new Vector2(FontSize * 10, FontSize * 2);

        private Texture2D m_BackgroundTexture;

        private GUIStyle m_TextureStyle;
        private GUIStyle m_ButtonStyle;
        private GUIStyle m_ButtonDropdownSelectedStyle;
        private GUIStyle m_InputFieldStyle;
        private GUIStyle m_LabelTextStyle;

        private void Awake()
        {
            m_BackgroundTexture = Texture2D.whiteTexture;
            m_TextureStyle = new GUIStyle {normal = new GUIStyleState {background = m_BackgroundTexture}};

            var reporter = GetComponentInChildren<Reporter>(true);
            AddCheatCodeButton("Show Reporter", param =>
            {
                if (reporter != null) reporter.doShow();
                SetCheatCodeState(ShowCheatCodeState.Hide);
            });

            var showFPS = GetComponentInChildren<ShowFPS>(true);
            AddCheatCodeButton("Show FPS", param =>
            {
                showFPS.gameObject.SetActive(!showFPS.gameObject.activeSelf);
                param.buttonText = showFPS.gameObject.activeSelf ? "Hide FPS" : "Show FPS";
            });


            AddCheatCodeButton("Toggle Debug Log", param =>
            {
                Debug.unityLogger.filterLogType = (Debug.unityLogger.filterLogType == LogType.Warning) ? LogType.Log : LogType.Warning;
                param.buttonText = "Debug Log: " + (Debug.unityLogger.filterLogType == LogType.Log ? "enabled" : "disabled");
                PlayerPrefs.SetInt(ModuleController.LOG_ENABLED_KEY, Debug.unityLogger.filterLogType == LogType.Log ? 1 : 0);
            });

            AddCheatCodeButtonWithInput("Time Scale", param =>
            {
                Time.timeScale = float.Parse(param.inputParams[0].value.ToString());
            }, new InputParam
            {
                value = 2
            });

//            AddTestButton();
        }

        private void AddTestButton()
        {
            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Test Button + Input",
                cheatCodeParam => { cheatCodeParam.inputParams.ForEach(print); }, new InputParam
                {
                },
                new InputParam
                {
                    label = "y",
                    value = 5
                });

            ModuleController.cheatCode.AddCheatCodeButtonWithDropdown("Test Dropdown",
                cheatCodeParam => { print(cheatCodeParam.dropdownParam.value); }, new List<string>
                {
                    "Options 1",
                    "Options 2",
                    "Options 3",
                    "Options 4",
                    "Options 5",
                    "Options 6",
                    "Options 7",
                    "Options 8",
                    "Options 9",
                    "Options 10",
                    "Options Long Long Long",
                    "Options 11",
                });

            AddCheatCodeButton("Test Add Button",
                param => { AddCheatCodeButton("Add Time: " + Time.time, param2 => { }); });
        }

        private void InitGuiParams()
        {
            if (m_ButtonStyle != null) return;
            m_ButtonStyle = new GUIStyle(GUI.skin.button) {fontSize = FontSize};

            m_ButtonDropdownSelectedStyle = new GUIStyle(m_ButtonStyle) {normal = {textColor = Color.cyan}};

            m_InputFieldStyle = new GUIStyle(GUI.skin.textField) {fontSize = FontSize};

            m_LabelTextStyle = new GUIStyle(GUI.skin.label) {fontSize = FontSize, normal = {textColor = Color.white}};
        }


        private void Update()
        {
            if (!Input.GetMouseButtonUp(0)) return;

            var cornerSize = 0.2f;

            if (Input.mousePosition.x < Screen.width * cornerSize &&
                (Input.mousePosition.y < Screen.height * cornerSize ||
                 Input.mousePosition.y > Screen.height * (1 - cornerSize))
                || Input.mousePosition.x > Screen.width * (1 - cornerSize) &&
                (
// Remove bottom right corner
//                    Input.mousePosition.y < Screen.height * cornerSize ||
                 Input.mousePosition.y > Screen.height * (1 - cornerSize)))
            {
                m_TestIndex++;
                if (m_TestIndex != 8 && (m_TestIndex != 3 || !m_HaveInputPassword && !Application.isEditor)) return;
                m_TestIndex = 0;

                if (m_HaveInputPassword || Application.isEditor)
                {
                    SetCheatCodeState(ShowCheatCodeState.ShowCheatCode);
                }
                else
                {
                    SetCheatCodeState(ShowCheatCodeState.ShowPasswordInput);
                }
            }
            else
            {
                m_TestIndex = 0;
            }
        }

        private void OnGUI()
        {
            InitGuiParams();
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            var screenWidthCenter = (float) screenWidth / 2;

            switch (showCheatCodeState)
            {
                case ShowCheatCodeState.Hide:
                    break;
                case ShowCheatCodeState.ShowPasswordInput:
                    var passwordInputTextSize = new Vector2(screenWidthCenter, 300);
                    var passwordInputTextRect = new Rect((screenWidth - passwordInputTextSize.x) / 2,
                        50, passwordInputTextSize.x, passwordInputTextSize.y);
                    m_PasswordInputEditString = GUI.TextField(passwordInputTextRect, m_PasswordInputEditString,
                        m_InputFieldStyle);

                    if (m_PasswordInputEditString.ToLower().Trim() == "iec123456")
                    {
                        m_HaveInputPassword = true;
                        SetCheatCodeState(ShowCheatCodeState.ShowCheatCode);
                    }

                    if (GuiButton(
                        new Rect(0, passwordInputTextRect.y + passwordInputTextRect.height + ButtonSize.y * 0.5f,
                            0, ButtonSize.y), "Send"))
                    {
                        SetCheatCodeState(ShowCheatCodeState.Hide);
                    }

                    break;
                case ShowCheatCodeState.ShowCheatCode:
                    DrawRect(new Rect(0, 0, screenWidth, screenHeight), new Color(0, 0, 0, 0.5f));


                    if (GuiButton(
                        new Rect(20, 20, 0, ButtonSize.y), "Close"))
                    {
                        SetCheatCodeState(ShowCheatCodeState.Hide);
                    }
                    
                    m_InputSearchString = GUI.TextField(new Rect(screenWidth - 20 - 300, 20,
                            300, ButtonSize.y), m_InputSearchString, m_InputFieldStyle);
                    
                    GUI.Label(new Rect(screenWidth - 20 - 300 - 150, 20,
                        150, ButtonSize.y), "Search", m_LabelTextStyle);

                    var cheatCodeRectHeight = screenHeight * 0.9f;
                    var cheatCodeRectStartY = screenHeight - cheatCodeRectHeight;
                    var cheatCodeSizeEach = ButtonSize.y * 1.2f;

                    var guiSkin = GUI.skin;
                    guiSkin.verticalScrollbar.fixedWidth = Screen.width * 0.05f;

                    var cheatCodeParamListFilter = m_CheatCodeParamList
                        .Where(x => string.IsNullOrEmpty(m_InputSearchString) || x.buttonText.ToLower().Contains(m_InputSearchString.ToLower()))
                        .ToList();
                    
                    m_ScrollviewVector2 = GUI.BeginScrollView(
                        new Rect(0, cheatCodeRectStartY, screenWidth, cheatCodeRectHeight),
                        m_ScrollviewVector2,
                        new Rect(0, cheatCodeRectStartY, screenWidth,
                            Mathf.Max(cheatCodeRectHeight, cheatCodeSizeEach * (cheatCodeParamListFilter.Count + 7))));

                    for (var i = 0; i < cheatCodeParamListFilter.Count; i++)
                    {
                        var cheatCodeParam = cheatCodeParamListFilter[i];

                        var commonButtonRect = new Rect(0, cheatCodeRectStartY + cheatCodeSizeEach * i, 0,
                            ButtonSize.y);

                        switch (cheatCodeParam.type)
                        {
                            case CheatCodeType.Button:
                                if (GuiButton(commonButtonRect, cheatCodeParam.buttonText))
                                {
                                    if (cheatCodeParam.action != null) cheatCodeParam.action.Invoke(cheatCodeParam);
                                }

                                break;
                            case CheatCodeType.ButtonWithInputText:
                                var buttonRatio = cheatCodeParam.inputParams.Count >= 2
                                    ? 2f / 3
                                    : 1f / 2;
                                var buttonRect = new Rect(commonButtonRect);
                                buttonRect.x = screenWidth * buttonRatio + 10;
                                if (GuiButton(buttonRect, cheatCodeParam.buttonText))
                                {
                                    if (cheatCodeParam.action != null) cheatCodeParam.action.Invoke(cheatCodeParam);
                                }

                                var labelSize = cheatCodeParam.inputParams.Select(x =>
                                    (int) m_LabelTextStyle.CalcSize(new GUIContent(x.label + ": ")).x).ToList();

                                var inputFieldWidth = Mathf.Min(screenWidth / 4,
                                    (screenWidth * buttonRatio - labelSize.Sum()) /
                                    cheatCodeParam.inputParams.Count);

                                var currentX = screenWidth * buttonRatio;
                                for (var j = cheatCodeParam.inputParams.Count - 1; j >= 0; j--)
                                {
                                    var inputParam = cheatCodeParam.inputParams[j];

                                    currentX = currentX - inputFieldWidth;
                                    inputParam.value = GUI.TextField(
                                        new Rect(currentX, buttonRect.y,
                                            inputFieldWidth, ButtonSize.y),
                                        inputParam.value.ToString(),
                                        m_InputFieldStyle);

                                    currentX = currentX - labelSize[j];
                                    if (!string.IsNullOrEmpty(inputParam.label))
                                    {
                                        GUI.Label(new Rect(currentX, buttonRect.y,
                                                labelSize[j], ButtonSize.y), inputParam.label + ": ", m_LabelTextStyle);
                                    }
                                }

                                break;
                            case CheatCodeType.ButtonWithDropdown:
                                var dropdownParam = cheatCodeParam.dropdownParam;

                                var maxWidth = Mathf.Clamp(dropdownParam.options.Max(option =>
                                        m_ButtonStyle.CalcSize(new GUIContent(option)).x), ButtonSize.y * 3,
                                    screenWidthCenter * 0.9f);

                                var dropdownRect = dropdownParam.dropdownRect = new Rect(screenWidthCenter,
                                    commonButtonRect.y, maxWidth,
                                    commonButtonRect.height);

                                if (GuiButton(dropdownRect,
                                    dropdownParam.isShowOption ? "Close" : dropdownParam.selected))
                                {
                                    dropdownParam.isShowOption = !dropdownParam.isShowOption;
                                }

                                var labelWidth = m_LabelTextStyle
                                    .CalcSize(new GUIContent(cheatCodeParam.buttonText + ": ")).x;
                                GUI.Label(
                                    new Rect(screenWidthCenter - labelWidth, dropdownRect.y, labelWidth,
                                        dropdownRect.height), cheatCodeParam.buttonText + ": ", m_LabelTextStyle);

                                if (!dropdownParam.isShowOption) continue;

                                var optionHeight = dropdownRect.height * 0.9f;
                                var windowMaxHeight = optionHeight * 10;
                                var scrollViewMaxHeight = dropdownParam.options.Count * optionHeight;
                                var scrollViewRect = new Rect(dropdownRect.x, dropdownRect.y + dropdownRect.height,
                                    dropdownRect.width, Mathf.Min(windowMaxHeight, scrollViewMaxHeight));

                                scrollViewRect.x -= m_ScrollviewVector2.x;
                                scrollViewRect.y -= m_ScrollviewVector2.y;

                                GUI.Window(i, scrollViewRect, id =>
                                {
                                    dropdownParam.scrollViewVector = GUI.BeginScrollView(
                                        new Rect(0, 0, scrollViewRect.width, scrollViewRect.height),
                                        dropdownParam.scrollViewVector,
                                        new Rect(0, 0, dropdownRect.width, scrollViewMaxHeight));

                                    GUI.Box(
                                        new Rect(0, 0, dropdownRect.width,
                                            Mathf.Max(dropdownRect.height,
                                                dropdownParam.options.Count * optionHeight)), "");

                                    for (var index = 0; index < dropdownParam.options.Count; index++)
                                    {
                                        if (GUI.Button(
                                            new Rect(0, index * optionHeight, dropdownRect.width, optionHeight),
                                            dropdownParam.options[index],
                                            index == dropdownParam.value
                                                ? m_ButtonDropdownSelectedStyle
                                                : m_ButtonStyle))
                                        {
                                            dropdownParam.isShowOption = false;
                                            dropdownParam.value = index;

                                            if (cheatCodeParam.action != null)
                                                cheatCodeParam.action.Invoke(cheatCodeParam);
                                        }
                                    }

                                    GUI.EndScrollView();
                                }, "");
                                break;
                        }
                    }

                    GUI.EndScrollView();
                    break;
            }
        }

        private bool GuiButton(Rect rect, string text, GUIStyle style = null)
        {
            Rect outRect;
            return GuiButton(rect, text, style, out outRect);
        }

        private bool GuiButton(Rect rect, string text, GUIStyle style, out Rect outRect)
        {
            outRect = new Rect(rect);
            if (style == null) style = m_ButtonStyle;
            if (outRect.width != 0f) return GUI.Button(outRect, text, style);

            var buttonWidth = style.CalcSize(new GUIContent(text)).x + 50;
            outRect.width = buttonWidth;
            if (outRect.x == 0f) outRect.x = Screen.width / 2 - buttonWidth / 2;

            return GUI.Button(outRect, text, style);
        }


        private void DrawRect(Rect position, Color color, GUIContent content = null)
        {
            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUI.Box(position, content ?? GUIContent.none, m_TextureStyle);
            GUI.backgroundColor = backgroundColor;
        }

        public void SetCheatCodeState(ShowCheatCodeState cheatCodeState)
        {
            var isShow = m_ShowCheatCodeState == ShowCheatCodeState.Hide && cheatCodeState != m_ShowCheatCodeState;

            m_ShowCheatCodeState = cheatCodeState;

            onCheatCodeStateChange.Invoke();
            if (showCheatCodeState == ShowCheatCodeState.Hide)
            {
                ToggleEventSystem(true);
                onCheatCodeClose.Invoke();
            }
            if (isShow)
            {
                haveOpenedCheatCode = true;
                ToggleEventSystem(false);
                onCheatCodeOpen.Invoke();
            }
        }

        public void ToggleEventSystem(bool toggle)
        {
            var eventSystem = FindObjectOfType<EventSystem>();
            if(!eventSystem) return;
            eventSystem.enabled = toggle;
        }

        private CheatCodeParam AddCheatCode(CheatCodeType cheatCodeType, string buttonText)
        {
            RemoveCheatCodeButtonByName(buttonText);
            var cheatCodeParam = new CheatCodeParam {type = cheatCodeType, buttonText = buttonText};
            m_CheatCodeParamList.Add(cheatCodeParam);
            return cheatCodeParam;
        }

        public CheatCodeParam GetCheatCodeParam(string buttonText)
        {
            return m_CheatCodeParamList.Find(x => x.buttonText == buttonText);
        }


        public void RemoveCheatCodeButtonByName(string buttonText)
        {
            m_CheatCodeParamList.RemoveAll(param => param.buttonText == buttonText);
        }

        public void AddCheatCodeButton(string buttonText, Action<CheatCodeParam> onClick)
        {
            var cheatCodeParam = AddCheatCode(CheatCodeType.Button, buttonText);
            cheatCodeParam.action = onClick;
        }

        public void AddCheatCodeButtonWithInput(string buttonText, Action<CheatCodeParam> onClick,
            params InputParam[] inputParams)
        {
            var cheatCodeParam = AddCheatCode(CheatCodeType.ButtonWithInputText, buttonText);
            cheatCodeParam.inputParams = inputParams.ToList();
            cheatCodeParam.action = onClick;
        }

        public void AddCheatCodeButtonWithDropdown(string buttonText, Action<CheatCodeParam> onClick,
            List<string> options, int defaultValue = 0)
        {
            var cheatCodeParam = AddCheatCode(CheatCodeType.ButtonWithDropdown, buttonText);
            cheatCodeParam.dropdownParam = new DropdownParam {options = options, value = defaultValue};
            cheatCodeParam.action = onClick;
        }
    }
}