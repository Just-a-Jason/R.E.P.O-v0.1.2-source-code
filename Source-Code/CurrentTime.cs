using System;
using TMPro;
using UnityEngine;

// Token: 0x0200024C RID: 588
public class CurrentTime : MonoBehaviour
{
	// Token: 0x06001258 RID: 4696 RVA: 0x000A15E7 File Offset: 0x0009F7E7
	private void Start()
	{
		this.hour = Random.Range(0, 23);
		this.minute = Random.Range(0, 59);
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x000A1608 File Offset: 0x0009F808
	private void Update()
	{
		string text = this.hour.ToString();
		if ((float)this.hour < 10f)
		{
			text = "0" + text;
		}
		string text2 = this.minute.ToString();
		if ((float)this.minute < 10f)
		{
			text2 = "0" + text2;
		}
		this.textMesh.text = text + ":" + text2;
	}

	// Token: 0x04001F27 RID: 7975
	public TextMeshProUGUI textMesh;

	// Token: 0x04001F28 RID: 7976
	[HideInInspector]
	public int hour;

	// Token: 0x04001F29 RID: 7977
	[HideInInspector]
	public int minute;
}
