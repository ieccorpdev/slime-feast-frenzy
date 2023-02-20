using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameInputValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        if (char.IsLetterOrDigit(ch) && text.Length <= 12)
        {
            text += ch;
            return ch;
        }
        return (char)0;
    }



}
