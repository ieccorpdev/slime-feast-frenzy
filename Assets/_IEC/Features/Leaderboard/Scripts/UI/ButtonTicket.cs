using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ButtonTicket : _BaseBehaviour
    {
        public GameObject adsObj;
        public GameObject ticket;

        public Button btn => GetComponent<Button>();
        public void UpdateUI(bool isActive = true)
        {
            ticket.SetActive(PlayerData.ticket > 0 && isActive);
            if(adsObj != null) adsObj.SetActive(!ticket.activeSelf && isActive);
        }
    }
}