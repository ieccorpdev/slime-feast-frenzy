using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;
using IEC;
using NaughtyAttributes;
using TMPro;

namespace EasyUI.PickerWheelUI
{
    public class PickerWheel : MonoBehaviour
    {
        [Header("References :")]
        [SerializeField]
        private GameObject linePrefab;

        [SerializeField] private Transform linesParent;

        [Space][SerializeField] private Transform PickerWheelTransform;
        [SerializeField] private Transform wheelCircle;
        [SerializeField] private GameObject wheelPiecePrefab;
        [SerializeField] private Transform wheelPiecesParent;

        [Space]
        [Header("Picker wheel settings :")]
        [Range(1, 20)]
        public int spinDuration = 8;

        [SerializeField][Range(.2f, 2f)] private float wheelSize = 1f;

        [Space]
        [Header("Picker wheel pieces :")]
        public WheelPiece[] wheelPieces;

        [Header("IEC Custom")]
        public DataRewardLuckyWheelConfig dataWheel;
        // Events
        private UnityAction onSpinStartEvent;
        private UnityAction<WheelPiece> onSpinEndEvent;
        public List<RewardsLuckyWheel> listRewardLuckyWheel;

        private bool _isSpinning = false;

        public bool IsSpinning
        {
            get { return _isSpinning; }
        }


        private Vector2 pieceMinSize = new Vector2(81f, 146f);
        private Vector2 pieceMaxSize = new Vector2(144f, 213f);
        private int piecesMin = 2;
        private int piecesMax = 12;

        private float pieceAngle;
        private float halfPieceAngle;
        private float halfPieceAngleWithPaddings;


        private double accumulatedWeight;
        private System.Random rand = new System.Random();

        private List<int> nonZeroChancesIndices = new List<int>();
        private bool wasGenerate;
        private void Start()
        {

        }

        public void SetupWheel()
        {
            pieceAngle = 360 / wheelPieces.Length;
            halfPieceAngle = pieceAngle / 2f;
            halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f);
            Generate();
            CalculateWeightsAndIndices();
            if (nonZeroChancesIndices.Count == 0)
                Debug.LogError("You can't set all pieces chance to zero");
        }
        [Button("Load Data Wheel")]
        public void LoadDataWheel()
        {
            int numberPicec = listRewardLuckyWheel.Count;
            wheelPieces = new WheelPiece[numberPicec];
            for (int i = 0; i < wheelPieces.Length; i++)
            {
                WheelPiece data = new WheelPiece();
                data.Icon = listRewardLuckyWheel[i].icon;
                data.Label = "";
                data.Amount = listRewardLuckyWheel[i].dataReward.rewardAmount;
                data.Chance = listRewardLuckyWheel[i].percentageRandom;
                data.dataReward = listRewardLuckyWheel[i];
                wheelPieces[i] = data;
            }
            CalculateWeightsAndIndices();
        }
        private void Generate()
        {
            if (wasGenerate) return;
            wheelPiecePrefab = InstantiatePiece();
            LoadDataWheel();
            RectTransform rt = wheelPiecePrefab.transform.GetChild(0).GetComponent<RectTransform>();
            float pieceWidth = Mathf.Lerp(pieceMinSize.x, pieceMaxSize.x, 1f - Mathf.InverseLerp(piecesMin, piecesMax, wheelPieces.Length));
            float pieceHeight = Mathf.Lerp(pieceMinSize.y, pieceMaxSize.y, 1f - Mathf.InverseLerp(piecesMin, piecesMax, wheelPieces.Length));
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pieceWidth);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pieceHeight);

            for (int i = 0; i < wheelPieces.Length; i++)
                DrawPiece(i);

            wheelPiecePrefab.gameObject.SetActive(false);
            wasGenerate = true;
        }

        private void DrawPiece(int index)
        {
            WheelPiece piece = wheelPieces[index];
            Transform pieceTrns = InstantiatePiece().transform;
            var img = pieceTrns.GetChild(0).GetComponent<Image>();
            img.sprite = piece.Icon;
            img.SetNativeSize();
            //pieceTrns.GetChild(1).GetComponent<Text>().text = piece.Label;
            pieceTrns.GetChild(1).GetComponent<TextMeshProUGUI>().text = piece.dataReward.dataReward.typeReward == ResourceType.UnlimitedLive ? (piece.Amount.ToString() + "m") : ("x" + piece.Amount.ToString());
            var pos = wheelPiecesParent.position;
            pieceTrns.RotateAround(pos, Vector3.back, pieceAngle * index);
        }

        private GameObject InstantiatePiece()
        {
            return Instantiate(wheelPiecePrefab, wheelPiecesParent.transform.position, Quaternion.identity, wheelPiecesParent);
        }
        [Button("Spin")]
        public void Spin(int debugSpinNum = -1)
        {
            if (!_isSpinning)
            {
                _isSpinning = true;
                if (onSpinStartEvent != null)
                    onSpinStartEvent.Invoke();

                int index = debugSpinNum != -1 ? debugSpinNum : GetRandomPieceIndex();

                index = Mathf.Clamp(index ,0, wheelPieces.Length - 1);
                WheelPiece piece = wheelPieces[index];

                if (piece.Chance == 0 && nonZeroChancesIndices.Count != 0)
                {
                    index = nonZeroChancesIndices[Random.Range(0, nonZeroChancesIndices.Count)];
                    piece = wheelPieces[index];
                }

                float angle = -(pieceAngle * index);

                float rightOffset = (angle - halfPieceAngleWithPaddings) % 360;
                float leftOffset = (angle + halfPieceAngleWithPaddings) % 360;

                float randomAngle = Random.Range(leftOffset, rightOffset);

                Vector3 targetRotation = Vector3.back * (randomAngle + 2 * 360 * spinDuration);

                //float prevAngle = wheelCircle.eulerAngles.z + halfPieceAngle ;
                float prevAngle, currentAngle;
                prevAngle = currentAngle = wheelCircle.eulerAngles.z;

                bool isIndicatorOnTheLine = false;

                wheelCircle
                    .DORotate(targetRotation, spinDuration, RotateMode.Fast)
                    .SetEase(Ease.InOutQuart)
                    .OnUpdate(() =>
                    {
                        float diff = Mathf.Abs(prevAngle - currentAngle);
                        if (diff >= halfPieceAngle)
                        {
                            if (isIndicatorOnTheLine)
                            {
                                //audioSource.PlayOneShot(audioSource.clip);
                                //EventController.RaiseEvent(EventList.Audio.LUCKY_WHEEL_TICK);
                            }

                            prevAngle = currentAngle;
                            isIndicatorOnTheLine = !isIndicatorOnTheLine;
                        }

                        currentAngle = wheelCircle.eulerAngles.z;
                    })
                    .OnComplete(() =>
                    {
                        _isSpinning = false;
                        if (onSpinEndEvent != null)
                            onSpinEndEvent.Invoke(piece);
                        
                        //EventController.RaiseEvent(EventList.Audio.LUCKY_WHEEL_END);

                        //onSpinStartEvent = null;
                        //onSpinEndEvent = null;
                    });
            }
        }

        private void FixedUpdate()
        {
        }

        public void OnSpinStart(UnityAction action)
        {
            onSpinStartEvent = action;
        }

        public void OnSpinEnd(UnityAction<WheelPiece> action)
        {
            onSpinEndEvent = action;
        }


        private int GetRandomPieceIndex()
        {
            double r = rand.NextDouble() * accumulatedWeight;

            for (int i = 0; i < wheelPieces.Length; i++)
                if (wheelPieces[i]._weight >= r)
                    return i;

            return 0;
        }

        private void CalculateWeightsAndIndices()
        {
            accumulatedWeight = 0;
            for (int i = 0; i < wheelPieces.Length; i++)
            {
                WheelPiece piece = wheelPieces[i];

                //add weights:
                accumulatedWeight += piece.Chance;
                piece._weight = accumulatedWeight;

                //add index :
                piece.Index = i;

                //save non zero chance indices:
                if (piece.Chance > 0)
                    nonZeroChancesIndices.Add(i);
            }
        }


        private void OnValidate()
        {
            if (PickerWheelTransform != null)
                PickerWheelTransform.localScale = new Vector3(wheelSize, wheelSize, 1f);

            if (wheelPieces.Length > piecesMax || wheelPieces.Length < piecesMin)
                Debug.LogError("[ PickerWheelwheel ]  pieces length must be between " + piecesMin + " and " +
                               piecesMax);
        }
    }
}