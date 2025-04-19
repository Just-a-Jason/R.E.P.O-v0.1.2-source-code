using System;
using UnityEngine;

// Token: 0x020001A3 RID: 419
public class PlayerAvatarMenuHover : MonoBehaviour
{
	// Token: 0x06000E05 RID: 3589 RVA: 0x0007E712 File Offset: 0x0007C912
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.menuElementHover = base.GetComponent<MenuElementHover>();
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x0007E738 File Offset: 0x0007C938
	private void Update()
	{
		Vector2 vector = SemiFunc.UIMouseGetLocalPositionWithinRectTransform(this.rectTransform);
		this.pointer.localPosition = new Vector3(vector.x * 0.98f, vector.y * 1.035f, 0f) / SemiFunc.UIMulti() * 2.23f;
		this.pointer.localPosition += new Vector3(-0.065f, -0.06f, 0f);
		this.pointer.GetComponent<MeshRenderer>().enabled = false;
		if (SemiFunc.InputHold(InputKey.Grab) && this.menuElementHover.isHovering)
		{
			if (!this.startClick)
			{
				this.startClick = true;
				this.mouseClickPos = vector;
			}
			Vector2 vector2 = (vector - this.mouseClickPos) * 25f;
			this.playerAvatarMenu.Rotate(new Vector3(0f, -vector2.x, 0f));
			return;
		}
		this.startClick = false;
	}

	// Token: 0x040016EF RID: 5871
	private RectTransform rectTransform;

	// Token: 0x040016F0 RID: 5872
	private MenuPage parentPage;

	// Token: 0x040016F1 RID: 5873
	public Transform pointer;

	// Token: 0x040016F2 RID: 5874
	private bool startClick;

	// Token: 0x040016F3 RID: 5875
	private Vector2 mouseClickPos;

	// Token: 0x040016F4 RID: 5876
	public PlayerAvatarMenu playerAvatarMenu;

	// Token: 0x040016F5 RID: 5877
	private MenuElementHover menuElementHover;
}
