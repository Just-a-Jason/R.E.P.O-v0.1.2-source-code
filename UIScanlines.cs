using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000AD RID: 173
public class UIScanlines : MonoBehaviour
{
	// Token: 0x060006BC RID: 1724 RVA: 0x00040A2C File Offset: 0x0003EC2C
	private void Start()
	{
		this.image = base.GetComponent<Image>();
		this.originalAlpha = this.image.color.a;
		this.parentText = base.GetComponentInParent<TextMeshProUGUI>();
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x00040A5C File Offset: 0x0003EC5C
	private void Update()
	{
		if (!this.parentText)
		{
			return;
		}
		if (this.changeColorTimer <= 0f)
		{
			Color color = this.parentText.color;
			this.image.color = new Color(color.r, color.g, color.b, this.originalAlpha);
			this.changeColorTimer = 0.03f;
			return;
		}
		this.changeColorTimer -= Time.deltaTime;
	}

	// Token: 0x04000B6A RID: 2922
	private TextMeshProUGUI parentText;

	// Token: 0x04000B6B RID: 2923
	private float originalAlpha;

	// Token: 0x04000B6C RID: 2924
	private Image image;

	// Token: 0x04000B6D RID: 2925
	private float changeColorTimer;
}
