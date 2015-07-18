using UnityEngine;

namespace GameJolt.API
{
	[System.Serializable]
	public class Settings : ScriptableObject {
		[Header("Game")]
		[Tooltip("The game ID. It can be found on the Game Jolt website under Dashboard > YOUR-GAME > Game API > API Settings.")]
        public int gameID = 80387;
		[Tooltip("The game Private Key. It can be found on the Game Jolt website under Dashboard > YOUR-GAME > Game API > API Settings.")]
        public string privateKey = "868a95515c29e7177c38d24d59c75c5d";

		[Header("Settings")]
		[Tooltip("The time in seconds before an API call should timeout and return failure.")]
		public float timeout = 10f;
		[Tooltip("Automatically create and ping sessions once a user has been authentified.")]
		public bool autoPing = true;
		[Tooltip("Cache High Score Tables and Trophies information for faster display.")]
		public bool useCaching = true;

		[Header("Debug")]
		[Tooltip("AutoConnect in the Editor as if the game was hosted on GameJolt.")]
		public bool autoConnect = false;
		[Tooltip("The username to use for AutoConnect.")]
		public string user;
		[Tooltip("The token to use for AutoConnect.")]
		public string token;
	}
}