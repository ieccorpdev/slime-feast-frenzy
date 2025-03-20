using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace IEC
{
    [CreateAssetMenu(fileName = "MapTile", menuName = "Tiles/MapTile")]
    public class MapTile : RuleTile<MapTile.Neighbor>
    {
        [System.Serializable]
        public class MapTilingRule : RuleTile.TilingRule
        {
            [Tooltip("Rotation offset in degrees to apply when this rule is matched.")]
            public float rotationOffset = 0f;
        }

        [System.Serializable]
        public class Neighbor : RuleTile.TilingRuleOutput.Neighbor { }

        // Custom list of tiling rules
        public new List<MapTilingRule> m_MapTilingRules = new List<MapTilingRule>();

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            // Use base class logic to handle tile matching
            base.GetTileData(position, tilemap, ref tileData);

            // Determine which rule matched
            TilingRule matchedRule = GetMatchingRule(position, tilemap);

            if (matchedRule != null)
            {
                // Apply the rotation offset if defined
                if (matchedRule is MapTilingRule customRule && customRule.rotationOffset != 0f)
                {
                    var rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, customRule.rotationOffset), Vector3.one);
                    tileData.transform = rotationMatrix * tileData.transform;
                }
            }
        }

        private TilingRule GetMatchingRule(Vector3Int position, ITilemap tilemap)
        {
            foreach (var rule in m_MapTilingRules)
            {
                // Attempt to match the rule using the base matching logic
                if (RuleMatches(rule, position, tilemap))
                {
                    return rule;
                }
            }
            return null;
        }

        private bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap)
        {
            return true;
            // Call base class logic to check if the rule matches
            // return base.RuleMatches(rule, position, tilemap, out _);
        }
    }
}
