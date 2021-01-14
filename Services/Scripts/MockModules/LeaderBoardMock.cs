using System;
using System.Collections.Generic;

namespace IEC
{
    public class LeaderBoardMock : ILeaderBoardService
    {
        Dictionary<string, int> data;

        public void ConnectService(Action<bool> callback)
        {
            if (callback != null)
            {
                callback(true);
            }
        }

        public int GetPlayerScore(string leaderBoardID, int defaultValue)
        {
            var dict = GetData();
            if (dict.ContainsKey(leaderBoardID))
            {
                return dict[leaderBoardID];
            }
            return defaultValue;
        }

        public void ShowLeaderBoard()
        {

        }

        Dictionary<string, int> GetData()
        {
            if (data == null)
            {
                data = new Dictionary<string, int>();
            }
            return data;
        }

        public void SubmitPlayerScore(string leaderBoardID, int value, Action<bool> callback)
        {
            var dict = GetData();
            if (dict.ContainsKey(leaderBoardID))
            {
                dict[leaderBoardID] = value;
            }
            else
            {
                dict.Add(leaderBoardID, value);
            }

            if (callback != null)
            {
                callback(true);
            }
        }

        public void SubmitPlayerScore(string boardId1, int value1, string boardId2, int value2, Action<bool> callback)
        {
            SubmitPlayerScore(boardId1, value1, null);
            SubmitPlayerScore(boardId2, value2, null);
            if (callback != null)
            {
                callback(true);
            }
        }

        public void ShowLeaderBoard(string boardId, ILeaderBoardView view)
        {
        }

        public void ShowLeaderBoard(string boardId1, string boardId2, ILeaderBoardView2 view)
        {
        }

        public int GetPlayerScore(string tableScoreName, string leaderBoardID, int defaultValue)
        {
            //throw new NotImplementedException();
            return defaultValue;
        }

        public void SubmitPlayerScore(string tableScoreName, string leaderBoardID, int value, Action<bool> callback)
        {
            //throw new NotImplementedException();
        }

        public void ShowLeaderBoard(string boardId, ILeaderBoardView view, string tableScoreName)
        {
            //throw new NotImplementedException();
        }

        public void RequestPlayerScore(string boardId, Action<bool> callback = null)
        {
            //throw new NotImplementedException();
        }

        public void RequestPlayerScore(string boardId1, string boardId2, Action<bool> callback = null)
        {
            //throw new NotImplementedException();
        }

        public void RequestPlayerScoreEvent(string tableScoreName, string boardId, Action<bool> callback = null)
        {
        }

        public string GetLastResult()
        {
            return string.Empty;   
        }

        public void ShowLeaderBoardCache(string boardId1, string boardId2, string cacheTop, string cacheRank, ILeaderBoardView2 view)
        {
            
        }

        public void SetIsCache(bool isCache)
        {
            
        }

        public void ShowLeaderBoardCache(string boardId, string cacheTop, string cacheRank, ILeaderBoardView view)
        {
            
        }
    }
}

