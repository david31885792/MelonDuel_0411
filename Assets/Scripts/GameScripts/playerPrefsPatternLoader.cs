using UnityEngine;

public static class PlayerPrefsPatternLoader
{
    private const string patternKeyPrefix = "ClearPattern";

    /// <summary>
    /// 3x3 패턴을 PlayerPrefs에 저장 (DiceScene에서 호출)
    /// </summary>
    public static void SavePattern(string[,] pattern)
    {
        if (pattern == null || pattern.GetLength(0) != 3 || pattern.GetLength(1) != 3)
        {
            Debug.LogError("❌ SavePattern: 잘못된 배열 크기");
            return;
        }

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                string key = $"{patternKeyPrefix}_{row}_{col}";
                PlayerPrefs.SetString(key, pattern[row, col]);
            }
        }

        PlayerPrefs.Save();
        Debug.Log("✅ ClearPattern 저장 완료");
    }

    /// <summary>
    /// GameScene에서 3x3 패턴 불러오기
    /// </summary>
    public static string[,] LoadPattern()
    {
        string[,] pattern = new string[3, 3];

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                string key = $"{patternKeyPrefix}_{row}_{col}";
                string value = PlayerPrefs.GetString(key, "Red"); // 기본값 Red
                pattern[row, col] = value;
            }
        }

        Debug.Log("✅ ClearPattern 불러오기 완료");
        return pattern;
    }
}
