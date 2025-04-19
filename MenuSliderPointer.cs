using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001F2 RID: 498
public class MenuSliderPointer : MonoBehaviour
{
	// Token: 0x06001083 RID: 4227 RVA: 0x00094CEA File Offset: 0x00092EEA
	private void Start()
	{
		this.rawImage = base.GetComponent<RawImage>();
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x00094CF8 File Offset: 0x00092EF8
	private void Update()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(1f, 1f, 1f), 15f * Time.deltaTime);
		this.rawImage.color = Color.Lerp(this.rawImage.color, Color.red, 5f * Time.deltaTime);
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x00094D6C File Offset: 0x00092F6C
	public void Tick()
	{
		if (!this.rawImage)
		{
			return;
		}
		base.transform.localScale = new Vector3(1f, 3f, 1f);
		this.rawImage.color = new Color(0.5f, 0.5f, 1f, 1f);
	}

	// Token: 0x04001B95 RID: 7061
	private RawImage rawImage;
}
