using System;
using TMPro;
using UnityEngine;

// Token: 0x0200025F RID: 607
public class Timecode : MonoBehaviour
{
	// Token: 0x060012C4 RID: 4804 RVA: 0x000A4314 File Offset: 0x000A2514
	public Timecode.TimeSnapshot GetSnapshot()
	{
		return new Timecode.TimeSnapshot
		{
			TimecodeSecond = Mathf.FloorToInt(this.timeSec),
			TimecodeMinute = Mathf.RoundToInt(this.timeMin),
			TimecodeHour = Mathf.RoundToInt(this.timeHour),
			TimeMinute = this.time.minute,
			TimeHour = this.time.hour
		};
	}

	// Token: 0x060012C5 RID: 4805 RVA: 0x000A437C File Offset: 0x000A257C
	public void SetTime(Timecode.TimeSnapshot snapshot)
	{
		this.timeSec = (float)snapshot.TimecodeSecond;
		this.timeMin = (float)snapshot.TimecodeMinute;
		this.timeHour = (float)snapshot.TimecodeHour;
		this.time.minute = snapshot.TimeMinute;
		this.time.hour = snapshot.TimeHour;
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x000A43D2 File Offset: 0x000A25D2
	public void SetToStartSnapshot()
	{
		this.SetTime(this.StartSnapshot);
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x000A43E0 File Offset: 0x000A25E0
	private void Update()
	{
		if (!this.RewindEffect.PlayRewind && GameDirector.instance.currentState < GameDirector.gameState.Outro)
		{
			if (this.SetStartSnapshot)
			{
				this.StartSnapshot = this.GetSnapshot();
				this.SetStartSnapshot = false;
			}
			this.timeSec += Time.deltaTime;
			if (Mathf.Round(this.timeSec) >= 60f)
			{
				this.timeSec = 0f;
				this.timeMin += 1f;
				this.time.minute++;
				if (this.time.minute >= 60)
				{
					this.time.minute = 0;
					this.time.hour++;
					if (this.time.hour >= 24)
					{
						this.time.hour = 0;
						this.date.UpdateDay();
					}
				}
				if (this.timeMin >= 60f)
				{
					this.timeMin = 0f;
					this.timeHour += 1f;
				}
			}
		}
		string text = this.timeHour.ToString();
		if (this.timeHour < 10f)
		{
			text = "0" + text;
		}
		string text2 = this.timeMin.ToString();
		if (this.timeMin < 10f)
		{
			text2 = "0" + text2;
		}
		float num = Mathf.Round(this.timeSec);
		string text3 = num.ToString();
		if (num < 10f)
		{
			text3 = "0" + text3;
		}
		this.textMesh.text = string.Concat(new string[]
		{
			text,
			":",
			text2,
			":",
			text3
		});
	}

	// Token: 0x04001FC4 RID: 8132
	public RewindEffect RewindEffect;

	// Token: 0x04001FC5 RID: 8133
	public TextMeshProUGUI textMesh;

	// Token: 0x04001FC6 RID: 8134
	public CurrentTime time;

	// Token: 0x04001FC7 RID: 8135
	public Date date;

	// Token: 0x04001FC8 RID: 8136
	private float timeSec;

	// Token: 0x04001FC9 RID: 8137
	private float timeMin;

	// Token: 0x04001FCA RID: 8138
	private float timeHour;

	// Token: 0x04001FCB RID: 8139
	private bool SetStartSnapshot = true;

	// Token: 0x04001FCC RID: 8140
	private Timecode.TimeSnapshot StartSnapshot;

	// Token: 0x020003B7 RID: 951
	[Serializable]
	public class TimeSnapshot
	{
		// Token: 0x040028AB RID: 10411
		public int TimecodeHour;

		// Token: 0x040028AC RID: 10412
		public int TimecodeMinute;

		// Token: 0x040028AD RID: 10413
		public int TimecodeSecond;

		// Token: 0x040028AE RID: 10414
		public int TimeHour;

		// Token: 0x040028AF RID: 10415
		public int TimeMinute;
	}
}
