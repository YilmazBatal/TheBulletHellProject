using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public PlayerData playerData;

	private string savePath;

	private void Awake() {
		savePath = Path.Combine(Application.persistentDataPath, "player_data.json");

		LoadGame();
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.K)) {
			SaveGame();
		}
		else if (Input.GetKeyDown(KeyCode.L)) {
			LoadGame();
		}
	}

	public void LoadGame() {
		if (File.Exists(savePath)) {
			// Read JSON data from file
			string jsonData = File.ReadAllText(savePath);

			// Deserialize JSON data into player data object
			GameData gameData = JsonUtility.FromJson<GameData>(jsonData);

			// Set coin data and item list
			playerData = gameData.playerData; 
			
			Debug.Log("Game has loaded.");
		}
		else {
			Debug.LogWarning("No saved game data found. \n Saving the game!");
			SaveGame();
		}
	}

	public void SaveGame() {
		GameData gameData = new GameData(); // Create a GameData to hold Datas 

		gameData.playerData = playerData;

		string jsonData = JsonUtility.ToJson(gameData); // Convert the GameData to JSON

		File.WriteAllText(savePath, jsonData); // Save to  the path
		
		Debug.Log("Game has saved.");

	}

	[System.Serializable]
	public class GameData {
		public PlayerData playerData;
	}
}
