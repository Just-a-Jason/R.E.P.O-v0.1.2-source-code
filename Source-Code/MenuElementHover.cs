using System;
using UnityEngine;

// Token: 0x020001FE RID: 510
public class MenuElementHover : MonoBehaviour
{
	// Token: 0x060010D0 RID: 4304 RVA: 0x00097004 File Offset: 0x00095204
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.menuSelectableElement = base.GetComponent<MenuSelectableElement>();
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.buttonPitch = SemiFunc.MenuGetPitchFromYPos(this.rectTransform);
		if (this.menuSelectableElement)
		{
			this.menuID = this.menuSelectableElement.menuID;
		}
	}

	// Token: 0x060010D1 RID: 4305 RVA: 0x00097064 File Offset: 0x00095264
	private void Update()
	{
		if (SemiFunc.UIMouseHover(this.parentPage, this.rectTransform, this.menuID, 0f, 0f))
		{
			if (!this.isHovering && this.hasHoverEffect)
			{
				MenuManager.instance.MenuEffectHover(this.buttonPitch, -1f);
			}
			this.isHovering = true;
		}
		else if (this.isHovering)
		{
			this.isHovering = false;
		}
		if (this.hasHoverEffect && this.isHovering)
		{
			SemiFunc.MenuSelectionBoxTargetSet(this.parentPage, this.rectTransform, default(Vector2), default(Vector2));
		}
	}

	// Token: 0x04001C08 RID: 7176
	internal bool isHovering;

	// Token: 0x04001C09 RID: 7177
	private RectTransform rectTransform;

	// Token: 0x04001C0A RID: 7178
	private MenuSelectableElement menuSelectableElement;

	// Token: 0x04001C0B RID: 7179
	private MenuPage parentPage;

	// Token: 0x04001C0C RID: 7180
	private float buttonPitch;

	// Token: 0x04001C0D RID: 7181
	public bool hasHoverEffect = true;

	// Token: 0x04001C0E RID: 7182
	internal string menuID = "-1";
}
