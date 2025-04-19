using System;
using Audial.Utils;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002AA RID: 682
	public class AudioEffectSimpleDelay : MonoBehaviour
	{
		// Token: 0x06001520 RID: 5408 RVA: 0x000B3D2D File Offset: 0x000B1F2D
		private void Awake()
		{
			this.sampleRate = (Settings.SampleRate = (float)AudioSettings.outputSampleRate);
			this.combFilter = new CombFilter((float)this.DelayLengthMS, 0.5f);
			this.ChangeDelay();
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06001521 RID: 5409 RVA: 0x000B3D5E File Offset: 0x000B1F5E
		// (set) Token: 0x06001522 RID: 5410 RVA: 0x000B3D66 File Offset: 0x000B1F66
		public int DelayLengthMS
		{
			get
			{
				return this._delayLengthMS;
			}
			set
			{
				this._delayLengthMS = Mathf.Clamp(value, 10, 3000);
				this.ChangeDelay();
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06001523 RID: 5411 RVA: 0x000B3D81 File Offset: 0x000B1F81
		// (set) Token: 0x06001524 RID: 5412 RVA: 0x000B3D89 File Offset: 0x000B1F89
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

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06001525 RID: 5413 RVA: 0x000B3DA1 File Offset: 0x000B1FA1
		// (set) Token: 0x06001526 RID: 5414 RVA: 0x000B3DA9 File Offset: 0x000B1FA9
		public float DecayLength
		{
			get
			{
				return this._decayLength;
			}
			set
			{
				this._decayLength = Mathf.Clamp(value, 0.1f, 1f);
			}
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x000B3DC1 File Offset: 0x000B1FC1
		private void ChangeDelay()
		{
			this.combFilter.DelayLength = (float)this.DelayLengthMS;
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x000B3DD8 File Offset: 0x000B1FD8
		private void OnAudioFilterRead(float[] data, int channels)
		{
			this.combFilter.gain = this.DecayLength;
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					float num = data[i + j];
					float num2 = this.combFilter.ProcessSample(j, num);
					this.output = num * (1f - this.DryWet / 2f) + num2 * this.DryWet / 2f;
					data[i + j] = this.output;
				}
				this.combFilter.MoveIndex();
			}
		}

		// Token: 0x0400232A RID: 9002
		private float sampleRate;

		// Token: 0x0400232B RID: 9003
		private CombFilter combFilter;

		// Token: 0x0400232C RID: 9004
		[SerializeField]
		private int _delayLengthMS = 120;

		// Token: 0x0400232D RID: 9005
		private int DelayLengthMSPrev = 10;

		// Token: 0x0400232E RID: 9006
		[SerializeField]
		private float _dryWet = 0.5f;

		// Token: 0x0400232F RID: 9007
		[SerializeField]
		private float _decayLength = 0.25f;

		// Token: 0x04002330 RID: 9008
		private float delayLength;

		// Token: 0x04002331 RID: 9009
		private int delaySamples;

		// Token: 0x04002332 RID: 9010
		private float output;
	}
}
