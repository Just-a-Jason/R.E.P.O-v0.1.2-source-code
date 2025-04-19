using System;
using System.Collections.Generic;
using Strobotnik.Klattersynth;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000D2 RID: 210
public class TTSVoice : MonoBehaviour
{
	// Token: 0x0600074E RID: 1870 RVA: 0x000457C3 File Offset: 0x000439C3
	private void Init()
	{
		if (this.voices.Length != 0)
		{
			this.setVoice(0);
			return;
		}
		Debug.LogError("Empty voices array!", this);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x000457ED File Offset: 0x000439ED
	public void TTSSpeakNow(string text, bool crouch)
	{
		this.StartSpeakingWithHighlight(text, crouch);
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x000457F8 File Offset: 0x000439F8
	public void StopAndClearVoice()
	{
		this.voices[0].stop(true);
		this.voices[0].cacheClear();
		this.voices[0].scheduleClear();
		this.voices[1].stop(true);
		this.voices[1].cacheClear();
		this.voices[1].scheduleClear();
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x00045858 File Offset: 0x00043A58
	public void StartSpeakingWithHighlight(string text, bool crouch)
	{
		this.StopAndClearVoice();
		if (crouch)
		{
			this.voicingSource = SpeechSynth.VoicingSource.whisper;
			this.setVoice(1);
		}
		else
		{
			this.voicingSource = SpeechSynth.VoicingSource.natural;
			this.setVoice(0);
		}
		if (!this.activeVoice)
		{
			Debug.LogError("Active voice is not set.");
			return;
		}
		text = this.TranslateSpecialLetters(text);
		this.words = new List<string>(text.Split(' ', StringSplitOptions.None));
		foreach (string text2 in this.words)
		{
			this.activeVoice.schedule(text2, false);
		}
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x00045910 File Offset: 0x00043B10
	public void setVoice(int num)
	{
		if (num >= 0 && num < this.voices.Length)
		{
			this.activeVoice = this.voices[num];
			this.activeVoiceNum = num;
			return;
		}
		Debug.LogWarning("Invalid voice: " + num.ToString(), this);
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x0004594E File Offset: 0x00043B4E
	public void VoiceText(string text, float wordTime)
	{
		if (this.playerAvatar && WorldSpaceUIParent.instance)
		{
			WorldSpaceUIParent.instance.TTS(this.playerAvatar, text, wordTime);
		}
		this.voiceText = text;
		this.currentWordTime = wordTime;
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x00045989 File Offset: 0x00043B89
	private void Start()
	{
		this.Init();
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x000459A0 File Offset: 0x00043BA0
	private string TranslateSpecialLetters(string _text)
	{
		if (_text.Contains("ö") || _text.Contains("Ö"))
		{
			_text.Replace("ö", "oe");
			_text.Replace("Ö", "OE");
		}
		if (_text.Contains("ä") || _text.Contains("Ä"))
		{
			_text.Replace("ä", "ae");
			_text.Replace("Ä", "AE");
		}
		if (_text.Contains("å") || _text.Contains("Å"))
		{
			_text.Replace("å", "oa");
			_text.Replace("Å", "OA");
		}
		if (_text.Contains("ü") || _text.Contains("Ü"))
		{
			_text.Replace("ü", "ue");
			_text.Replace("Ü", "UE");
		}
		if (_text.Contains("ß"))
		{
			_text.Replace("ß", "ss");
		}
		if (_text.Contains("æ") || _text.Contains("Æ"))
		{
			_text.Replace("æ", "ae");
			_text.Replace("Æ", "AE");
		}
		if (_text.Contains("ø") || _text.Contains("Ø"))
		{
			_text.Replace("ø", "oe");
			_text.Replace("Ø", "OE");
		}
		return _text;
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00045B34 File Offset: 0x00043D34
	private void Update()
	{
		if (this.playerAvatar && this.isPlayerAvatarDisabledPrev != this.playerAvatar.isDisabled)
		{
			this.StopAndClearVoice();
			this.isPlayerAvatarDisabledPrev = this.playerAvatar.isDisabled;
		}
		this.isSpeaking = this.audioSource.isPlaying;
		if (this.isSpeaking && this.playerAvatar.voiceChat.clipLoudnessTTS <= 0.01f)
		{
			if (this.noClipLoudnessTimer > 2f)
			{
				this.StopAndClearVoice();
			}
			this.noClipLoudnessTimer += Time.deltaTime;
			return;
		}
		this.noClipLoudnessTimer = 0f;
	}

	// Token: 0x04000CE4 RID: 3300
	internal PlayerAvatar playerAvatar;

	// Token: 0x04000CE5 RID: 3301
	internal bool isPlayerAvatarDisabledPrev;

	// Token: 0x04000CE6 RID: 3302
	public Text baseFreqHzLabel;

	// Token: 0x04000CE7 RID: 3303
	public Speech[] voices;

	// Token: 0x04000CE8 RID: 3304
	internal string voiceText;

	// Token: 0x04000CE9 RID: 3305
	internal int voiceTextWordIndex;

	// Token: 0x04000CEA RID: 3306
	internal bool isSpeaking;

	// Token: 0x04000CEB RID: 3307
	internal AudioSource audioSource;

	// Token: 0x04000CEC RID: 3308
	private Speech activeVoice;

	// Token: 0x04000CED RID: 3309
	private int activeVoiceNum;

	// Token: 0x04000CEE RID: 3310
	private int voiceBaseFrequency;

	// Token: 0x04000CEF RID: 3311
	private SpeechSynth.VoicingSource voicingSource;

	// Token: 0x04000CF0 RID: 3312
	private List<string> words;

	// Token: 0x04000CF1 RID: 3313
	internal float currentWordTime;

	// Token: 0x04000CF2 RID: 3314
	private float tumbleCooldown;

	// Token: 0x04000CF3 RID: 3315
	private float noClipLoudnessTimer;
}
