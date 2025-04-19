using System;
using UnityEngine;

namespace Audial.Utils
{
	// Token: 0x020002B4 RID: 692
	[Serializable]
	public class Envelope
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06001556 RID: 5462 RVA: 0x000B46B9 File Offset: 0x000B28B9
		// (set) Token: 0x06001557 RID: 5463 RVA: 0x000B46C1 File Offset: 0x000B28C1
		public float Attack
		{
			get
			{
				return this._attack;
			}
			set
			{
				this._attack = value;
				this.attackCoeff = Mathf.Exp(-1f / (Settings.SampleRate * this._attack));
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06001558 RID: 5464 RVA: 0x000B46E7 File Offset: 0x000B28E7
		// (set) Token: 0x06001559 RID: 5465 RVA: 0x000B46EF File Offset: 0x000B28EF
		public float Release
		{
			get
			{
				return this._release;
			}
			set
			{
				this._release = value;
				this.releaseCoeff = Mathf.Exp(-1f / (Settings.SampleRate * this._release));
			}
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x000B4715 File Offset: 0x000B2915
		public Envelope(float attack, float release)
		{
			this.Attack = attack;
			this.Release = release;
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x000B4738 File Offset: 0x000B2938
		public float ProcessSample(float sample)
		{
			float num = (sample > this.env) ? this.attackCoeff : this.releaseCoeff;
			this.env = (1f - num) * sample + num * this.env;
			return this.env;
		}

		// Token: 0x04002361 RID: 9057
		private EnvelopeState envelopeState;

		// Token: 0x04002362 RID: 9058
		private float env;

		// Token: 0x04002363 RID: 9059
		private float _attack;

		// Token: 0x04002364 RID: 9060
		public float attackCoeff;

		// Token: 0x04002365 RID: 9061
		private float _release;

		// Token: 0x04002366 RID: 9062
		private float releaseCoeff;

		// Token: 0x04002367 RID: 9063
		private float sustain = 1f;

		// Token: 0x04002368 RID: 9064
		private float sampleRate;
	}
}
