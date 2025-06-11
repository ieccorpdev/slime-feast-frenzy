using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityUtils;

namespace IEC
{
    public class CharacterSelectionUIScroller : _BaseBehaviour, IEnhancedScrollerDelegate
    {
        public EnhancedScroller scroller;
        public float cellSize = 205;
        public CharacterSelectionUI characterSelectionUIPrefab;
        
        public CharacterSelectionTab CharacterSelectionTab { get; private set; }

        public void Init(CharacterSelectionTab tab)
        {
            CharacterSelectionTab = tab;
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
        
        private void JumpToCurrent(string currentCharName = "")
        {
            if (String.IsNullOrEmpty(currentCharName))
                currentCharName = PlayerData.playerCharacterName;
            
            //InitScroller();
            int index = Config.allCharactersConfig.listAllCharactersConfig.FindIndex(data => data.characterName.Equals(currentCharName));
            //scroller.JumpToDataIndex(index, 0.5f, 0, true, EnhancedScroller.TweenType.easeInOutQuart, 0.2f);
            JumpToCurrent(index);
        }
        
        private void JumpToCurrent(int index)
        {
            InitScroller();
            scroller.JumpToDataIndex(index, 0.5f, 0, true, EnhancedScroller.TweenType.easeInOutQuart, 0.2f);
        }
        
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return Config.allCharactersConfig.listAllCharactersConfig.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return cellSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            CharacterSelectionUI cellView = scroller.GetCellView(characterSelectionUIPrefab) as CharacterSelectionUI;

            var charConfig = Config.allCharactersConfig.listAllCharactersConfig[dataIndex];

            if(cellView != null)
                cellView.Setup(charConfig);
            
            return cellView;
        }
    }
}
