using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000211 RID: 529
public class Blink : MonoBehaviour
{
	// Token: 0x0600113D RID: 4413 RVA: 0x00099ECC File Offset: 0x000980CC
	private void Update()
	{
		if (this.blinkTimer <= 0f)
		{
			if (this.targetImage.enabled)
			{
				this.targetImage.enabled = false;
			}
			else
			{
				this.targetImage.enabled = true;
			}
			this.blinkTimer = this.blinkTime;
			return;
		}
		this.blinkTimer -= Time.deltaTime;
	}

	// Token: 0x04001CEE RID: 7406
	public Image targetImage;

	// Token: 0x04001CEF RID: 7407
	public float blinkTime;

	// Token: 0x04001CF0 RID: 7408
	private float blinkTimer;
}
