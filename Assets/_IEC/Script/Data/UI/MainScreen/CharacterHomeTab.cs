using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

public class CharacterHomeTab : ScreenNavigateUI
{
    public CharacterSelectionTab characterSelectionTab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Show()
    {
        characterSelectionTab.Setup();
        base.Show();
    }
}
