using UnityEngine;
using System;
using System.Collections.Generic;

namespace IEC
{
    public interface ILeaderBoardService
    {
        void ConnectService(Action<bool> callback);

        int GetPlayerScore(string leaderBoardID, int defaultValue);

        void RequestPlayerScore(string boardId, Action<bool> callback = null);

        void RequestPlayerScore(string boardId1, string boardId2, Action<bool> callback = null);

        void RequestPlayerScoreEvent(string tableScoreName, string boardId, Action<bool> callback = null);

        void SubmitPlayerScore(string leaderBoardID, int value, Action<bool> callback);

        void SubmitPlayerScore(string tableScoreName, string leaderBoardID, int value, Action<bool> callback);

        void SubmitPlayerScore(string boardId1, int value1, string boardId2, int value2, Action<bool> callback);

        void ShowLeaderBoard();

        void ShowLeaderBoard(string boardId, ILeaderBoardView view);

        void ShowLeaderBoard(string boardId, ILeaderBoardView view, string tableScoreName);

        void ShowLeaderBoard(string boardId1, string boardId2, ILeaderBoardView2 view);

        string GetLastResult();

        void ShowLeaderBoardCache(string boardId1, string boardId2, string cacheTop, string cacheRank, ILeaderBoardView2 view);

        void ShowLeaderBoardCache(string boardId, string cacheTop, string cacheRank, ILeaderBoardView view);

        void SetIsCache(bool isCache);
    }

    public interface ILeaderBoardView
    {
        void Show(string boardId, Dictionary<string, int> scores, Dictionary<string, string> names, Dictionary<string, string> countries, Dictionary<string, bool> vips);
        void ShowRank(string boardId, string userId, int rank, int score, string country);
    }

    public interface ILeaderBoardView2
    {
        void Show(string boardId, string boardId2, Dictionary<string, Score2> scores, Dictionary<string, string> names, Dictionary<string, string> countries, Dictionary<string, bool> vips);
        void ShowRank2(string boardId, string boardId2, string userId, int rank, int score1, int score2, string country);
    }

    public struct Score2
    {
        public int x;
        public int y;
    }
}

