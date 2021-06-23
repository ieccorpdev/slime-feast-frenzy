using System;

namespace IEC
{
    public interface IScoreService
    {
        int GetBestHighScore(string leaderBoardID);

        int GetBestHighScore(string leaderBoardID, out bool isServerScore);

        int GetBestLowScore(string leaderBoardID);

        int GetBestLowScore(string leaderBoardID, out bool isServerScore);

        bool SetNewBestHighScore(int score, string leaderBoardID);

        bool SetNewBestLowScore(int score, string leaderBoardID);

        bool SubmitBestHighScore(string leaderBoardID, Action<bool> callback);

        bool SubmitBestLowScore(string leaderBoardID, Action<bool> callback);

        bool SubmitBestHighScore2(string boardId, string boardId2, Action<bool> callback);

        int GetLocalScore(string leaderBoardID);

        int GetServerScore(string leaderBoardID, int defaultValue = 0);
    }
}

