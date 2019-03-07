using System;
using UnityEngine;

namespace Root
{
    public class ScoreModule : IScoreService
	{
		IDataService dataService;
        
		ILeaderBoardService leaderBoardService;

        public ScoreModule (IDataService dataService, ILeaderBoardService lbService)
        {
            this.dataService = dataService;
            this.leaderBoardService = lbService;
        }

        public int GetBestHighScore(string leaderBoardID)
        {
            bool isServerScore;
            return GetBestHighScore(leaderBoardID, out isServerScore);
        }

        public int GetBestHighScore (string leaderBoardID, out bool isServerScore) {
			int localScore = GetLocalScore (leaderBoardID);
			int serverScore = GetServerScore (leaderBoardID);
            isServerScore = false;

			if (localScore < serverScore) {
				localScore = serverScore;
				dataService.SetInt (leaderBoardID, serverScore);
                isServerScore = true;
			}

			return localScore;
		}

        public int GetBestLowScore(string leaderBoardID)
        {
            bool isServerScore;
            return GetBestLowScore(leaderBoardID, out isServerScore);
        }

        public int GetBestLowScore (string leaderBoardID, out bool isServerScore) {
			int localScore = GetLocalScore (leaderBoardID);
			int serverScore = GetServerScore (leaderBoardID);
            isServerScore = false;

			if (localScore > serverScore && serverScore > 0) {
				localScore = serverScore;
				dataService.SetInt (leaderBoardID, serverScore);
                isServerScore = true;
			}

			return localScore;
		}

		public bool SetNewBestHighScore (int score, string leaderBoardID) {
            bool isServerScore;
			int currentScore = GetBestHighScore (leaderBoardID, out isServerScore);

			if (score > currentScore) {
				dataService.SetInt (leaderBoardID, score);
                return true;
			}

            return false;
		}

		public bool SetNewBestLowScore (int score, string leaderBoardID) {
            bool isServerScore;
            int currentScore = GetBestLowScore (leaderBoardID, out isServerScore);

			if (score <= 0) {
				return false;
			}

			if (currentScore <= 0) {
				dataService.SetInt (leaderBoardID, score);
                return true;
			}
			else if (score < currentScore) {
				dataService.SetInt (leaderBoardID, score);
                return true;
			}

            return false;
		}

        public bool SubmitBestHighScore (string leaderBoardID, Action<bool> callback) {
			int localScore = GetLocalScore (leaderBoardID);
			int serverScore = GetServerScore (leaderBoardID);

            if (localScore > serverScore)
            {
                leaderBoardService.SubmitPlayerScore(leaderBoardID, localScore, callback);
                return true;
            }

            Invoke(callback, false);
            return false;
		}

        public bool SubmitBestHighScore2(string boardId, string boardId2, Action<bool> callback)
        {
            int localScore = GetLocalScore (boardId);
            int serverScore = GetServerScore (boardId);
            int localScore2 = GetLocalScore (boardId2);
            int serverScore2 = GetServerScore (boardId2);

            if (localScore > serverScore || localScore2 > serverScore2)
            {
                leaderBoardService.SubmitPlayerScore(boardId, localScore, boardId2, localScore2, callback);
                return true;
            }

            Invoke(callback, false);
            return false;
        }

        public bool SubmitBestLowScore (string leaderBoardID, Action<bool> callback) {
			int localScore = GetLocalScore (leaderBoardID);
			int serverScore = GetServerScore (leaderBoardID);

			if (localScore <= 0) {
                Invoke(callback, false);
				return false;
			}

			if (serverScore <= 0) {
				leaderBoardService.SubmitPlayerScore (leaderBoardID, localScore, callback);
                return true;
			}
			else if (localScore < serverScore) {
				leaderBoardService.SubmitPlayerScore (leaderBoardID, localScore, callback);
                return true;
			}

            Invoke(callback, false);
            return false;
		}

		int GetServerScore(string leaderBoardID) {
			return leaderBoardService.GetPlayerScore(leaderBoardID, 0);
		}

		int GetLocalScore(string leaderBoardID) {
			return dataService.GetInt (leaderBoardID, 0);
		}

        void Invoke(Action<bool> callback, bool result)
        {
            if (callback != null)
            {
                callback(result);
            }
        }

	}
}

