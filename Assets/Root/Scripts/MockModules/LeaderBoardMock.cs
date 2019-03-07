using System;
using System.Collections.Generic;

namespace Root
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

        public int GetPlayerScore(string leaderBoardID, int defaultValue) {
			var dict = GetData ();
			if (dict.ContainsKey (leaderBoardID)) {
				return dict [leaderBoardID];
			}
			return defaultValue;
		}

        public void ShowLeaderBoard()
        {

        }

		Dictionary<string, int> GetData() {
			if (data == null) {
				data = new Dictionary<string, int> ();
			}
			return data;
		}

        public void SubmitPlayerScore(string leaderBoardID, int value, Action<bool> callback)
        {
            var dict = GetData ();
            if (dict.ContainsKey (leaderBoardID)) {
                dict [leaderBoardID] = value;
            } else {
                dict.Add (leaderBoardID, value);
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
	}
}

