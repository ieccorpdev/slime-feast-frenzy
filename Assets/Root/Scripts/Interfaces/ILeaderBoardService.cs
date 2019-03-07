using UnityEngine;
using System;
using System.Collections.Generic;

namespace Root
{
	public interface ILeaderBoardService
	{
        void ConnectService(Action<bool> callback);

        int GetPlayerScore(string leaderBoardID, int defaultValue);

        void SubmitPlayerScore(string leaderBoardID, int value, Action<bool> callback);

        void SubmitPlayerScore(string boardId1, int value1, string boardId2, int value2, Action<bool> callback);

        void ShowLeaderBoard();

        void ShowLeaderBoard(string boardId, ILeaderBoardView view);

        void ShowLeaderBoard(string boardId1, string boardId2, ILeaderBoardView2 view);
    }

    public interface ILeaderBoardView
    {
        void Show(string boardId, Dictionary<string, int> scores, Dictionary<string, string> names);
        void ShowRank(string boardId, string userId, int rank, int score);
    }

    public interface ILeaderBoardView2
    {
        void Show(string boardId, string boardId2, Dictionary<string, Score2> scores, Dictionary<string, string> names);
        void ShowRank2(string boardId, string boardId2, string userId, int rank, int score1, int score2);
    }

    public struct Score2
    {
        public int x;
        public int y;
    }
}

