using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

// Token: 0x020001C3 RID: 451
[Serializable]
public class PlayerVoiceChat : MonoBehaviour
{
	// Token: 0x06000F36 RID: 3894 RVA: 0x0008AAFF File Offset: 0x00088CFF
	private void Awake()
	{
		RunManager.instance.voiceChats.Add(this);
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x0008AB14 File Offset: 0x00088D14
	private void Start()
	{
		this.clipSampleData = new float[this.sampleDataLength];
		this.audioSource = base.GetComponent<AudioSource>();
		this.photonView = base.GetComponent<PhotonView>();
		this.recorder = base.GetComponent<Recorder>();
		this.speaker = base.GetComponent<Speaker>();
		if (this.photonView.IsMine)
		{
			if (PlayerVoiceChat.instance)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			PlayerVoiceChat.instance = this;
			this.audioSource.volume = 0f;
			this.voiceGain = 0f;
		}
		Object.DontDestroyOnLoad(base.gameObject);
		this.ToggleLobby(true);
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x0008ABBA File Offset: 0x00088DBA
	public void OverrideClipLoudnessAnimationValue(float _value)
	{
		this.overrideAddToClipLoudness = _value;
		this.overrideAddToClipLoudnessTimer = 0.1f;
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x0008ABCE File Offset: 0x00088DCE
	private void OverrideClipLoudnessAnimationValueTick()
	{
		if (this.overrideAddToClipLoudnessTimer > 0f)
		{
			this.overrideAddToClipLoudnessTimer -= Time.deltaTime;
			return;
		}
		this.overrideAddToClipLoudness = 0f;
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x0008ABFB File Offset: 0x00088DFB
	private void FixedUpdate()
	{
		this.OverridePitchTick();
		this.OverrideClipLoudnessAnimationValueTick();
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x0008AC0C File Offset: 0x00088E0C
	private void Update()
	{
		this.OverridePitchLogic();
		if (this.photonView.IsMine)
		{
			this.microphoneVolumeSetting = DataDirector.instance.SettingValueFetch(DataDirector.Setting.MicVolume);
			if (this.microphoneVolumeSetting != this.microphoneVolumeSettingPrevious)
			{
				this.microphoneVolumeSettingPrevious = this.microphoneVolumeSetting;
				this.photonView.RPC("MicrophoneVolumeSettingRPC", RpcTarget.OthersBuffered, new object[]
				{
					this.microphoneVolumeSetting
				});
			}
		}
		this.microphoneVolumeMultiplier = (float)this.microphoneVolumeSetting * 0.01f;
		if (!this.TTSinstantiated && this.playerAvatar)
		{
			if (this.TTSinstantiatedTimer > 3f && (PunVoiceClient.Instance.Client.State == ClientState.Joined || PunVoiceClient.Instance.Client.State == ClientState.Disconnected))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.TTSprefab, base.transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				this.ttsVoice = gameObject.GetComponent<TTSVoice>();
				this.ttsAudioSource = this.ttsVoice.GetComponent<AudioSource>();
				this.lowPassLogicTTS = this.ttsAudioSource.GetComponent<AudioLowPassLogic>();
				this.lowPassLogicTTS.Fetch = true;
				this.ttsVoice.playerAvatar = this.playerAvatar;
				if (this.playerAvatar.isLocal)
				{
					this.recorder.RecordingEnabled = true;
				}
				this.photonView.RPC("RecordingEnabled", RpcTarget.AllBuffered, Array.Empty<object>());
				this.TTSinstantiated = true;
			}
			else
			{
				this.TTSinstantiatedTimer += Time.deltaTime;
			}
		}
		if (this.TTSinstantiated && this.playerAvatar.isLocal)
		{
			this.microphoneEnabledPrevious = this.microphoneEnabled;
			this.microphoneEnabled = false;
			if (this.currentDeviceName != "NONE")
			{
				string[] devices = Microphone.devices;
				for (int i = 0; i < devices.Length; i++)
				{
					if (devices[i] == this.currentDeviceName)
					{
						this.microphoneEnabled = true;
						break;
					}
				}
			}
			if (this.currentDeviceName == "" || this.currentDeviceName != SessionManager.instance.micDeviceCurrent || this.microphoneEnabled != this.microphoneEnabledPrevious)
			{
				this.currentDeviceName = SessionManager.instance.micDeviceCurrent;
				if (!this.microphoneEnabled && this.currentDeviceName != "")
				{
					this.recorder.MicrophoneDevice = new DeviceInfo("", null);
				}
				else if (this.currentDeviceName != "NONE")
				{
					this.recorder.MicrophoneDevice = new DeviceInfo(this.currentDeviceName, null);
				}
			}
		}
		if (this.clipCheckTimer <= 0f)
		{
			this.clipCheckTimer = 0.001f;
			this.clipLoudness = 0f;
			this.clipLoudnessTTS = 0f;
			this.clipLoudnessNoTTS = 0f;
			if (this.audioSource && this.audioSource.clip && this.audioSource.isPlaying)
			{
				this.audioSource.clip.GetData(this.clipSampleData, this.audioSource.timeSamples);
				foreach (float f in this.clipSampleData)
				{
					this.clipLoudness += Mathf.Abs(f);
				}
				this.clipLoudness /= (float)this.sampleDataLength;
				this.clipLoudnessNoTTS = this.clipLoudness;
			}
			this.clipLoudness *= this.microphoneVolumeMultiplier;
			this.clipLoudnessNoTTS *= this.microphoneVolumeMultiplier;
			this.clipLoudness += this.overrideAddToClipLoudness;
			if (this.ttsVoice && this.ttsAudioSource.isPlaying)
			{
				this.ttsAudioSource.GetSpectrumData(this.ttsAudioSpectrum, 0, FFTWindow.BlackmanHarris);
				float num = Mathf.Max(this.ttsAudioSpectrum) * 2f;
				this.clipLoudnessTTS = num;
				if (num > this.clipLoudness)
				{
					this.clipLoudness = num;
				}
			}
		}
		else
		{
			this.clipCheckTimer -= Time.deltaTime;
		}
		if (this.photonView.IsMine)
		{
			if (!this.debug)
			{
				if (this.clipLoudness > 0.005f)
				{
					this.isTalking = true;
					this.isTalkingTimer = 0.5f;
				}
			}
			else if (this.debugTalkingTimer > 0f)
			{
				this.debugTalkingTimer -= Time.deltaTime;
				this.isTalkingTimer = 1f;
				this.isTalking = true;
				if (this.debugTalkingTimer <= 0f)
				{
					this.debugTalkingCooldown = Random.Range(3f, 10f);
				}
			}
			else
			{
				this.debugTalkingCooldown -= Time.deltaTime;
				if (this.debugTalkingCooldown <= 0f)
				{
					this.debugTalkingTimer = Random.Range(1f, 6f);
				}
			}
			if (this.isTalkingTimer > 0f)
			{
				this.isTalkingTimer -= Time.deltaTime;
				if (this.isTalkingTimer <= 0f)
				{
					this.isTalking = false;
				}
			}
			if (this.isTalking != this.isTalkingPrevious)
			{
				this.isTalkingPrevious = this.isTalking;
				if (this.isTalking)
				{
					this.isTalkingStartTime = Time.time;
				}
				this.photonView.RPC("IsTalkingRPC", RpcTarget.Others, new object[]
				{
					this.isTalking
				});
			}
		}
		if (this.debug)
		{
			if (this.isTalking)
			{
				this.lowPassLogic.Volume = Mathf.Lerp(this.lowPassLogic.Volume, 1f, Time.deltaTime * 20f);
				if (this.lowPassLogicTTS)
				{
					this.lowPassLogicTTS.Volume = this.lowPassLogic.Volume;
				}
			}
			else
			{
				this.lowPassLogic.Volume = Mathf.Lerp(this.lowPassLogic.Volume, 0f, Time.deltaTime * 20f);
				if (this.lowPassLogicTTS)
				{
					this.lowPassLogicTTS.Volume = this.lowPassLogic.Volume;
				}
			}
		}
		if (SemiFunc.IsMultiplayer() && SemiFunc.IsMasterClient())
		{
			if (this.playerAvatar && !this.playerAvatar.isDisabled)
			{
				bool flag = false;
				if (this.playerAvatar.isCrawling)
				{
					if (this.clipLoudness > 0.15f)
					{
						flag = true;
					}
				}
				else if (this.playerAvatar.isCrouching)
				{
					if (this.clipLoudness > 0.05f)
					{
						flag = true;
					}
				}
				else if (this.clipLoudness > 0.025f)
				{
					flag = true;
				}
				if (flag && this.investigateTimer <= 0f)
				{
					this.investigateTimer = 1f;
					EnemyDirector.instance.SetInvestigate(this.playerAvatar.PlayerVisionTarget.VisionTransform.transform.position, 5f);
				}
			}
			if (this.investigateTimer >= 0f)
			{
				this.investigateTimer -= Time.deltaTime;
			}
		}
		if (this.SpatialDisableTimer > 0f || this.inLobbyMixer || this.photonView.IsMine)
		{
			this.audioSource.spatialBlend = 0f;
			this.SpatialDisableTimer -= Time.deltaTime;
		}
		else
		{
			this.audioSource.spatialBlend = 1f;
		}
		float volume = 0f;
		if (!this.photonView.IsMine)
		{
			volume = this.voiceGain * this.microphoneVolumeMultiplier;
		}
		this.lowPassLogic.Volume = volume;
		if (this.lowPassLogicTTS && this.playerAvatar)
		{
			if (this.playerAvatar.isCrouching)
			{
				this.lowPassLogicTTS.Volume = 0.8f;
			}
			else
			{
				this.lowPassLogicTTS.Volume = 1f;
			}
		}
		if (this.TTSinstantiated && this.playerAvatar.isLocal)
		{
			if (!this.microphoneEnabled)
			{
				this.recorder.TransmitEnabled = false;
				return;
			}
			if (AudioManager.instance.pushToTalk)
			{
				if (SemiFunc.InputHold(InputKey.PushToTalk))
				{
					this.recorder.TransmitEnabled = true;
					return;
				}
				this.recorder.TransmitEnabled = false;
				return;
			}
			else
			{
				this.recorder.TransmitEnabled = true;
			}
		}
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x0008B459 File Offset: 0x00089659
	private void LateUpdate()
	{
		this.TtsFollowVoiceSettings();
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x0008B461 File Offset: 0x00089661
	private void OnDestroy()
	{
		RunManager.instance.voiceChats.Remove(this);
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x0008B474 File Offset: 0x00089674
	private void TtsFollowVoiceSettings()
	{
		if (!this.ttsVoice)
		{
			return;
		}
		if (!this.playerAvatar)
		{
			return;
		}
		if (this.playerAvatar.isCrouching || this.playerAvatar.isCrawling)
		{
			this.ttsVoice.setVoice(1);
		}
		else
		{
			this.ttsVoice.setVoice(0);
		}
		if (SemiFunc.IsMultiplayer())
		{
			Vector3 forward = this.playerAvatar.PlayerVisionTarget.VisionTransform.transform.forward;
			float num = Mathf.Lerp(0.7f, 1.3f, (forward.y + 1f) / 1.5f) + this.TTSPitchChange;
			num *= this.pitchMultiplier;
			if (this.playerAvatar.isDisabled)
			{
				num = 1f;
			}
			this.ttsAudioSource.pitch = this.audioSource.pitch * num;
			this.ttsAudioSource.spatialBlend = this.audioSource.spatialBlend;
		}
		if (this.inLobbyMixer != this.inLobbyMixerTTS)
		{
			this.inLobbyMixerTTS = this.inLobbyMixer;
			if (this.inLobbyMixer)
			{
				this.ttsAudioSource.outputAudioMixerGroup = this.mixerTTSSpectate;
				return;
			}
			this.ttsAudioSource.outputAudioMixerGroup = this.mixerTTSSound;
		}
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x0008B5B0 File Offset: 0x000897B0
	public void SetDebug()
	{
		this.debug = true;
		this.audioSource.Stop();
		this.audioSource.clip = this.debugClip;
		this.audioSource.time = Random.Range(0f, this.debugClip.length);
		this.audioSource.loop = true;
		if (this.photonView.IsMine)
		{
			this.audioSource.pitch = 0.8f * this.pitchMultiplier;
			this.audioSource.volume = 0.3f;
			this.lowPassLogic.Volume = 0.3f;
		}
		else
		{
			this.audioSource.pitch = 1.25f * this.pitchMultiplier;
		}
		this.audioSource.Play();
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x0008B674 File Offset: 0x00089874
	public void ToggleLobby(bool _toggle)
	{
		if (_toggle)
		{
			this.inLobbyMixer = true;
			if (this.photonView.IsMine)
			{
				this.volumeOn.TransitionTo(0.1f);
			}
			base.transform.position = new Vector3(1000f, 1000f, 1000f);
			this.audioSource.outputAudioMixerGroup = this.mixerMicrophoneSpectate;
			return;
		}
		this.inLobbyMixer = false;
		if (this.photonView.IsMine)
		{
			this.volumeOff.TransitionTo(0.1f);
			if (AudioManager.instance)
			{
				AudioManager.instance.SetSoundSnapshot(AudioManager.SoundSnapshot.On, 0.5f);
			}
		}
		this.audioSource.outputAudioMixerGroup = this.mixerMicrophoneSound;
		if (this.ttsAudioSource)
		{
			this.ttsAudioSource.outputAudioMixerGroup = this.mixerTTSSound;
		}
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x0008B748 File Offset: 0x00089948
	[PunRPC]
	public void IsTalkingRPC(bool _isTalking)
	{
		this.isTalking = _isTalking;
		if (this.isTalking)
		{
			this.isTalkingStartTime = Time.time;
		}
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x0008B764 File Offset: 0x00089964
	[PunRPC]
	public void MicrophoneVolumeSettingRPC(int _volume)
	{
		this.microphoneVolumeSetting = _volume;
	}

	// Token: 0x06000F43 RID: 3907 RVA: 0x0008B76D File Offset: 0x0008996D
	[PunRPC]
	public void RecordingEnabled()
	{
		this.recordingEnabled = true;
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x0008B776 File Offset: 0x00089976
	public void SpatialDisable(float _time)
	{
		if (this.photonView.IsMine)
		{
			return;
		}
		this.SpatialDisableTimer = _time;
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x0008B790 File Offset: 0x00089990
	public void OverridePitch(float _multiplier, float _timeIn, float _timeOut, float _overrideTimer = 0.1f, bool doRPC = true)
	{
		float num = this.overridePitchMultiplierTarget;
		this.overridePitchMultiplierTarget = _multiplier;
		this.overridePitchSpeedIn = _timeIn;
		this.overridePitchSpeedOut = _timeOut;
		this.overridePitchTimer = _overrideTimer;
		this.overridePitchTime = _overrideTimer;
		this.overridePitchIsActive = true;
		if (this.overridePitchIsActive && num < 0f && this.overridePitchMultiplierTarget < num)
		{
			this.overridePitchIsActive = false;
		}
		if (this.overridePitchIsActive && num > 0f && this.overridePitchMultiplierTarget > num)
		{
			this.overridePitchIsActive = false;
		}
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x0008B810 File Offset: 0x00089A10
	public void OverridePitchCancel()
	{
		this.overridePitchMultiplierTarget = 1f;
		this.overridePitchSpeedIn = 0.1f;
		this.overridePitchSpeedOut = 0.1f;
		this.overridePitchTimer = 0f;
		this.overridePitchTime = 0f;
		this.overridePitchIsActive = false;
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x0008B850 File Offset: 0x00089A50
	private void OverridePitchTick()
	{
		if (this.overridePitchTimer <= 0f)
		{
			this.overridePitchIsActive = false;
		}
		if (this.overridePitchTimer > 0f)
		{
			this.overridePitchIsActive = true;
			this.overridePitchTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x0008B88C File Offset: 0x00089A8C
	private void OverridePitchLogic()
	{
		this.audioSource.pitch = this.pitchMultiplier;
		if (!this.overridePitchIsActive && this.overridePitchLerp < 0.05f)
		{
			this.pitchMultiplier = 1f;
			return;
		}
		if (this.overridePitchTimer > 0f)
		{
			this.overridePitchLerp += Time.deltaTime / this.overridePitchSpeedIn;
			if (this.overridePitchLerp > 1f)
			{
				this.overridePitchLerp = 1f;
			}
		}
		else
		{
			this.overridePitchLerp -= Time.deltaTime / this.overridePitchSpeedOut;
			if (this.overridePitchLerp < 0f)
			{
				this.overridePitchLerp = 0f;
			}
		}
		this.pitchMultiplier = Mathf.Lerp(1f, this.overridePitchMultiplierTarget, this.overridePitchLerp);
	}

	// Token: 0x04001990 RID: 6544
	public GameObject TTSprefab;

	// Token: 0x04001991 RID: 6545
	public static PlayerVoiceChat instance;

	// Token: 0x04001992 RID: 6546
	[FormerlySerializedAs("textToSpeech")]
	public TTSVoice ttsVoice;

	// Token: 0x04001993 RID: 6547
	public AudioClip debugClip;

	// Token: 0x04001994 RID: 6548
	internal bool debug;

	// Token: 0x04001995 RID: 6549
	private float debugTalkingTimer;

	// Token: 0x04001996 RID: 6550
	private float debugTalkingCooldown;

	// Token: 0x04001997 RID: 6551
	internal bool inLobbyMixer;

	// Token: 0x04001998 RID: 6552
	internal bool inLobbyMixerTTS;

	// Token: 0x04001999 RID: 6553
	internal bool isTalking;

	// Token: 0x0400199A RID: 6554
	private bool isTalkingPrevious;

	// Token: 0x0400199B RID: 6555
	private float isTalkingTimer;

	// Token: 0x0400199C RID: 6556
	internal float isTalkingStartTime;

	// Token: 0x0400199D RID: 6557
	internal float voiceGain = 0.5f;

	// Token: 0x0400199E RID: 6558
	internal PhotonView photonView;

	// Token: 0x0400199F RID: 6559
	internal PlayerAvatar playerAvatar;

	// Token: 0x040019A0 RID: 6560
	internal AudioSource audioSource;

	// Token: 0x040019A1 RID: 6561
	private Recorder recorder;

	// Token: 0x040019A2 RID: 6562
	private Speaker speaker;

	// Token: 0x040019A3 RID: 6563
	[Space]
	public AudioMixerGroup mixerMicrophoneSound;

	// Token: 0x040019A4 RID: 6564
	public AudioMixerGroup mixerMicrophoneSpectate;

	// Token: 0x040019A5 RID: 6565
	public AudioMixerGroup mixerTTSSound;

	// Token: 0x040019A6 RID: 6566
	public AudioMixerGroup mixerTTSSpectate;

	// Token: 0x040019A7 RID: 6567
	[Space]
	public AudioMixerSnapshot volumeOff;

	// Token: 0x040019A8 RID: 6568
	public AudioMixerSnapshot volumeOn;

	// Token: 0x040019A9 RID: 6569
	[Space]
	public AudioLowPassLogic lowPassLogic;

	// Token: 0x040019AA RID: 6570
	public AudioLowPassLogic lowPassLogicTTS;

	// Token: 0x040019AB RID: 6571
	private float SpatialDisableTimer;

	// Token: 0x040019AC RID: 6572
	private int sampleDataLength = 1024;

	// Token: 0x040019AD RID: 6573
	internal float clipLoudnessNoTTS;

	// Token: 0x040019AE RID: 6574
	internal float clipLoudnessTTS;

	// Token: 0x040019AF RID: 6575
	internal float clipLoudness;

	// Token: 0x040019B0 RID: 6576
	private float[] clipSampleData;

	// Token: 0x040019B1 RID: 6577
	private float clipCheckTimer;

	// Token: 0x040019B2 RID: 6578
	private string currentDeviceName = "";

	// Token: 0x040019B3 RID: 6579
	private float investigateTimer;

	// Token: 0x040019B4 RID: 6580
	public AudioSource ttsAudioSource;

	// Token: 0x040019B5 RID: 6581
	private float[] ttsAudioSpectrum = new float[1024];

	// Token: 0x040019B6 RID: 6582
	internal bool TTSinstantiated;

	// Token: 0x040019B7 RID: 6583
	private float TTSinstantiatedTimer;

	// Token: 0x040019B8 RID: 6584
	private float TTSPitchChangeTimer;

	// Token: 0x040019B9 RID: 6585
	private float TTSPitchChangeTarget;

	// Token: 0x040019BA RID: 6586
	private float TTSPitchChange;

	// Token: 0x040019BB RID: 6587
	private float TTSPitchChangeSpeed;

	// Token: 0x040019BC RID: 6588
	private float switchDeviceTimer;

	// Token: 0x040019BD RID: 6589
	private int microphoneVolumeSetting = -1;

	// Token: 0x040019BE RID: 6590
	private int microphoneVolumeSettingPrevious = -1;

	// Token: 0x040019BF RID: 6591
	internal float microphoneVolumeMultiplier = 1f;

	// Token: 0x040019C0 RID: 6592
	private float pitchMultiplier = 1f;

	// Token: 0x040019C1 RID: 6593
	private float overridePitchTimer;

	// Token: 0x040019C2 RID: 6594
	private float overridePitchMultiplierTarget = 1f;

	// Token: 0x040019C3 RID: 6595
	private float overridePitchSpeedIn;

	// Token: 0x040019C4 RID: 6596
	private float overridePitchSpeedOut;

	// Token: 0x040019C5 RID: 6597
	private float overridePitchLerp;

	// Token: 0x040019C6 RID: 6598
	private float overridePitchTime;

	// Token: 0x040019C7 RID: 6599
	private bool overridePitchIsActive;

	// Token: 0x040019C8 RID: 6600
	private float currentBoost;

	// Token: 0x040019C9 RID: 6601
	private float overrideAddToClipLoudnessTimer;

	// Token: 0x040019CA RID: 6602
	private float overrideAddToClipLoudness;

	// Token: 0x040019CB RID: 6603
	internal bool recordingEnabled;

	// Token: 0x040019CC RID: 6604
	internal bool microphoneEnabled;

	// Token: 0x040019CD RID: 6605
	private bool microphoneEnabledPrevious;
}
