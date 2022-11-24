using UnityEngine;
using System.Collections;

public class ShowFPS : MonoBehaviour
{
    public static float topOffset;
    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        var offsetY = topOffset * Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0 + offsetY, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        DrawTextWithOutline(rect, text, style, Color.black, Color.white, 1);
    }


    void DrawTextWithOutline(Rect centerRect, string text, GUIStyle style, Color borderColor, Color innerColor, int borderWidth)
    {
        // assign the border color
        style.normal.textColor = borderColor;

        // draw an outline color copy to the left and up from original
        Rect modRect = centerRect;
        modRect.x -= borderWidth;
        modRect.y -= borderWidth;

        // stamp copies from the top left corner to the top right corner
        while (modRect.x <= centerRect.x + borderWidth)
        {
            modRect.x++;
        }

        // stamp copies from the top right corner to the bottom right corner
        while (modRect.y <= centerRect.y + borderWidth)
        {
            modRect.y++;
        }

        // stamp copies from the bottom right corner to the bottom left corner
        while (modRect.x >= centerRect.x - borderWidth)
        {
            modRect.x--;
        }

        // stamp copies from the bottom left corner to the top left corner
        while (modRect.y >= centerRect.y - borderWidth)
        {
            modRect.y--;
        }

        GUI.Label(modRect, text, style);

        // draw the inner color version in the center
        style.normal.textColor = innerColor;
        GUI.Label(centerRect, text, style);
    }
}