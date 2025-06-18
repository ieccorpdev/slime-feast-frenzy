using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class PopupCharacterList : BasePopup
    {
        public RectTransform uiCardContainer;
        public CharacterUICard uiCardPrefab;
        public List<CharacterUICard> listCharacterUICard = new();

        public override void Show()
        {
            base.Show();
            Setup();
        }

        public void Setup()
        {
            foreach (var card in listCharacterUICard)
            {
                card.gameObject.SetActive(false);
            }

            var characterList = Config.allCharactersConfig.GetListCharacterConfigNotDefault();
            
            for (int i = 0; i < characterList.Count; i++)
            {
                if (i >= listCharacterUICard.Count)
                {
                    listCharacterUICard.Add(uiCardPrefab.Spawn(uiCardContainer));
                }

                var uiCard = listCharacterUICard[i];
                uiCard.transform.localScale = Vector3.one;
                
                uiCard.Setup(characterList[i]);
                uiCard.gameObject.SetActive(true);
            }
        }
    }
}
