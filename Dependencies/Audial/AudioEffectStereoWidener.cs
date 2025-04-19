using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002AD RID: 685
	public class AudioEffectStereoWidener : MonoBehaviour
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06001537 RID: 5431 RVA: 0x000B4241 File Offset: 0x000B2441
		// (set) Token: 0x06001538 RID: 5432 RVA: 0x000B4249 File Offset: 0x000B2449
		public float Width
		{
			get
			{
				return this._width;
			}
			set
			{
				this._width = Mathf.Clamp(value, 0f, 2f);
			}
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x000B4264 File Offset: 0x000B2464
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (channels < 2)
			{
				return;
			}
			float num = this.Width * 0.5f;
			for (int i = 0; i < data.Length; i += channels)
			{
				float num2 = (data[i] + data[i + 1]) * 0.5f;
				float num3 = (data[i] - data[i + 1]) * num;
				data[i] = num2 + num3;
				data[i + 1] = num2 - num3;
			}
		}

		// Token: 0x04002349 RID: 9033
		[SerializeField]
		[Range(0f, 2f)]
		private float _width = 1.3f;
	}
}
