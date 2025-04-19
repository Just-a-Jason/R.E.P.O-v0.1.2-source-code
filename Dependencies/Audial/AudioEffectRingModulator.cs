using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002A8 RID: 680
	public class AudioEffectRingModulator : MonoBehaviour
	{
		// Token: 0x06001510 RID: 5392 RVA: 0x000B3A92 File Offset: 0x000B1C92
		private void OnEnable()
		{
			this.sampleRate = (Settings.SampleRate = (float)AudioSettings.outputSampleRate);
			this.ResetUtils();
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x000B3AAC File Offset: 0x000B1CAC
		private void ResetUtils()
		{
			this.carrierLFO = new LFO(1f / this.CarrierFrequency);
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06001512 RID: 5394 RVA: 0x000B3AC5 File Offset: 0x000B1CC5
		// (set) Token: 0x06001513 RID: 5395 RVA: 0x000B3ACD File Offset: 0x000B1CCD
		public float CarrierFrequency
		{
			get
			{
				return this._carrierFrequency;
			}
			set
			{
				this._carrierFrequency = Mathf.Clamp(value, 20f, 5000f);
				this.carrierLFO.SetRate(1f / this._carrierFrequency);
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06001514 RID: 5396 RVA: 0x000B3AFC File Offset: 0x000B1CFC
		// (set) Token: 0x06001515 RID: 5397 RVA: 0x000B3B04 File Offset: 0x000B1D04
		public float DryWet
		{
			get
			{
				return this._dryWet;
			}
			set
			{
				this._dryWet = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x06001516 RID: 5398 RVA: 0x000B3B1C File Offset: 0x000B1D1C
		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					float num = data[i + j];
					float num2 = num * this.carrierLFO.GetValue();
					data[i + j] = num * (1f - this.DryWet) + num2 * this.DryWet;
				}
				this.carrierLFO.MoveIndex();
			}
		}

		// Token: 0x0400231F RID: 8991
		private float output;

		// Token: 0x04002320 RID: 8992
		private float sampleRate;

		// Token: 0x04002321 RID: 8993
		public LFO carrierLFO;

		// Token: 0x04002322 RID: 8994
		[SerializeField]
		private float _carrierFrequency = 400f;

		// Token: 0x04002323 RID: 8995
		[SerializeField]
		private float _dryWet = 0.75f;
	}
}
