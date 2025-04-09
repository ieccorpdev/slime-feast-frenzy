using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UINameBracketingUtility
{
    [MenuItem("GameObject/UI/Prefix Name/[Button]", false, 0)]
    private static void AddButtonPrefix()
    {
        AddPrefixToSelectedObject("Button");
    }

    [MenuItem("GameObject/UI/Prefix Name/[Image]", false, 0)]
    private static void AddImagePrefix()
    {
        AddPrefixToSelectedObject("Image");
    }

    [MenuItem("GameObject/UI/Prefix Name/[Text]", false, 0)]
    private static void AddTextPrefix()
    {
        AddPrefixToSelectedObject("Text");
    }

    [MenuItem("GameObject/UI/Prefix Name/[Button]", true)]
    [MenuItem("GameObject/UI/Prefix Name/[Image]", true)]
    [MenuItem("GameObject/UI/Prefix Name/[Text]", true)]
    private static bool ValidateSelectedObject()
    {
        return Selection.activeGameObject != null;
    }

    private static void AddPrefixToSelectedObject(string prefix)
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj, $"Add [{prefix}] Prefix");
            
            string currentName = obj.name;
            
            currentName = RemoveBrackets(currentName);
            
            obj.name = $"[{prefix}] {currentName}";
        }
    }

    private static string RemoveBrackets(string name)
    {
        while (name.StartsWith("[") && name.Contains("]"))
        {
            int closingBracketIndex = name.IndexOf(']');
            if (closingBracketIndex + 1 < name.Length)
            {
                name = name.Substring(closingBracketIndex + 2);
            }
            else
            {
                name = name.Substring(closingBracketIndex + 1);
            }
        }
        
        return name.Trim();
    }
    
    [MenuItem("GameObject/UI/Quick Create/Button", false, 10)]
    private static void CreateButtonWithPrefix()
    {
        GameObject buttonObj = new GameObject("[Button] New Button");
        buttonObj.AddComponent<Button>();
        buttonObj.AddComponent<Image>();
        Selection.activeGameObject = buttonObj;
    }

    [MenuItem("GameObject/UI/Quick Create/Image", false, 10)]
    private static void CreateImageWithPrefix()
    {
        GameObject imageObj = new GameObject("[Image] New Image");
        imageObj.AddComponent<Image>();
        Selection.activeGameObject = imageObj;
    }

    [MenuItem("GameObject/UI/Quick Create/Text", false, 10)]
    private static void CreateTextWithPrefix()
    {
        GameObject textObj = new GameObject("[Text] New Text");
        textObj.AddComponent<TextMeshProUGUI>();
        Selection.activeGameObject = textObj;
    }
}