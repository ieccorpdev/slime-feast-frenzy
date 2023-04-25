using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC.FakeLeaderboard
{
    [System.Serializable]
    public class FakeLeaderboardDataSave
    {
        public FakeLeaderboardUserData userData;
        public List<FakeLeaderboardUserData> top3ContainPlayer
        {
            get
            {
                List<FakeLeaderboardUserData> output = new List<FakeLeaderboardUserData>(top3);
                bool isInsert = false;
                for (int i = 0; i < output.Count; i++)
                {
                    if (output[i].trophy >= userData.trophy)
                    {
                        isInsert = true;
                        output.Insert(i, userData);
                        break;
                    }
                }

                if (!isInsert)
                    if (userData.trophy > output[output.Count - 1].trophy)
                    {
                        output.Add(userData);
                    }

                if (output.Count > 3)
                    output.RemoveAt(0);
                return output;
            }

        }
        public List<FakeLeaderboardUserData> top3;
        public List<FakeLeaderboardUserData> otherPlayers;

        public DateTime lastTime;
        public List<FakeLeaderboardUserData> allPlayers
        {
            get
            {
                if (top3 == null || otherPlayers == null || userData == null) return null;
                List<FakeLeaderboardUserData> output = new List<FakeLeaderboardUserData>();
                output.AddRange(otherPlayers);
                output.AddRange(top3);
                // output.Add(userData);
                bool isInsert = false;
                for (int i = 0; i < output.Count; i++)
                {
                    if (output[i].trophy >= userData.trophy)
                    {
                        output.Insert(i, userData);
                        isInsert = true;
                        break;
                    }
                }

                if (!isInsert)
                    if (userData.trophy > output[output.Count - 1].trophy)
                    {
                        output.Add(userData);
                    }

                for (int i = output.Count - 1; i >= 0; i--)
                {
                    output[i].index = output.Count - i;
                }
                //  output = output.OrderBy(x => x.trophy).ToList();
                return output;
            }
        }

        public List<FakeLeaderboardUserData> playersToShowOnListView
        {
            get
            {
                var allPlayerCache = allPlayers;
                List<FakeLeaderboardUserData> output = new List<FakeLeaderboardUserData>();
                int indexPlayer = IndexOfPlayer;
                //Index =100:
                //109 -> 100
                for (int i = Mathf.Min(allPlayerCache.Count - 1, indexPlayer + 9); i >= indexPlayer + 1; i--)
                {
                    output.Add(playerByIndex(i));
                }
                output.Add(userData);

                //99->90
                for (int i = indexPlayer - 1; i >= Mathf.Max(0, indexPlayer - 9); i--)
                {
                    output.Add(playerByIndex(i));
                }
                return output;
            }
        }

        public FakeLeaderboardUserData playerByIndex(int index)
        {
            try
            {
                return allPlayers[allPlayers.Count - index];
            }
            catch (System.Exception)
            {

                return null;
            }

        }

        public int IndexOfPlayer => allPlayers == null ? -1 : allPlayers.Count - allPlayers.IndexOf(userData);
    }
}
