using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class EditProfileTabSelection : _BaseBehaviour
    {
        public Button btn;
        public ProfileSelectTab profileSelectTab;

        public Image btnSprite;

        public GameObject selectedGroup;
        public GameObject deselectedGroup;

        public Sprite selectedSprite;
        public Sprite deselectedSprite;

        public void SetSelected(bool selected)
        {
            btnSprite.sprite = selected ? selectedSprite : deselectedSprite;
            selectedGroup.gameObject.SetActive(selected);
            deselectedGroup.gameObject.SetActive(!selected);
        }
    }
}
