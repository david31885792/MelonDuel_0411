using UnityEngine;

public static class ChallengeScore
{
    private const string BestScoreKey = "BestChallengeScore_ClearAndMatch";

    /// <summary>
    /// 현재 기록을 저장된 최고 기록과 비교하여 더 높으면 저장
    /// </summary>
    public static void SaveScore(int clearCount, int matchCount)
    {
        int bestClear, bestMatch;
        LoadScore(out bestClear, out bestMatch);

        if (IsNewScoreBetter(clearCount, matchCount, bestClear, bestMatch))
        {
            string scoreString = $"{clearCount}-{matchCount}";
            PlayerPrefs.SetString(BestScoreKey, scoreString);
            PlayerPrefs.Save();
            Debug.Log($"🎉 새로운 최고기록 저장: {scoreString}");
        }
        else
        {
            Debug.Log("ℹ️ 기존 최고기록이 더 높음, 저장되지 않음.");
        }
    }

    /// <summary>
    /// 저장된 최고기록을 불러옴
    /// </summary>
    public static void LoadScore(out int clearCount, out int matchCount)
    {
        string saved = PlayerPrefs.GetString(BestScoreKey, "0-0");
        string[] split = saved.Split('-');
        int.TryParse(split[0], out clearCount);
        int.TryParse(split[1], out matchCount);
    }

    /// <summary>
    /// 점수 비교 로직: 더 높은 클리어 수가 우선, 같을 경우 일치 수 비교
    /// </summary>
    public static bool IsNewScoreBetter(int newClear, int newMatch, int bestClear, int bestMatch)
    {
        if (newClear > bestClear) return true;
        if (newClear == bestClear && newMatch > bestMatch) return true;
        return false;
    }

    /// <summary>
    /// 최고기록을 문자열로 가져옴
    /// </summary>
    public static string GetBestScoreText()
    {
        int c, m;
        LoadScore(out c, out m);
        return $"{c}-{m}";
    }
}
