#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace IEC
{
	/// <summary>
	/// https://docs.unity3d.com/ScriptReference/MenuItem.html?_ga=2.149104054.1289335750.1640072924-1622336907.1637639321
	/// </summary>
	public static class EditorShortcuts
	{
		[MenuItem("IEC/Shortcuts/Game Start|Stop _F5", priority = 10)]
		private static void StartStop()
		{
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
				return;
			}

			EditorApplication.EnterPlaymode();
		}

		[MenuItem("IEC/Shortcuts/Game Pause|Resume _F6", priority = 10)]
		private static void PauseResume()
		{
			EditorApplication.isPaused = !EditorApplication.isPaused;
		}

		[MenuItem("IEC/Shortcuts/Game Next Frame _F7", priority = 10)]
		private static void NextFrame()
		{
			EditorApplication.Step();
		}

		[MenuItem("IEC/Shortcuts/Game Fresh Start _F8", priority = 10)]
		private static void FreshStart()
		{
			PlayerPrefs.DeleteAll();
			EditorApplication.EnterPlaymode();
		}
	}
}
#endif