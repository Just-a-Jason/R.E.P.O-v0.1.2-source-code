using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002AE RID: 686
	public class AudioEffectTremolo : MonoBehaviour
	{
		// Token: 0x0600153B RID: 5435 RVA: 0x000B42CE File Offset: 0x000B24CE
		private void Awake()
		{
			this.sampleRate = (Settings.SampleRate = (float)AudioSettings.outputSampleRate);
			this.ResetUtils();
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x000B42E8 File Offset: 0x000B24E8
		private void ResetUtils()
		{
			this.carrierLFO = new LFO(1f / this.CarrierFrequency);
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600153D RID: 5437 RVA: 0x000B4301 File Offset: 0x000B2501
		// (set) Token: 0x0600153E RID: 5438 RVA: 0x000B4309 File Offset: 0x000B2509
		public float CarrierFrequency
		{
			get
			{
				return this._carrierFrequency;
			}
			set
			{
				this._carrierFrequency = Mathf.Clamp(value, 2f, 20f);
				this.carrierLFO.SetRate(1f / this._carrierFrequency);
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600153F RID: 5439 RVA: 0x000B4338 File Offset: 0x000B2538
		// (set) Token: 0x06001540 RID: 5440 RVA: 0x000B4340 File Offset: 0x000B2540
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

		// Token: 0x06001541 RID: 5441 RVA: 0x000B4358 File Offset: 0x000B2558
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

		// Token: 0x0400234A RID: 9034
		private float output;

		// Token: 0x0400234B RID: 9035
		private float sampleRate;

		// Token: 0x0400234C RID: 9036
		public LFO carrierLFO;

		// Token: 0x0400234D RID: 9037
		[SerializeField]
		private float _carrierFrequency = 10f;

		// Token: 0x0400234E RID: 9038
		[SerializeField]
		private float _dryWet = 0.75f;
	}
}
