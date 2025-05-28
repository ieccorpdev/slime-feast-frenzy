using System.Collections;
using System.Collections.Generic;
using IEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicButtonEvent : _BaseBehaviour
{
    public GameObject lockedGroup;
    public GameObject joinGroup;
    public GameObject claimRewardGroup;
    public GameObject completedGroup;
    public TextMeshProUGUI levelUnlockText;
    public Button button;

    public void UpdateUI()
    {
        completedGroup.SetActive(false);
        lockedGroup.SetActive(false);
        joinGroup.SetActive(false);
        claimRewardGroup.SetActive(false);

        if (PlayerData.level < Config.artifactCollectorConfig.levelUnlockFeature)
        {
            levelUnlockText.text = "Lvl. " + Config.artifactCollectorConfig.levelUnlockFeature;
            lockedGroup.SetActive(true);
            button.interactable = false;
        }
        else
        {
            var relicLevel = PlayerData.relicData.curRelicLevel;
            var config = Config.artifactCollectorConfig;
            var isMaxLevel = relicLevel == config.ArtifactLevels.Count;

            if (PlayerData.relicData.CheckIfHasReward(config))
            {
                claimRewardGroup.SetActive(true);
            }
            else
            {
                if (isMaxLevel)
                {
                    completedGroup.SetActive(true);
                }
                else
                {
                    joinGroup.SetActive(true);
                }
            }
        }
    }

    private void Update()
    {
        UpdateUI();
    }
}