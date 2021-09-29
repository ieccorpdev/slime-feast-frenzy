#if UNITY_EDITOR
using UnityEditor;

namespace IEC
{
	public static class EditorShortcuts
	{
		[MenuItem("IEC/Shortcuts/Game Play|Stop _F5", priority = 10)]
		private static void PlayStop()
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
	}
}
#endif