using System;
using UnityEngine;

// Token: 0x020001E9 RID: 489
public class MenuScrollBox : MonoBehaviour
{
	// Token: 0x06001064 RID: 4196 RVA: 0x00093E30 File Offset: 0x00092030
	private void Start()
	{
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.menuSelectableElement = this.scrollBarBackground.GetComponent<MenuSelectableElement>();
		this.scrollHandleTargetPosition = this.scrollHandle.localPosition.y;
		float num = 0f;
		foreach (object obj in this.scroller)
		{
			RectTransform rectTransform = (RectTransform)obj;
			float num2 = rectTransform.rect.height * rectTransform.pivot.y;
			if (rectTransform.localPosition.y - num2 < num)
			{
				num = rectTransform.localPosition.y - num2;
			}
		}
		this.scrollHeight = Mathf.Abs(num) + this.heightPadding;
		this.scrollerStartPosition = this.scrollHeight + 42f;
		this.scrollerEndPosition = this.scroller.localPosition.y;
		bool flag = true;
		if (this.scrollHeight < this.scrollBarBackground.rect.height)
		{
			this.scrollBar.SetActive(false);
			flag = false;
		}
		if (flag)
		{
			this.parentPage.scrollBoxes++;
		}
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x00093F78 File Offset: 0x00092178
	private void Update()
	{
		if (this.parentPage.scrollBoxes > 1)
		{
			if (this.menuElementHover.isHovering)
			{
				this.scrollBoxActive = true;
			}
			else
			{
				this.scrollBoxActive = false;
			}
		}
		if (!this.scrollBar.activeSelf)
		{
			return;
		}
		if (!this.scrollBoxActive)
		{
			return;
		}
		if (Input.GetMouseButton(0) && SemiFunc.UIMouseHover(this.parentPage, this.scrollBarBackground, this.menuSelectableElement.menuID, 0f, 0f))
		{
			float num = SemiFunc.UIMouseGetLocalPositionWithinRectTransform(this.scrollBarBackground).y;
			if (num < this.scrollHandle.sizeDelta.y / 2f)
			{
				num = this.scrollHandle.sizeDelta.y / 2f;
			}
			if (num > this.scrollBarBackground.rect.height - this.scrollHandle.sizeDelta.y / 2f)
			{
				num = this.scrollBarBackground.rect.height - this.scrollHandle.sizeDelta.y / 2f;
			}
			this.scrollHandleTargetPosition = num;
		}
		if (SemiFunc.InputMovementY() != 0f || SemiFunc.InputScrollY() != 0f)
		{
			this.scrollHandleTargetPosition += SemiFunc.InputMovementY() * 20f / (this.scrollHeight * 0.01f);
			this.scrollHandleTargetPosition += SemiFunc.InputScrollY() / (this.scrollHeight * 0.01f);
			if (this.scrollHandleTargetPosition < this.scrollHandle.sizeDelta.y / 2f)
			{
				this.scrollHandleTargetPosition = this.scrollHandle.sizeDelta.y / 2f;
			}
			if (this.scrollHandleTargetPosition > this.scrollBarBackground.rect.height - this.scrollHandle.sizeDelta.y / 2f)
			{
				this.scrollHandleTargetPosition = this.scrollBarBackground.rect.height - this.scrollHandle.sizeDelta.y / 2f;
			}
		}
		this.scrollHandle.localPosition = new Vector3(this.scrollHandle.localPosition.x, Mathf.Lerp(this.scrollHandle.localPosition.y, this.scrollHandleTargetPosition, Time.deltaTime * 20f), this.scrollHandle.localPosition.z);
		this.scrollAmount = this.scrollHandle.localPosition.y / this.scrollBarBackground.rect.height * 1.1f;
		if (this.scrollAmount < 0f)
		{
			this.scrollAmount = 0f;
		}
		if (this.scrollAmount > 1f)
		{
			this.scrollAmount = 1f;
		}
		this.scroller.localPosition = new Vector3(this.scroller.localPosition.x, Mathf.Lerp(this.scrollerStartPosition, this.scrollerEndPosition, this.scrollAmount), this.scroller.localPosition.z);
	}

	// Token: 0x04001B61 RID: 7009
	public RectTransform scrollSize;

	// Token: 0x04001B62 RID: 7010
	public RectTransform scroller;

	// Token: 0x04001B63 RID: 7011
	public RectTransform scrollHandle;

	// Token: 0x04001B64 RID: 7012
	public RectTransform scrollBarBackground;

	// Token: 0x04001B65 RID: 7013
	public GameObject scrollBar;

	// Token: 0x04001B66 RID: 7014
	internal float scrollAmount;

	// Token: 0x04001B67 RID: 7015
	private float scrollAmountTarget;

	// Token: 0x04001B68 RID: 7016
	private float scrollHeight;

	// Token: 0x04001B69 RID: 7017
	internal MenuPage parentPage;

	// Token: 0x04001B6A RID: 7018
	private MenuSelectableElement menuSelectableElement;

	// Token: 0x04001B6B RID: 7019
	public MenuSelectionBox menuSelectionBox;

	// Token: 0x04001B6C RID: 7020
	internal float scrollerStartPosition;

	// Token: 0x04001B6D RID: 7021
	internal float scrollerEndPosition;

	// Token: 0x04001B6E RID: 7022
	private float scrollHandleTargetPosition;

	// Token: 0x04001B6F RID: 7023
	public MenuElementHover menuElementHover;

	// Token: 0x04001B70 RID: 7024
	internal bool scrollBoxActive = true;

	// Token: 0x04001B71 RID: 7025
	public float heightPadding;
}
