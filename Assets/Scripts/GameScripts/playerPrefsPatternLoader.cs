using UnityEngine;

public static class PlayerPrefsPatternLoader
{
    private const string KeyPrefix = "ClearPattern_";

    public static void SavePattern(string[,] pattern)
    {
        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
                PlayerPrefs.SetString(KeyPrefix + $"{y}_{x}", pattern[y, x]);

        PlayerPrefs.Save();
        Debug.Log("✅ [PatternLoader] 클리어 패턴 저장 완료");
    }

    public static string[,] LoadPattern()
    {
        string[,] pattern = new string[3, 3];

        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
                pattern[y, x] = PlayerPrefs.GetString(KeyPrefix + $"{y}_{x}", "None");

        Debug.Log("[PatternLoader] 클리어 패턴 불러오기 완료");
        return pattern;
    }
}
