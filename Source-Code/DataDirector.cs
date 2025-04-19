using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000217 RID: 535
public class DataDirector : MonoBehaviour
{
	// Token: 0x0600114D RID: 4429 RVA: 0x0009A427 File Offset: 0x00098627
	private void Awake()
	{
		if (DataDirector.instance == null)
		{
			DataDirector.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			this.InitializeSettings();
			this.LoadSettings();
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600114E RID: 4430 RVA: 0x0009A460 File Offset: 0x00098660
	private void InitializeSettings()
	{
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.MasterVolume, "Master Volume", 75);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.MusicVolume, "Music Volume", 75);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.SfxVolume, "Sfx Volume", 75);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.ProximityVoice, "Proximity Voice Volume", 75);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.TextToSpeechVolume, "Text to Speech Volume", 75);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.MicDevice, "Microphone", 1);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.MicVolume, "Microphone Volume", 100);
		this.SettingAdd(DataDirector.SettingType.Audio, DataDirector.Setting.PushToTalk, "Push to Talk", 0);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.Resolution, "Resolution", 0);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.Fullscreen, "Fullscreen", 0);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.LightDistance, "Light Distance", 3);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.Vsync, "Vsync", 0);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.Bloom, "Bloom", 1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.ChromaticAberration, "Chromatic Aberration", 1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.Grain, "Grain", 1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.MotionBlur, "Motion Blur", 1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.LensEffect, "Lens Effect", 1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.GlitchLoop, "Glitch Loop", 1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.MaxFPS, "Max FPS", -1);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.ShadowQuality, "Shadow Quality", 2);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.ShadowDistance, "Shadow Distance", 3);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.WindowMode, "Window Mode", 0);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.RenderSize, "Pixelation", 2);
		this.SettingAdd(DataDirector.SettingType.Graphics, DataDirector.Setting.Gamma, "Gamma", 40);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.Tips, "Tips", 1);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.AimSensitivity, "Aim Sensitivity", 35);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.CameraSmoothing, "Camera Smoothing", 80);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.CameraShake, "Camera Shake", 100);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.CameraNoise, "Camera Noise", 100);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.CameraAnimation, "Camera Animation", 4);
		this.SettingAdd(DataDirector.SettingType.Gameplay, DataDirector.Setting.PlayerNames, "Player Names", 1);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.RunsPlayed, "Runs Played", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialPlayed, "Tutorial Played", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialJumping, "Tutorial Jumping", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialSprinting, "Tutorial Sprinting", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialSneaking, "Tutorial Sneaking", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialHiding, "Tutorial Hiding", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialTumbling, "Tutorial Tumbling", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialPushingAndPulling, "Tutorial Pushing and Pulling", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialRotating, "Tutorial Rotating", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialReviving, "Tutorial Reviving", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialHealing, "Tutorial Healing", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialCartHandling, "Tutorial Cart Handling", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialItemToggling, "Tutorial Item Toggling", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialInventoryFill, "Tutorial Inventory Fill", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialMap, "Tutorial Map", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialChargingStation, "Tutorial Charging Station", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialOnlyOneExtraction, "Tutorial Only One Extraction", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialChat, "Tutorial Chat", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialFinalExtraction, "Tutorial Final Extraction", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialMultipleExtractions, "Tutorial Multiple Extractions", 0);
		this.SettingAdd(DataDirector.SettingType.None, DataDirector.Setting.TutorialShop, "Tutorial Shop", 0);
	}

	// Token: 0x0600114F RID: 4431 RVA: 0x0009A77C File Offset: 0x0009897C
	private void SettingAdd(DataDirector.SettingType settingType, DataDirector.Setting setting, string _name, int value)
	{
		if (this.settings.ContainsKey(settingType))
		{
			this.settings[settingType].Add(setting);
		}
		else
		{
			this.settings[settingType] = new List<DataDirector.Setting>
			{
				setting
			};
		}
		if (this.settingsName.ContainsKey(setting))
		{
			Debug.LogError("Setting already exists: " + setting.ToString() + " " + _name);
			return;
		}
		this.settingsName[setting] = _name;
		this.settingsValue[setting] = value;
		this.defaultSettingsValue[setting] = value;
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x0009A81D File Offset: 0x00098A1D
	public int SettingValueFetch(DataDirector.Setting setting)
	{
		if (!this.settingsValue.ContainsKey(setting))
		{
			return 0;
		}
		return this.settingsValue[setting];
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x0009A83B File Offset: 0x00098A3B
	public float SettingValueFetchFloat(DataDirector.Setting setting)
	{
		return (float)this.settingsValue[setting] / 100f;
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x0009A850 File Offset: 0x00098A50
	public void SettingValueSet(DataDirector.Setting setting, int value)
	{
		if (this.settingsValue.ContainsKey(setting))
		{
			this.settingsValue[setting] = value;
			return;
		}
		Debug.LogWarning("Setting not found: " + setting.ToString());
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x0009A88A File Offset: 0x00098A8A
	public string SettingNameGet(DataDirector.Setting setting)
	{
		if (!this.settingsName.ContainsKey(setting))
		{
			return null;
		}
		return this.settingsName[setting];
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x0009A8A8 File Offset: 0x00098AA8
	public void SaveSettings()
	{
		SettingsSaveData settingsSaveData = new SettingsSaveData();
		settingsSaveData.settingsValue = new Dictionary<string, int>();
		foreach (KeyValuePair<DataDirector.Setting, int> keyValuePair in this.settingsValue)
		{
			settingsSaveData.settingsValue[keyValuePair.Key.ToString()] = keyValuePair.Value;
		}
		ES3Settings es3Settings = new ES3Settings("SettingsData.es3", new Enum[]
		{
			ES3.Location.File
		});
		ES3.Save<SettingsSaveData>("Settings", settingsSaveData, es3Settings);
		ES3.Save<string>("PlayerBodyColor", this.playerBodyColor, es3Settings);
		ES3.Save<string>("micDevice", this.micDevice, es3Settings);
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x0009A978 File Offset: 0x00098B78
	public void ColorSetBody(int colorID)
	{
		string text = colorID.ToString();
		this.playerBodyColor = text;
		ES3Settings es3Settings = new ES3Settings("SettingsData.es3", new Enum[]
		{
			ES3.Location.File
		});
		ES3.Save<string>("PlayerBodyColor", this.playerBodyColor, es3Settings);
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x0009A9C0 File Offset: 0x00098BC0
	public int ColorGetBody()
	{
		ES3Settings es3Settings = new ES3Settings("SettingsData.es3", new Enum[]
		{
			ES3.Location.File
		});
		if (ES3.KeyExists("PlayerBodyColor", es3Settings))
		{
			this.playerBodyColor = ES3.Load<string>("PlayerBodyColor", es3Settings);
		}
		return int.Parse(this.playerBodyColor);
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x0009AA10 File Offset: 0x00098C10
	public void LoadSettings()
	{
		try
		{
			ES3Settings es3Settings = new ES3Settings("SettingsData.es3", new Enum[]
			{
				ES3.Location.File
			});
			if (ES3.FileExists(es3Settings))
			{
				if (ES3.KeyExists("Settings", es3Settings))
				{
					using (Dictionary<string, int>.Enumerator enumerator = ES3.Load<SettingsSaveData>("Settings", es3Settings).settingsValue.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, int> keyValuePair = enumerator.Current;
							DataDirector.Setting key;
							if (Enum.TryParse<DataDirector.Setting>(keyValuePair.Key, out key) && this.settingsValue.ContainsKey(key))
							{
								this.settingsValue[key] = keyValuePair.Value;
							}
						}
						goto IL_B1;
					}
				}
				Debug.LogWarning("Key 'Settings' not found in file: " + es3Settings.FullPath);
				IL_B1:
				if (ES3.KeyExists("PlayerBodyColor", es3Settings))
				{
					this.playerBodyColor = ES3.Load<string>("PlayerBodyColor", es3Settings);
				}
				if (ES3.KeyExists("micDevice", es3Settings))
				{
					this.micDevice = ES3.Load<string>("micDevice", es3Settings);
				}
			}
			else
			{
				this.SaveSettings();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to load settings: " + ex.Message);
			ES3.DeleteFile("SettingsData.es3");
			this.SaveSettings();
		}
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x0009AB5C File Offset: 0x00098D5C
	public void ResetSettingToDefault(DataDirector.Setting setting)
	{
		if (this.defaultSettingsValue.ContainsKey(setting))
		{
			this.settingsValue[setting] = this.defaultSettingsValue[setting];
			return;
		}
		Debug.LogWarning("Default value not found for setting: " + setting.ToString());
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x0009ABAC File Offset: 0x00098DAC
	public void ResetSettingTypeToDefault(DataDirector.SettingType settingType)
	{
		if (this.settings.ContainsKey(settingType))
		{
			using (List<DataDirector.Setting>.Enumerator enumerator = this.settings[settingType].GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DataDirector.Setting key = enumerator.Current;
					if (this.defaultSettingsValue.ContainsKey(key))
					{
						this.settingsValue[key] = this.defaultSettingsValue[key];
					}
				}
				return;
			}
		}
		Debug.LogWarning("SettingType not found: " + settingType.ToString());
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x0009AC50 File Offset: 0x00098E50
	public void RunsPlayedAdd()
	{
		int value = this.SettingValueFetch(DataDirector.Setting.RunsPlayed) + 1;
		this.SettingValueSet(DataDirector.Setting.RunsPlayed, value);
		this.SaveSettings();
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x0009AC77 File Offset: 0x00098E77
	public void TutorialPlayed()
	{
		this.SettingValueSet(DataDirector.Setting.TutorialPlayed, 1);
		this.SaveSettings();
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x0009AC88 File Offset: 0x00098E88
	public void SaveDeleteCheck(bool _leaveGame)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer() || RunManager.instance.levelPrevious == RunManager.instance.levelTutorial || RunManager.instance.levelPrevious == RunManager.instance.levelLobbyMenu || RunManager.instance.levelPrevious == RunManager.instance.levelMainMenu || RunManager.instance.levelPrevious == RunManager.instance.levelRecording)
		{
			return;
		}
		bool flag = false;
		if (SemiFunc.RunIsArena())
		{
			flag = true;
		}
		else if (RunManager.instance.allPlayersDead && RunManager.instance.levelPrevious != RunManager.instance.levelMainMenu && RunManager.instance.levelPrevious != RunManager.instance.levelLobbyMenu && RunManager.instance.levelPrevious != RunManager.instance.levelTutorial && RunManager.instance.levelPrevious != RunManager.instance.levelLobby && RunManager.instance.levelPrevious != RunManager.instance.levelShop && RunManager.instance.levelPrevious != RunManager.instance.levelRecording)
		{
			flag = true;
		}
		else if (_leaveGame && RunManager.instance.levelsCompleted == 0)
		{
			flag = true;
		}
		if (flag)
		{
			SemiFunc.SaveFileDelete(StatsManager.instance.saveFileCurrent);
		}
	}

	// Token: 0x04001D22 RID: 7458
	public static DataDirector instance;

	// Token: 0x04001D23 RID: 7459
	private string playerBodyColor = "0";

	// Token: 0x04001D24 RID: 7460
	internal string micDevice = "";

	// Token: 0x04001D25 RID: 7461
	private Dictionary<DataDirector.Setting, string> settingsName = new Dictionary<DataDirector.Setting, string>();

	// Token: 0x04001D26 RID: 7462
	private Dictionary<DataDirector.Setting, int> settingsValue = new Dictionary<DataDirector.Setting, int>();

	// Token: 0x04001D27 RID: 7463
	private Dictionary<DataDirector.Setting, int> defaultSettingsValue = new Dictionary<DataDirector.Setting, int>();

	// Token: 0x04001D28 RID: 7464
	private Dictionary<DataDirector.SettingType, List<DataDirector.Setting>> settings = new Dictionary<DataDirector.SettingType, List<DataDirector.Setting>>();

	// Token: 0x020003A3 RID: 931
	public enum Setting
	{
		// Token: 0x04002830 RID: 10288
		MusicVolume,
		// Token: 0x04002831 RID: 10289
		SfxVolume,
		// Token: 0x04002832 RID: 10290
		AmbienceVolume,
		// Token: 0x04002833 RID: 10291
		MicDevice,
		// Token: 0x04002834 RID: 10292
		ProximityVoice,
		// Token: 0x04002835 RID: 10293
		Resolution,
		// Token: 0x04002836 RID: 10294
		Fullscreen,
		// Token: 0x04002837 RID: 10295
		MicVolume,
		// Token: 0x04002838 RID: 10296
		TextToSpeechVolume,
		// Token: 0x04002839 RID: 10297
		CameraShake,
		// Token: 0x0400283A RID: 10298
		CameraAnimation,
		// Token: 0x0400283B RID: 10299
		Tips,
		// Token: 0x0400283C RID: 10300
		Vsync,
		// Token: 0x0400283D RID: 10301
		MasterVolume,
		// Token: 0x0400283E RID: 10302
		CameraSmoothing,
		// Token: 0x0400283F RID: 10303
		LightDistance,
		// Token: 0x04002840 RID: 10304
		Bloom,
		// Token: 0x04002841 RID: 10305
		LensEffect,
		// Token: 0x04002842 RID: 10306
		MotionBlur,
		// Token: 0x04002843 RID: 10307
		MaxFPS,
		// Token: 0x04002844 RID: 10308
		ShadowQuality,
		// Token: 0x04002845 RID: 10309
		ShadowDistance,
		// Token: 0x04002846 RID: 10310
		ChromaticAberration,
		// Token: 0x04002847 RID: 10311
		Grain,
		// Token: 0x04002848 RID: 10312
		WindowMode,
		// Token: 0x04002849 RID: 10313
		RenderSize,
		// Token: 0x0400284A RID: 10314
		GlitchLoop,
		// Token: 0x0400284B RID: 10315
		AimSensitivity,
		// Token: 0x0400284C RID: 10316
		CameraNoise,
		// Token: 0x0400284D RID: 10317
		Gamma,
		// Token: 0x0400284E RID: 10318
		PlayerNames,
		// Token: 0x0400284F RID: 10319
		RunsPlayed,
		// Token: 0x04002850 RID: 10320
		PushToTalk,
		// Token: 0x04002851 RID: 10321
		TutorialPlayed,
		// Token: 0x04002852 RID: 10322
		TutorialJumping,
		// Token: 0x04002853 RID: 10323
		TutorialSprinting,
		// Token: 0x04002854 RID: 10324
		TutorialSneaking,
		// Token: 0x04002855 RID: 10325
		TutorialHiding,
		// Token: 0x04002856 RID: 10326
		TutorialTumbling,
		// Token: 0x04002857 RID: 10327
		TutorialPushingAndPulling,
		// Token: 0x04002858 RID: 10328
		TutorialRotating,
		// Token: 0x04002859 RID: 10329
		TutorialReviving,
		// Token: 0x0400285A RID: 10330
		TutorialHealing,
		// Token: 0x0400285B RID: 10331
		TutorialCartHandling,
		// Token: 0x0400285C RID: 10332
		TutorialItemToggling,
		// Token: 0x0400285D RID: 10333
		TutorialInventoryFill,
		// Token: 0x0400285E RID: 10334
		TutorialMap,
		// Token: 0x0400285F RID: 10335
		TutorialChargingStation,
		// Token: 0x04002860 RID: 10336
		TutorialOnlyOneExtraction,
		// Token: 0x04002861 RID: 10337
		TutorialChat,
		// Token: 0x04002862 RID: 10338
		TutorialFinalExtraction,
		// Token: 0x04002863 RID: 10339
		TutorialMultipleExtractions,
		// Token: 0x04002864 RID: 10340
		TutorialShop
	}

	// Token: 0x020003A4 RID: 932
	public enum SettingType
	{
		// Token: 0x04002866 RID: 10342
		Audio,
		// Token: 0x04002867 RID: 10343
		Gameplay,
		// Token: 0x04002868 RID: 10344
		Graphics,
		// Token: 0x04002869 RID: 10345
		None
	}
}
