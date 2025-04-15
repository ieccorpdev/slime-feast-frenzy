#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FakeScreenCreator : EditorWindow
{
    [MenuItem("GameObject/Create Fake Screen")]
    static void CreatFakeScreenObject()
    {
        GameObject parent = Selection.activeObject as GameObject;
        if (parent == null)
        {
            EditorUtility.DisplayDialog("Select Parent", "You must select a parent first!", "OK");
            return;
        }
        GameObject newGO = null;
        // check if already has a fake screen
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            var child = parent.transform.GetChild(i);
            if (child.name == "Fake Screen" && child.tag == "EditorOnly")
            {
                newGO = child.gameObject;
                break;
            }
        }
        // create one if haven't yet
        if (newGO == null)
        {
            newGO = new GameObject();
            newGO.name = "Fake Screen";
            var rect = newGO.AddComponent<RectTransform>();
            newGO.tag = "EditorOnly";
            newGO.transform.SetParent(parent.transform);
            rect.sizeDelta = Vector2.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            rect.anchorMax = Vector2.one;
        }


        string path = EditorUtility.OpenFilePanel("Choose a image file", "", "jpg, png");
        if (path.Length != 0)
        {
            var fileContent = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileContent);
            texture.Apply();


            var image = newGO.GetComponent<Image>();
            if (image == null) image = newGO.AddComponent<Image>();


            image.sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), new Vector2(0.5f, 0.5f));

            //transparent image
            var color = image.color;
            color.a = 0.5f;
            image.color = color;
            texture = null;
        }
    }
}
#endif