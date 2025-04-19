using System;
using UnityEngine;

// Token: 0x020001EB RID: 491
public class MenuSelectableElement : MonoBehaviour
{
	// Token: 0x06001068 RID: 4200 RVA: 0x000942AC File Offset: 0x000924AC
	private void Start()
	{
		this.menuID = SemiFunc.MenuGetSelectableID(base.gameObject);
		this.rectTransform = base.GetComponent<RectTransform>();
		this.parentPage = base.GetComponentInParent<MenuPage>();
		if (this.parentPage)
		{
			this.parentPage.selectableElements.Add(this);
			if (this.rectTransform.localPosition.y < this.parentPage.bottomElementYPos)
			{
				this.parentPage.bottomElementYPos = this.rectTransform.localPosition.y;
			}
		}
		this.isInScrollBox = false;
		MenuScrollBox componentInParent = base.GetComponentInParent<MenuScrollBox>();
		if (componentInParent)
		{
			this.isInScrollBox = true;
			this.menuScrollBox = componentInParent;
		}
	}

	// Token: 0x04001B72 RID: 7026
	internal string menuID;

	// Token: 0x04001B73 RID: 7027
	internal RectTransform rectTransform;

	// Token: 0x04001B74 RID: 7028
	internal MenuPage parentPage;

	// Token: 0x04001B75 RID: 7029
	internal bool isInScrollBox;

	// Token: 0x04001B76 RID: 7030
	internal MenuScrollBox menuScrollBox;
}
