using System.IO;
using UnityEngine;

namespace InventorySystem.Extensions
{
	public class PersistentMonoBehaviour : MonoBehaviour
	{
		private const string SAVE_DIRECTORY = "PlayerData";
		private static string SavePath => $"{Application.dataPath}/{SAVE_DIRECTORY}";
		private string SaveFileName => $"{SavePath}/{GetType().Name}.json";
		protected virtual void OnEnable()
		{
			Load();
		}
		protected virtual void OnDisable()
		{
			Save();
		}
		private void Load()
		{
			var saveFileName = SaveFileName;
			if (!File.Exists(saveFileName)) return; 
			JsonUtility.FromJsonOverwrite(File.ReadAllText(saveFileName), this);
		}
		private void Save()
		{
			var savePath = SavePath;
			if (!Directory.Exists(savePath))
				Directory.CreateDirectory(savePath);
			File.WriteAllText(SaveFileName, JsonUtility.ToJson(this, true));
		}
	}
}