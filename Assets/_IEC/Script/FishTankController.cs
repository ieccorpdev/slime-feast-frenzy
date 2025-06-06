using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using IEC;
using IEC.Common.Event;
using UnityEngine;

namespace IEC
{
    public class FishTankController : _BaseBehaviour
    {
        public FreeSwimingCollider characterHomeSwimming;
        public Camera targetCamera;

        private float delay;

        private float maxDelay = 1f;

        // Start is called before the first frame update
        void Start()
        {
            this.RegisterListener(EventList.GamePlay.SELECT_NEW_CHARACTER, UpdateCharacter);
            UpdateCharacter();
        }

        public LayerMask fishLayer;

        private void UpdateCharacter(object param = null)
        {
            var currentCharacter = Config.allCharactersConfig.GetCharacterConfig(PlayerData.playerCharacterName);

            var homeChar = currentCharacter.characterHomePrefab.Spawn(
                transform, 
                currentCharacter.characterControlPrefab.transform.position,
                currentCharacter.characterControlPrefab.transform.rotation
            );

            homeChar.speed = currentCharacter.characterAnimSpeed;

            if (characterHomeSwimming != null)
            {
                Destroy(characterHomeSwimming.gameObject);
            }

            characterHomeSwimming = homeChar.GetComponent<FreeSwimingCollider>();
            characterHomeSwimming.swimAreaMeshCollider = this.GetComponent<Collider>();
        }

        void Update()
        {
            delay -= Time.deltaTime;
            if (Input.GetMouseButtonDown(0) && delay < 0) // Left mouse button or screen touch
            {
                Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition); // Convert screen to ray
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, fishLayer))
                {
                    GameObject clickedObject = hit.collider.gameObject;
                    if (clickedObject.CompareTag("Fish"))
                    {
                        var fsc = clickedObject.GetComponent<FreeSwimingCollider>();
                        if (fsc != null)
                        {
                            fsc.TouchFish();
                            delay = maxDelay;
                        }
                    }
                }
            }
        }
    }
}
