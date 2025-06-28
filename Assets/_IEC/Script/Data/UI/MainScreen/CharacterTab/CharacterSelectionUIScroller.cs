using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Common.Extension;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityUtils;

namespace IEC
{
    public class CharacterSelectionUIScroller : _BaseBehaviour, IEnhancedScrollerDelegate, IPointerDownHandler, IPointerUpHandler
    {
        public EnhancedScroller scroller;
        public float cellSize = 205;
        public CharacterSelectionUI characterSelectionUIPrefab;
        
        public CharacterSelectionTab CharacterSelectionTab { get; private set; }
        
        public bool IsDragging { get; private set; }

        public void Init(CharacterSelectionTab tab)
        {
            CharacterSelectionTab = tab;
            InitScroller();
            ReloadData();
        }

        private void OnEnable()
        {
            this.RegisterListener(EventList.GamePlay.SELECT_NEW_CHARACTER, OnSelectNewCharacter);
        }

        private void OnDisable()
        {
            this.UnregisterListener(EventList.GamePlay.SELECT_NEW_CHARACTER, OnSelectNewCharacter);
        }

        private void OnSelectNewCharacter(object param)
        {
            var data = (CharacterEventData)param;
            if (data != null)
            {
                JumpToCurrent(data.characterName);
            }
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
            int index = Config.allCharactersConfig.GetListCharacterConfigNotDefault().FindIndex(data => data.characterName.Equals(currentCharName));
            //scroller.JumpToDataIndex(index, 0.5f, 0, true, EnhancedScroller.TweenType.easeInOutQuart, 0.2f);
            JumpToCurrent(index);
        }


        private void JumpToCurrent(int index)
        {
            //InitScroller();
            //scroller.JumpToDataIndex(index, 0.5f, 0.5f, true, EnhancedScroller.TweenType.easeInOutQuart, 0.2f);
            var space = 50;
            var pos = scroller.GetScrollPositionForDataIndex(index, EnhancedScroller.CellViewPositionEnum.Before) - (scroller.ScrollSize + space)/2 + cellSize;
            scroller.SetScrollPositionImmediately(pos);
        }
        
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return Config.allCharactersConfig.GetListCharacterConfigNotDefault().Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return cellSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            CharacterSelectionUI cellView = scroller.GetCellView(characterSelectionUIPrefab) as CharacterSelectionUI;

            var charConfig = Config.allCharactersConfig.GetListCharacterConfigNotDefault()[dataIndex];

            if(cellView != null)
                cellView.Setup(charConfig, CharacterSelectionTab.CurrentCharacterConfig.characterName);
            
            return cellView;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsDragging = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsDragging = false;
        }
    }
}
