using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002A5 RID: 677
	public class AudioEffectPanControl : MonoBehaviour
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060014F2 RID: 5362 RVA: 0x000B33D0 File Offset: 0x000B15D0
		// (set) Token: 0x060014F3 RID: 5363 RVA: 0x000B33D8 File Offset: 0x000B15D8
		public float PanAmount
		{
			get
			{
				return this._panAmount;
			}
			set
			{
				this._panAmount = Mathf.Clamp(value, -1f, 1f);
			}
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x000B33F0 File Offset: 0x000B15F0
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (channels != 2)
			{
				return;
			}
			for (int i = 0; i < data.Length; i += channels)
			{
				if (Mathf.Sign(this.PanAmount) > 0f)
				{
					data[i] = (1f - Mathf.Abs(this.PanAmount)) * data[i];
				}
				else
				{
					data[i + 1] = (1f - Mathf.Abs(this.PanAmount)) * data[i + 1];
				}
			}
		}

		// Token: 0x0400230C RID: 8972
		[SerializeField]
		[Range(-1f, 1f)]
		private float _panAmount;
	}
}
