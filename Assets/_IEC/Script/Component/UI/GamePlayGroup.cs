using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class GamePlayGroup : _BaseBehaviour, IGroup
    {
        public Camera mainCamera;
        public Transform transTutHand;
        public Transform effectParent;
        public Transform[] wallParents;   // TOP, BOT, LEFT, RIGHT

        public List<CornerComponent> listWallCorners;   // TOP_LEFT, TOP_RIGHT, BOT_LEFT, BOT_RIGHT

        public Transform transGridParent;
        public Transform transMainCam;

        public SpriteRenderer sprBackground;

        public Image sprBGTop, sprBGBot;

        public Transform groupBigSquare;

        public List<BaseComponent> listGridConnectCache = new List<BaseComponent>();

        public BaseComponent gridCache;

        public WaterEffectData waterEffectData;

        public void LocateGroup()
        {
            Pool.Set(this);
        }

        public void SetBackground(int index)
        {
            var config = ShopConfig.listBGConfig[index];

            sprBackground.sprite = config.sprMain;

            var isHaveTop = config.sprTop != null;
            sprBGTop.gameObject.SetActive(isHaveTop);
            if (isHaveTop)
            {
                sprBGTop.sprite = config.sprTop;
            }

            var isHaveBot = config.sprBot != null;
            sprBGBot.gameObject.SetActive(isHaveBot);
            if (isHaveBot)
            {
                sprBGBot.sprite = config.sprBot;
            }
        }

        public int GetIndexSpawn(int row, int col)
        {
            if (Game.introType == 0)
            {
                if (Game.isCamRotated)
                {
                    return row - 1;
                }
                else
                {
                    return col - 1;
                }
            }
            else
            {
                if (Game.isCamRotated)
                {
                    return (Game.size - row - 2);
                }
                else
                {
                    return (Game.size - col - 2);
                }
            }
        }
    }
}