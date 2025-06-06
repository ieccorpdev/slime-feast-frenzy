using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace IEC
{
    public class CharacterSelectionUIScroller : _BaseBehaviour, IEnhancedScrollerDelegate
    {
        public EnhancedScroller scroller;
        public CharacterSelectionUI characterSelectionUIPrefab;

        public void Init()
        {
            InitScroller();
            ReloadData();
        }
        
        private void InitScroller()
        {
            if (scroller.Delegate == null)
            {
                scroller.Delegate = this;
            }
            else
            {
                LoadData();
            }
        }
        
        public void LoadData()
        {
            scroller.RefreshActiveCellViews();
        }
        
        public void ReloadData()
        {
            scroller.ReloadData();
            JumpToCurrent();
        }
        
        private void JumpToCurrent()
        {
            InitScroller();
            int index = Config.allCharactersConfig.listAllCharactersConfig.FindIndex(data => data.characterName.Equals(PlayerData.playerCharacterName));
            scroller.JumpToDataIndex(index, 0.5f, 0, true, EnhancedScroller.TweenType.easeInOutQuart, 0.2f);
        }
        
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return Config.allCharactersConfig.listAllCharactersConfig.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return 162;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            CharacterSelectionUI cellView = scroller.GetCellView(characterSelectionUIPrefab) as CharacterSelectionUI;

            if(cellView != null)
                cellView.Setup(Config.allCharactersConfig.listAllCharactersConfig[dataIndex]);
            
            return cellView;
        }
    }
}
