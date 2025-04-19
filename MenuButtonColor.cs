using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001FA RID: 506
public class MenuButtonColor : MonoBehaviour
{
	// Token: 0x060010BE RID: 4286 RVA: 0x00096A58 File Offset: 0x00094C58
	private void Start()
	{
		this.parentPage = base.GetComponentInParent<MenuPage>();
		List<Color> playerColors = AssetManager.instance.playerColors;
		this.color = playerColors[this.colorID];
		this.menuButton = base.GetComponent<MenuButton>();
		this.menuPageColor = base.GetComponentInParent<MenuPageColor>();
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x00096AB3 File Offset: 0x00094CB3
	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(0.1f);
		while (this.parentPage.currentPageState != MenuPage.PageState.Active)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (this.color == PlayerAvatar.instance.playerAvatarVisuals.color)
		{
			this.menuPageColor.SetColor(this.colorID, base.GetComponent<RectTransform>());
		}
		yield break;
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x00096AC4 File Offset: 0x00094CC4
	private void Update()
	{
		if (this.menuButton.clicked && !this.buttonClicked)
		{
			this.menuPageColor.SetColor(this.colorID, base.GetComponent<RectTransform>());
			PlayerAvatar.instance.PlayerAvatarSetColor(this.colorID);
			this.buttonClicked = true;
		}
		if (this.buttonClicked && !this.menuButton.clicked)
		{
			this.buttonClicked = false;
		}
	}

	// Token: 0x04001BED RID: 7149
	internal int colorID;

	// Token: 0x04001BEE RID: 7150
	internal Color color = Color.white;

	// Token: 0x04001BEF RID: 7151
	private MenuButton menuButton;

	// Token: 0x04001BF0 RID: 7152
	private MenuPageColor menuPageColor;

	// Token: 0x04001BF1 RID: 7153
	private MenuPage parentPage;

	// Token: 0x04001BF2 RID: 7154
	private bool buttonClicked;
}
