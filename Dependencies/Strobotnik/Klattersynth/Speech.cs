using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Strobotnik.Klattersynth
{
	// Token: 0x0200029C RID: 668
	public class Speech : MonoBehaviour
	{
		// Token: 0x06001472 RID: 5234 RVA: 0x000B1CF3 File Offset: 0x000AFEF3
		private bool errCheck(bool errorWhenTrue, string logErrorString)
		{
			if (errorWhenTrue)
			{
				if (logErrorString != null)
				{
					Debug.LogError(logErrorString, this);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06001473 RID: 5235 RVA: 0x000B1D08 File Offset: 0x000AFF08
		private void cache(SpeechClip sc)
		{
			if (this.maxAutoCachedClips <= 0)
			{
				return;
			}
			if (this.cachedSpeechClips == null)
			{
				this.cachedSpeechClips = new List<SpeechClip>(this.maxAutoCachedClips);
			}
			else
			{
				int num = this.cachedSpeechClips.FindIndex((SpeechClip x) => x.hash == sc.hash);
				if (num >= 0)
				{
					this.cachedSpeechClips[num] = sc;
					return;
				}
				if (this.cachedSpeechClips.Count >= this.maxAutoCachedClips)
				{
					this.cachedSpeechClips.RemoveRange(0, this.cachedSpeechClips.Count - (this.maxAutoCachedClips - 1));
				}
			}
			this.cachedSpeechClips.Add(sc);
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x000B1DBC File Offset: 0x000AFFBC
		private SpeechClip findFromCache(StringBuilder text, int freq, SpeechSynth.VoicingSource voicingSrc, bool bracketsAsPhonemes)
		{
			if (this.cachedSpeechClips == null)
			{
				return null;
			}
			ulong hash = SpeechSynth.makeHashCode(text, freq, voicingSrc, bracketsAsPhonemes);
			int num = this.cachedSpeechClips.FindIndex((SpeechClip x) => x.hash == hash);
			if (num < 0)
			{
				return null;
			}
			SpeechClip speechClip = this.cachedSpeechClips[num];
			if (num < this.cachedSpeechClips.Count - 1)
			{
				this.cachedSpeechClips.RemoveAt(num);
				this.cachedSpeechClips.Add(speechClip);
			}
			return speechClip;
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x000B1E40 File Offset: 0x000B0040
		public void speakNextScheduled()
		{
			if (this.scheduled == null || this.scheduled.Count == 0)
			{
				return;
			}
			Speech.ScheduledUnit scheduledUnit = this.scheduled[0];
			this.scheduled.RemoveAt(0);
			if (scheduledUnit.pregenClip != null)
			{
				this.speak(scheduledUnit.pregenClip);
				return;
			}
			this.speak(scheduledUnit.voiceBaseFrequency, scheduledUnit.voicingSource, scheduledUnit.text, scheduledUnit.bracketsAsPhonemes);
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x000B1EAF File Offset: 0x000B00AF
		public bool isTalking()
		{
			return this.talking;
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x000B1EB7 File Offset: 0x000B00B7
		public float getCurrentLoudness()
		{
			return this.speechSynth.getCurrentLoudness();
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x000B1EC4 File Offset: 0x000B00C4
		public string getPhonemes()
		{
			return this.speechSynth.getPhonemes();
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x000B1ED1 File Offset: 0x000B00D1
		public void speak(SpeechClip pregenSpeech)
		{
			this.speechSynth.speak(pregenSpeech);
			this.talking = true;
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x000B1EE6 File Offset: 0x000B00E6
		public void speak(string text, bool bracketsAsPhonemes = false)
		{
			this.speak(this.voiceBaseFrequency, this.voicingSource, text, bracketsAsPhonemes);
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x000B1EFC File Offset: 0x000B00FC
		public void speak(int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource, string text, bool bracketsAsPhonemes = false)
		{
			if (this.errCheck(text == null, "null text"))
			{
				return;
			}
			if (this.speakSB == null)
			{
				this.speakSB = new StringBuilder(text.Length * 3 / 2);
			}
			this.speakSB.Length = 0;
			this.speakSB.Append(text);
			this.speak(voiceBaseFrequency, voicingSource, this.speakSB, bracketsAsPhonemes);
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x000B1F61 File Offset: 0x000B0161
		public void speak(StringBuilder text, bool bracketsAsPhonemes = false)
		{
			this.speak(this.voiceBaseFrequency, this.voicingSource, text, bracketsAsPhonemes);
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x000B1F77 File Offset: 0x000B0177
		private void VoiceText(StringBuilder text, float wordTime)
		{
			if (this.ttsVoice)
			{
				this.ttsVoice.VoiceText(text.ToString(), wordTime);
			}
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x000B1F98 File Offset: 0x000B0198
		public void speak(int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource, StringBuilder text, bool bracketsAsPhonemes = false)
		{
			this.VoiceText(text, Time.time);
			text = new StringBuilder(text.ToString().ToLower());
			if (this.errCheck(text == null, "null text (SB)"))
			{
				return;
			}
			if (!this.useStreamingMode)
			{
				SpeechClip speechClip = this.findFromCache(text, voiceBaseFrequency, voicingSource, bracketsAsPhonemes);
				if (speechClip == null)
				{
					this.pregenerate(out speechClip, voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes, true);
				}
				if (speechClip != null)
				{
					this.talking = true;
					this.speechSynth.speak(speechClip);
					return;
				}
			}
			else
			{
				this.talking = true;
				this.speechSynth.speak(text, voiceBaseFrequency, voicingSource, bracketsAsPhonemes);
			}
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x000B2028 File Offset: 0x000B0228
		public void pregenerate(string text, bool bracketsAsPhonemes = false)
		{
			SpeechClip speechClip;
			this.pregenerate(out speechClip, text, bracketsAsPhonemes, true);
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x000B2040 File Offset: 0x000B0240
		public void pregenerate(out SpeechClip speechClip, string text, bool bracketsAsPhonemes = false, bool addToCache = false)
		{
			speechClip = null;
			if (this.errCheck(text == null, "null text"))
			{
				return;
			}
			if (this.speakSB == null)
			{
				this.speakSB = new StringBuilder(text.Length * 3 / 2);
			}
			this.speakSB.Length = 0;
			this.speakSB.Append(text);
			this.pregenerate(out speechClip, this.speakSB, bracketsAsPhonemes, addToCache);
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x000B20A8 File Offset: 0x000B02A8
		public void pregenerate(out SpeechClip speechClip, StringBuilder text, bool bracketsAsPhonemes = false, bool addToCache = false)
		{
			this.pregenerate(out speechClip, this.voiceBaseFrequency, this.voicingSource, text, bracketsAsPhonemes, addToCache);
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x000B20C1 File Offset: 0x000B02C1
		public void pregenerate(out SpeechClip speechClip, int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource, StringBuilder text, bool bracketsAsPhonemes = false, bool addToCache = false)
		{
			speechClip = null;
			if (this.errCheck(text == null, "null text (SB)"))
			{
				return;
			}
			speechClip = this.speechSynth.pregenerate(text, voiceBaseFrequency, voicingSource, bracketsAsPhonemes, true);
			if (speechClip != null && addToCache)
			{
				this.cache(speechClip);
			}
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x000B2100 File Offset: 0x000B0300
		public void schedule(SpeechClip speechClip)
		{
			Speech.ScheduledUnit item = default(Speech.ScheduledUnit);
			item.pregenClip = speechClip;
			this.scheduled.Add(item);
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x000B2129 File Offset: 0x000B0329
		public void scheduleClear()
		{
			this.scheduled.Clear();
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x000B2136 File Offset: 0x000B0336
		public void schedule(string text, bool bracketsAsPhonemes = false)
		{
			this.schedule(this.voiceBaseFrequency, this.voicingSource, text, bracketsAsPhonemes);
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x000B214C File Offset: 0x000B034C
		public void schedule(StringBuilder text, bool bracketsAsPhonemes = false)
		{
			this.schedule(this.voiceBaseFrequency, this.voicingSource, text, bracketsAsPhonemes);
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x000B2164 File Offset: 0x000B0364
		public void schedule(int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource, string text, bool bracketsAsPhonemes = false)
		{
			if (!this.talking && this.scheduled.Count == 0)
			{
				this.speak(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
				return;
			}
			Speech.ScheduledUnit item = default(Speech.ScheduledUnit);
			item.voiceBaseFrequency = voiceBaseFrequency;
			item.voicingSource = voicingSource;
			item.text = text;
			item.bracketsAsPhonemes = bracketsAsPhonemes;
			item.pregenClip = null;
			this.scheduled.Add(item);
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x000B21CF File Offset: 0x000B03CF
		public void schedule(int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource, StringBuilder text, bool bracketsAsPhonemes = false)
		{
			if (!this.talking && this.scheduled.Count == 0)
			{
				this.speak(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
				return;
			}
			this.schedule(voiceBaseFrequency, voicingSource, text.ToString(), bracketsAsPhonemes);
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x000B2202 File Offset: 0x000B0402
		public void stop(bool allScheduled = false)
		{
			if (allScheduled)
			{
				this.scheduled.Clear();
			}
			this.speechSynth.stop();
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x000B221D File Offset: 0x000B041D
		public void cacheClear()
		{
			if (this.cachedSpeechClips != null)
			{
				this.cachedSpeechClips.Clear();
			}
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x000B2234 File Offset: 0x000B0434
		private void Awake()
		{
			this.audioSrc = base.GetComponentInParent<AudioSource>();
			this.speechSynth = new SpeechSynth();
			this.speechSynth.init(this.audioSrc, this.useStreamingMode, 11025, this.msPerSpeechFrame, this.flutter, this.flutterSpeed);
			this.ttsVoice = base.GetComponentInParent<TTSVoice>();
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x000B2292 File Offset: 0x000B0492
		private void Update()
		{
			this.talking = this.speechSynth.update();
			if (!this.talking)
			{
				this.speakNextScheduled();
			}
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x000B22B3 File Offset: 0x000B04B3
		private void OnDestroy()
		{
			this.ClearAllData();
		}

		// Token: 0x0600148E RID: 5262 RVA: 0x000B22BC File Offset: 0x000B04BC
		private void ClearAllData()
		{
			this.stop(true);
			this.cacheClear();
			this.scheduleClear();
			if (this.speakSB != null)
			{
				this.speakSB.Clear();
			}
			if (this.audioSrc)
			{
				this.audioSrc.Stop();
			}
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x000B2308 File Offset: 0x000B0508
		private void OnDisable()
		{
			this.ClearAllData();
		}

		// Token: 0x040022BC RID: 8892
		[Tooltip("When true, speech is real-time generated (played using a single small looping audio clip).\n\nNOTE: Not supported with WebGL, will be auto-disabled in Start() when running in WebGL!")]
		public bool useStreamingMode = true;

		// Token: 0x040022BD RID: 8893
		[Tooltip("Maximum amount of speech clips to automatically cache in non-streaming mode.\n(Least recently used are discarded when going over this amount.)")]
		public int maxAutoCachedClips = 10;

		// Token: 0x040022BE RID: 8894
		[Tooltip("Base frequency for the synthesized voice.\nCan be runtime-adjusted.")]
		public int voiceBaseFrequency = 220;

		// Token: 0x040022BF RID: 8895
		[Tooltip("Type of \"voicing source\".\nCan be runtime-adjusted.")]
		public SpeechSynth.VoicingSource voicingSource;

		// Token: 0x040022C0 RID: 8896
		[Tooltip("How many milliseconds to use per one \"speech frame\".")]
		[Range(1f, 100f)]
		public int msPerSpeechFrame = 10;

		// Token: 0x040022C1 RID: 8897
		[Tooltip("Amount of flutter in voice.")]
		[Range(0f, 200f)]
		public int flutter = 10;

		// Token: 0x040022C2 RID: 8898
		[Tooltip("Speed of the flutter.")]
		[Range(0.001f, 100f)]
		public float flutterSpeed = 1f;

		// Token: 0x040022C3 RID: 8899
		private TTSVoice ttsVoice;

		// Token: 0x040022C4 RID: 8900
		private const int sampleRate = 11025;

		// Token: 0x040022C5 RID: 8901
		private bool talking;

		// Token: 0x040022C6 RID: 8902
		private AudioSource audioSrc;

		// Token: 0x040022C7 RID: 8903
		private SpeechSynth speechSynth;

		// Token: 0x040022C8 RID: 8904
		private StringBuilder speakSB;

		// Token: 0x040022C9 RID: 8905
		private List<SpeechClip> cachedSpeechClips;

		// Token: 0x040022CA RID: 8906
		private List<Speech.ScheduledUnit> scheduled = new List<Speech.ScheduledUnit>(5);

		// Token: 0x020003D1 RID: 977
		private struct ScheduledUnit
		{
			// Token: 0x04002930 RID: 10544
			public string text;

			// Token: 0x04002931 RID: 10545
			public int voiceBaseFrequency;

			// Token: 0x04002932 RID: 10546
			public SpeechSynth.VoicingSource voicingSource;

			// Token: 0x04002933 RID: 10547
			public bool bracketsAsPhonemes;

			// Token: 0x04002934 RID: 10548
			public SpeechClip pregenClip;
		}
	}
}
