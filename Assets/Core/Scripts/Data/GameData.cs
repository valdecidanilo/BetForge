using System.Collections.Generic;

namespace Core.Scripts.Data
{
    public class GameData
    {
        private static readonly Dictionary<string, object> _gameData = new ();

        public static void SetData(string key, object data) => _gameData[key] = data;
        public static string GetStringData(string key) => _gameData[key].ToString();
        public static T GetData<T>(string key) => (T)_gameData[key];

    }
}