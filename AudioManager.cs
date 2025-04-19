using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000009 RID: 9
public class AudioManager : MonoBehaviour
{
	// Token: 0x0600001B RID: 27 RVA: 0x00002721 File Offset: 0x00000921
	private void Awake()
	{
		AudioManager.instance = this;
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00002729 File Offset: 0x00000929
	private void Start()
	{
		this.UpdateAll();
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00002734 File Offset: 0x00000934
	private void Update()
	{
		this.MasterMixer.SetFloat("Volume", Mathf.Lerp(-80f, 0f, this.VolumeCurve.Evaluate((float)DataDirector.instance.SettingValueFetch(DataDirector.Setting.MasterVolume) * 0.01f)));
		this.MusicMasterGroup.audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-80f, 0f, this.VolumeCurve.Evaluate((float)DataDirector.instance.SettingValueFetch(DataDirector.Setting.MusicVolume) * 0.01f)));
		float value = Mathf.Lerp(-80f, 0f, this.VolumeCurve.Evaluate((float)DataDirector.instance.SettingValueFetch(DataDirector.Setting.SfxVolume) * 0.01f));
		this.SoundMasterGroup.audioMixer.SetFloat("SoundVolume", value);
		this.PersistentSoundGroup.audioMixer.SetFloat("PersistentVolume", value);
		float value2 = Mathf.Lerp(-80f, 0f, this.VolumeCurve.Evaluate((float)DataDirector.instance.SettingValueFetch(DataDirector.Setting.ProximityVoice) * 0.01f));
		this.MicrophoneSoundGroup.audioMixer.SetFloat("MicrophoneVolume", value2);
		this.MicrophoneSpectateGroup.audioMixer.SetFloat("MicrophoneVolume", value2);
		float value3 = Mathf.Lerp(-80f, 0f, this.VolumeCurve.Evaluate((float)DataDirector.instance.SettingValueFetch(DataDirector.Setting.TextToSpeechVolume) * 0.01f));
		this.TTSSoundGroup.audioMixer.SetFloat("TTSVolume", value3);
		this.TTSSpectateGroup.audioMixer.SetFloat("TTSVolume", value3);
	}

	// Token: 0x0600001E RID: 30 RVA: 0x000028D2 File Offset: 0x00000AD2
	public void UpdateAll()
	{
		this.UpdatePushToTalk();
	}

	// Token: 0x0600001F RID: 31 RVA: 0x000028DC File Offset: 0x00000ADC
	public void SetSoundSnapshot(AudioManager.SoundSnapshot _snapShot, float _transitionTime)
	{
		if (_snapShot == this.currentSnapshot)
		{
			return;
		}
		this.currentSnapshot = _snapShot;
		switch (_snapShot)
		{
		case AudioManager.SoundSnapshot.Off:
			this.volumeOff.TransitionTo(_transitionTime);
			return;
		case AudioManager.SoundSnapshot.On:
			this.volumeOn.TransitionTo(_transitionTime);
			return;
		case AudioManager.SoundSnapshot.Spectate:
			this.volumeSpectate.TransitionTo(_transitionTime);
			return;
		case AudioManager.SoundSnapshot.CutsceneOnly:
			this.volumeCutsceneOnly.TransitionTo(_transitionTime);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00002944 File Offset: 0x00000B44
	public void RestartAudioLoopDistances()
	{
		foreach (AudioLoopDistance audioLoopDistance in this.audioLoopDistances)
		{
			if (audioLoopDistance.isActiveAndEnabled)
			{
				audioLoopDistance.Restart();
			}
		}
	}

	// Token: 0x06000021 RID: 33 RVA: 0x000029A0 File Offset: 0x00000BA0
	public void UpdatePushToTalk()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.PushToTalk) == 1)
		{
			this.pushToTalk = true;
			return;
		}
		this.pushToTalk = false;
	}

	// Token: 0x04000020 RID: 32
	public static AudioManager instance;

	// Token: 0x04000021 RID: 33
	public Transform SoundsParent;

	// Token: 0x04000022 RID: 34
	public Transform MusicParent;

	// Token: 0x04000023 RID: 35
	[Space]
	public AudioMixer MasterMixer;

	// Token: 0x04000024 RID: 36
	public AudioMixerGroup PersistentSoundGroup;

	// Token: 0x04000025 RID: 37
	public AudioMixerGroup SoundMasterGroup;

	// Token: 0x04000026 RID: 38
	public AudioMixerGroup MusicMasterGroup;

	// Token: 0x04000027 RID: 39
	public AudioMixerGroup MicrophoneSoundGroup;

	// Token: 0x04000028 RID: 40
	public AudioMixerGroup MicrophoneSpectateGroup;

	// Token: 0x04000029 RID: 41
	public AudioMixerGroup TTSSoundGroup;

	// Token: 0x0400002A RID: 42
	public AudioMixerGroup TTSSpectateGroup;

	// Token: 0x0400002B RID: 43
	public AnimationCurve VolumeCurve;

	// Token: 0x0400002C RID: 44
	[Space]
	public AudioListenerFollow AudioListener;

	// Token: 0x0400002D RID: 45
	[Space]
	public float lowpassValueMin = 1000f;

	// Token: 0x0400002E RID: 46
	public float lowpassValueMax = 22000f;

	// Token: 0x0400002F RID: 47
	public GameObject AudioDefault;

	// Token: 0x04000030 RID: 48
	public GameObject AudioHighFalloff;

	// Token: 0x04000031 RID: 49
	public GameObject AudioFootstep;

	// Token: 0x04000032 RID: 50
	public GameObject AudioMaterialImpact;

	// Token: 0x04000033 RID: 51
	public GameObject AudioCutscene;

	// Token: 0x04000034 RID: 52
	public GameObject AudioAmbienceBreaker;

	// Token: 0x04000035 RID: 53
	public GameObject AudioMaterialSlidingLoop;

	// Token: 0x04000036 RID: 54
	public GameObject AudioLowFalloff;

	// Token: 0x04000037 RID: 55
	public GameObject AudioGlobal;

	// Token: 0x04000038 RID: 56
	public GameObject AudioHigherFalloff;

	// Token: 0x04000039 RID: 57
	public GameObject AudioAttack;

	// Token: 0x0400003A RID: 58
	public GameObject AudioPersistent;

	// Token: 0x0400003B RID: 59
	public AudioMixerSnapshot volumeOff;

	// Token: 0x0400003C RID: 60
	public AudioMixerSnapshot volumeOn;

	// Token: 0x0400003D RID: 61
	public AudioMixerSnapshot volumeSpectate;

	// Token: 0x0400003E RID: 62
	public AudioMixerSnapshot volumeCutsceneOnly;

	// Token: 0x0400003F RID: 63
	private AudioManager.SoundSnapshot currentSnapshot;

	// Token: 0x04000040 RID: 64
	internal List<AudioLoopDistance> audioLoopDistances = new List<AudioLoopDistance>();

	// Token: 0x04000041 RID: 65
	internal bool pushToTalk;

	// Token: 0x020002BA RID: 698
	public enum AudioType
	{
		// Token: 0x04002379 RID: 9081
		Default,
		// Token: 0x0400237A RID: 9082
		HighFalloff,
		// Token: 0x0400237B RID: 9083
		Footstep,
		// Token: 0x0400237C RID: 9084
		MaterialImpact,
		// Token: 0x0400237D RID: 9085
		Cutscene,
		// Token: 0x0400237E RID: 9086
		AmbienceBreaker,
		// Token: 0x0400237F RID: 9087
		LowFalloff,
		// Token: 0x04002380 RID: 9088
		Global,
		// Token: 0x04002381 RID: 9089
		HigherFalloff,
		// Token: 0x04002382 RID: 9090
		Attack,
		// Token: 0x04002383 RID: 9091
		Persistent
	}

	// Token: 0x020002BB RID: 699
	public enum SoundSnapshot
	{
		// Token: 0x04002385 RID: 9093
		Off,
		// Token: 0x04002386 RID: 9094
		On,
		// Token: 0x04002387 RID: 9095
		Spectate,
		// Token: 0x04002388 RID: 9096
		CutsceneOnly
	}
}
