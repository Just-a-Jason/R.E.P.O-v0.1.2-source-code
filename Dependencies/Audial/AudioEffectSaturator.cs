using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002A9 RID: 681
	public class AudioEffectSaturator : MonoBehaviour
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06001518 RID: 5400 RVA: 0x000B3B9C File Offset: 0x000B1D9C
		// (set) Token: 0x06001519 RID: 5401 RVA: 0x000B3BA4 File Offset: 0x000B1DA4
		public float InputGain
		{
			get
			{
				return this._inputGain;
			}
			set
			{
				this._inputGain = Mathf.Clamp(value, 0f, 3f);
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600151A RID: 5402 RVA: 0x000B3BBC File Offset: 0x000B1DBC
		// (set) Token: 0x0600151B RID: 5403 RVA: 0x000B3BC4 File Offset: 0x000B1DC4
		public float Threshold
		{
			get
			{
				return this._threshold;
			}
			set
			{
				this._threshold = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600151C RID: 5404 RVA: 0x000B3BDC File Offset: 0x000B1DDC
		// (set) Token: 0x0600151D RID: 5405 RVA: 0x000B3BE4 File Offset: 0x000B1DE4
		public float Amount
		{
			get
			{
				return this._amount;
			}
			set
			{
				this._amount = Mathf.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x0600151E RID: 5406 RVA: 0x000B3BFC File Offset: 0x000B1DFC
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (this.Amount == 0f)
			{
				return;
			}
			for (int i = 0; i < channels; i++)
			{
				for (int j = 0; j < data.Length; j += channels)
				{
					this.input = data[j + i] * this.InputGain;
					this.sampleAbs = Mathf.Abs(this.input);
					this.sampleSign = Mathf.Sign(this.input);
					if (this.sampleAbs > 1f)
					{
						this.input = (this.Threshold + 1f) / 2f * this.sampleSign;
					}
					else if (this.sampleAbs > this.Threshold)
					{
						this.input = (this.Threshold + (this.sampleAbs - this.Threshold) / (1f + Mathf.Pow((this.sampleAbs - this.Threshold) / (1f - this.Amount), 2f))) * this.sampleSign;
					}
					data[j + i] = this.input;
				}
			}
		}

		// Token: 0x04002324 RID: 8996
		[SerializeField]
		[Range(0f, 3f)]
		private float _inputGain = 1f;

		// Token: 0x04002325 RID: 8997
		[SerializeField]
		[Range(0f, 1f)]
		private float _threshold = 0.247f;

		// Token: 0x04002326 RID: 8998
		[SerializeField]
		[Range(0f, 1f)]
		public float _amount = 0.5f;

		// Token: 0x04002327 RID: 8999
		private float input;

		// Token: 0x04002328 RID: 9000
		private float sampleAbs;

		// Token: 0x04002329 RID: 9001
		private float sampleSign;
	}
}
