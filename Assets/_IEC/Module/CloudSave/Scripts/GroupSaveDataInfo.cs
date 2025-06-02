using System;
using TMPro;
using UnityEngine;

namespace IEC.CloudSave
{
    public class GroupSaveDataInfo : _BaseBehaviour
    {
        public TextMeshProUGUI txtSaveDate;
        public TextMeshProUGUI txtSaveCoin;
        public TextMeshProUGUI txtSaveLevel;

        public void Init(PlayerSecret data)
        {
            if (data == null) return;
            
            txtSaveDate.text = DateTimeHelper.TicksToDateTime(data.lastTickPlayTime).ToString("dd/MM/yyyy");
            txtSaveCoin.text = data.gold.ToString();
            txtSaveLevel.text = data.level.ToString();
        }
    }
}