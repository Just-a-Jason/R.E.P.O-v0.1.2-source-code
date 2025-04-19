using System;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public class DebugAxelTemp : MonoBehaviour
{
	// Token: 0x06000F5D RID: 3933 RVA: 0x0008CDD8 File Offset: 0x0008AFD8
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			this.clipData = new float[this.loopClipLength];
			for (int i = 0; i < this.clipData.Length; i++)
			{
				this.clipData[i] = Random.Range(-1f, 1f);
			}
			AudioClip.Create("Speech Loop", this.loopClipLength, 1, this.sampleRate, true, new AudioClip.PCMReaderCallback(this.callback_audioRead));
		}
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x0008CE50 File Offset: 0x0008B050
	private void callback_audioRead(float[] output)
	{
		for (int i = 0; i < output.Length; i++)
		{
			output[i] = this.clipData[i];
		}
	}

	// Token: 0x040019F7 RID: 6647
	private int loopClipLength = 4096;

	// Token: 0x040019F8 RID: 6648
	private int sampleRate = 11025;

	// Token: 0x040019F9 RID: 6649
	private float[] clipData;
}
