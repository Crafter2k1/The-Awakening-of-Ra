using UnityEngine;

namespace MainTool.Utils
{
    public static class DataUtility
    {
        public static T ToDeserialize<T>(this string json) => JsonUtility.FromJson<T>(json);
        
        public static bool IsUrlSaved() => PlayerPrefs.HasKey(Constants.URL_KEY);
        
        public static bool AreCookiesSaved() => PlayerPrefs.HasKey(Constants.COOKIE_KEY);
        
        public static string GetSavedUrl() => PlayerPrefs.GetString(Constants.URL_KEY);

        public static string GetSavedCookies() => PlayerPrefs.GetString(Constants.COOKIE_KEY, null);
        
        public static void SaveUrl(string url)
        {
            PlayerPrefs.SetString(Constants.URL_KEY, url);
            PlayerPrefs.Save();
        }

        public static void SaveCookies(string cookies)
        {
            PlayerPrefs.SetString(Constants.COOKIE_KEY, cookies);
            PlayerPrefs.Save();
        }
    }
}