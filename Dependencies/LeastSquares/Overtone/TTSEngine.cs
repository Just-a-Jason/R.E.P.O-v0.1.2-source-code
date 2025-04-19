using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Overtone.Scripts;
using UnityEngine;

namespace LeastSquares.Overtone
{
	// Token: 0x02000298 RID: 664
	public class TTSEngine : MonoBehaviour
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06001450 RID: 5200 RVA: 0x000B192F File Offset: 0x000AFB2F
		// (set) Token: 0x06001451 RID: 5201 RVA: 0x000B1937 File Offset: 0x000AFB37
		public bool Loaded { get; private set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06001452 RID: 5202 RVA: 0x000B1940 File Offset: 0x000AFB40
		// (set) Token: 0x06001453 RID: 5203 RVA: 0x000B1948 File Offset: 0x000AFB48
		public bool Disposed { get; private set; }

		// Token: 0x06001454 RID: 5204 RVA: 0x000B1954 File Offset: 0x000AFB54
		private void Awake()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				this._context = TTSNative.OvertoneStart();
				this.Loaded = true;
			}
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x000B19A0 File Offset: 0x000AFBA0
		public Task<AudioClip> Speak(string text, TTSVoiceNative voice)
		{
			TTSEngine.<Speak>d__11 <Speak>d__;
			<Speak>d__.<>t__builder = AsyncTaskMethodBuilder<AudioClip>.Create();
			<Speak>d__.<>4__this = this;
			<Speak>d__.text = text;
			<Speak>d__.voice = voice;
			<Speak>d__.<>1__state = -1;
			<Speak>d__.<>t__builder.Start<TTSEngine.<Speak>d__11>(ref <Speak>d__);
			return <Speak>d__.<>t__builder.Task;
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x000B19F3 File Offset: 0x000AFBF3
		private AudioClip MakeClip(string name, TTSResult result)
		{
			AudioClip audioClip = AudioClip.Create(name ?? string.Empty, result.Samples.Length, (int)result.Channels, (int)result.SampleRate, false);
			audioClip.SetData(result.Samples, 0);
			return audioClip;
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x000B1A28 File Offset: 0x000AFC28
		public Task<TTSResult> SpeakSamples(SpeechUnit unit, TTSVoiceNative voice)
		{
			TTSEngine.<SpeakSamples>d__13 <SpeakSamples>d__;
			<SpeakSamples>d__.<>t__builder = AsyncTaskMethodBuilder<TTSResult>.Create();
			<SpeakSamples>d__.<>4__this = this;
			<SpeakSamples>d__.unit = unit;
			<SpeakSamples>d__.voice = voice;
			<SpeakSamples>d__.<>1__state = -1;
			<SpeakSamples>d__.<>t__builder.Start<TTSEngine.<SpeakSamples>d__13>(ref <SpeakSamples>d__);
			return <SpeakSamples>d__.<>t__builder.Task;
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x000B1A7C File Offset: 0x000AFC7C
		private float[] PtrToSamples(IntPtr int16Buffer, uint samplesLength)
		{
			float[] array = new float[samplesLength];
			short[] array2 = new short[samplesLength];
			Marshal.Copy(int16Buffer, array2, 0, (int)samplesLength);
			int num = 0;
			while ((long)num < (long)((ulong)samplesLength))
			{
				array[num] = (float)array2[num] / 32767f;
				num++;
			}
			return array;
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x000B1ABC File Offset: 0x000AFCBC
		private void OnDestroy()
		{
			this.Dispose();
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x000B1AC4 File Offset: 0x000AFCC4
		private void Dispose()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				this.Disposed = true;
				if (this._context != IntPtr.Zero)
				{
					TTSNative.OvertoneFree(this._context);
				}
				Debug.Log("Successfully cleaned up TTS Engine");
			}
		}

		// Token: 0x040022AF RID: 8879
		private IntPtr _context;

		// Token: 0x040022B0 RID: 8880
		private readonly object _lock = new object();
	}
}
