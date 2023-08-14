using AudioMob.Internal;
using AudioMob.Internal.Utility;
using UnityEngine;
using AudioMob.Internal.DevicePlugins;

namespace AudioMob.Unmanaged
{
	/// <summary>
	/// Sets up the audio plugin at runtime.
	/// Needs to remain out of a managed package so the RuntimeInitializeOnLoadMethod attribute is registered.
	///
	/// You may be tempted to move initialization into the AudioMobPlugin class; but as that gets compiled into an
	/// assembly for distribution and some of the things you need to initialize are unmanaged code, this will just mean
	/// that your build fails.
	/// </summary>
	public class AudioMobPluginInitialization
	{
		/// <summary>
		/// The mediation adaptor SDK to use in this project.
		/// </summary>
		public static IMediationAdapterSdk MediationAdapterSdk { private get; set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializePluginAutomatically()
		{
			if (AudioMobSettings.Instance.AutoInitializePlugin)
			{
				InitializePlugin();
			}
		}

		/// <summary>
		/// Initialize the AudioMob Plugin.
		/// This should be done early in the game's runtime lifecycle, ideally at launch.
		/// This initialization can be set to happen automatically by checking "Automatically Initialize Plugin" in the Editor.
		/// </summary>
		public static void InitializePlugin()
		{
#if UNITY_EDITOR
			IAudioPlugin audioPlugin = new EditorAudioPlugin();
			IOpenMeasurementSdk openMeasurementSdk = null;
#elif UNITY_IOS
			IAudioPlugin audioPlugin = new IosAudioPlugin();
			IOpenMeasurementSdk openMeasurementSdk = new IosOpenMeasurementSdk();
#elif UNITY_ANDROID
			IAudioPlugin audioPlugin = new AndroidAudioPlugin();
			IOpenMeasurementSdk openMeasurementSdk = new AndroidOpenMeasurementSdk();
#else
			IAudioPlugin audioPlugin = new StandaloneAudioPlugin();
			IOpenMeasurementSdk openMeasurementSdk = null;
#endif

			ILocationService locationService = new AudioMobLocationService();

			audioPlugin.Start(nameof(DeviceAudioCheck));
			AudioMobPluginController.Init(audioPlugin, locationService, openMeasurementSdk, MediationAdapterSdk);
		}
	}
}
