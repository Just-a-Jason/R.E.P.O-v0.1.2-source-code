using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001EC RID: 492
public class MenuSelectionBoxTop : MonoBehaviour
{
	// Token: 0x0600106A RID: 4202 RVA: 0x00094364 File Offset: 0x00092564
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.rawImage = base.GetComponentInChildren<RawImage>();
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x00094380 File Offset: 0x00092580
	private void Update()
	{
		MenuSelectionBox activeSelectionBox = MenuManager.instance.activeSelectionBox;
		if (activeSelectionBox)
		{
			this.rectTransform.localPosition = activeSelectionBox.rectTransform.position - base.transform.parent.position;
			base.transform.localScale = activeSelectionBox.rectTransform.localScale;
			this.rawImage.color = activeSelectionBox.rawImage.color * 1.5f;
			this.fadeDone = false;
			return;
		}
		if (!this.fadeDone)
		{
			this.rawImage.color = new Color(1f, 1f, 1f, this.rawImage.color.a - Time.deltaTime);
			if (this.rawImage.color.a <= 0f)
			{
				this.fadeDone = true;
			}
		}
	}

	// Token: 0x04001B77 RID: 7031
	private RectTransform rectTransform;

	// Token: 0x04001B78 RID: 7032
	private RawImage rawImage;

	// Token: 0x04001B79 RID: 7033
	private bool fadeDone;
}
