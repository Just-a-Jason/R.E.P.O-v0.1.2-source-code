using System;
using TMPro;
using UnityEngine;

// Token: 0x0200024D RID: 589
public class Date : MonoBehaviour
{
	// Token: 0x0600125B RID: 4699 RVA: 0x000A1680 File Offset: 0x0009F880
	private void Start()
	{
		this.year = Random.Range(this.yearMin, this.yearMax);
		this.currentMonth = Random.Range(0, this.Months.Length);
		this.currentDay = Random.Range(1, this.Days[this.currentMonth]);
		this.UpdateText();
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x000A16D8 File Offset: 0x0009F8D8
	public void UpdateDay()
	{
		this.currentDay++;
		if (this.currentDay > this.Days[this.currentMonth])
		{
			this.currentDay = 1;
			this.currentMonth++;
			if (this.currentMonth >= this.Months.Length)
			{
				this.currentMonth = 0;
				this.year++;
			}
		}
		this.UpdateText();
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x000A1748 File Offset: 0x0009F948
	private void UpdateText()
	{
		this.textMesh.text = this.Months[this.currentMonth] + this.currentDay.ToString() + " " + this.year.ToString();
	}

	// Token: 0x04001F2A RID: 7978
	public TextMeshProUGUI textMesh;

	// Token: 0x04001F2B RID: 7979
	public int yearMin;

	// Token: 0x04001F2C RID: 7980
	public int yearMax;

	// Token: 0x04001F2D RID: 7981
	private int year;

	// Token: 0x04001F2E RID: 7982
	public string[] Months;

	// Token: 0x04001F2F RID: 7983
	public int[] Days;

	// Token: 0x04001F30 RID: 7984
	private int currentMonth;

	// Token: 0x04001F31 RID: 7985
	private int currentDay;
}
