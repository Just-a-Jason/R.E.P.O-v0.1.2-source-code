using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002A4 RID: 676
	public class AudioEffectGate : MonoBehaviour
	{
		// Token: 0x060014E5 RID: 5349 RVA: 0x000B31CD File Offset: 0x000B13CD
		private void OnEnable()
		{
			this.sampleRate = (float)AudioSettings.outputSampleRate;
			this.envelope = new Envelope(this.Attack, this.Release);
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060014E6 RID: 5350 RVA: 0x000B31F2 File Offset: 0x000B13F2
		// (set) Token: 0x060014E7 RID: 5351 RVA: 0x000B31FA File Offset: 0x000B13FA
		public float InputGain
		{
			get
			{
				return this._inputGain;
			}
			set
			{
				if (value == this._inputGain)
				{
					return;
				}
				this._inputGain = Mathf.Clamp(value, 0f, 3f);
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060014E8 RID: 5352 RVA: 0x000B321C File Offset: 0x000B141C
		// (set) Token: 0x060014E9 RID: 5353 RVA: 0x000B3224 File Offset: 0x000B1424
		public float Threshold
		{
			get
			{
				return this._threshold;
			}
			set
			{
				if (value == this._threshold)
				{
					return;
				}
				this._threshold = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060014EA RID: 5354 RVA: 0x000B3246 File Offset: 0x000B1446
		// (set) Token: 0x060014EB RID: 5355 RVA: 0x000B324E File Offset: 0x000B144E
		public float Attack
		{
			get
			{
				return this._attack;
			}
			set
			{
				if (value == this._attack)
				{
					return;
				}
				this._attack = Mathf.Clamp(value, 0f, 1f);
				this.envelope.Attack = this._attack;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060014EC RID: 5356 RVA: 0x000B3281 File Offset: 0x000B1481
		// (set) Token: 0x060014ED RID: 5357 RVA: 0x000B3289 File Offset: 0x000B1489
		public float Release
		{
			get
			{
				return this._release;
			}
			set
			{
				if (value == this._release)
				{
					return;
				}
				this._release = Mathf.Clamp(value, 0f, 1f);
				this.envelope.Release = this._release;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060014EE RID: 5358 RVA: 0x000B32BC File Offset: 0x000B14BC
		// (set) Token: 0x060014EF RID: 5359 RVA: 0x000B32C4 File Offset: 0x000B14C4
		public float OutputGain
		{
			get
			{
				return this._outputGain;
			}
			set
			{
				if (value == this._outputGain)
				{
					return;
				}
				this._outputGain = Mathf.Clamp(value, 0f, 5f);
			}
		}

		// Token: 0x060014F0 RID: 5360 RVA: 0x000B32E8 File Offset: 0x000B14E8
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				data[i] *= this.InputGain;
				data[i + 1] *= this.InputGain;
				float sample = Mathf.Sqrt(data[i] * data[i] + data[i + 1] * data[i + 1]);
				float num = this.envelope.ProcessSample(sample);
				float num2 = 1f;
				if (num < this.Threshold)
				{
					num2 = Mathf.Pow(num / 4f, 4f);
				}
				data[i] *= num2 * this.OutputGain;
				data[i + 1] *= num2 * this.OutputGain;
			}
		}

		// Token: 0x04002304 RID: 8964
		[SerializeField]
		private Envelope envelope;

		// Token: 0x04002305 RID: 8965
		private float sampleRate;

		// Token: 0x04002306 RID: 8966
		[SerializeField]
		private float _inputGain = 1f;

		// Token: 0x04002307 RID: 8967
		[SerializeField]
		private float _threshold = 0.247f;

		// Token: 0x04002308 RID: 8968
		[SerializeField]
		private float _attack;

		// Token: 0x04002309 RID: 8969
		[SerializeField]
		public float _release = 0.75f;

		// Token: 0x0400230A RID: 8970
		[SerializeField]
		private float _outputGain = 1f;

		// Token: 0x0400230B RID: 8971
		private float env;
	}
}
