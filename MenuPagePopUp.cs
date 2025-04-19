using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000205 RID: 517
public class MenuPagePopUp : MonoBehaviour
{
	// Token: 0x06001103 RID: 4355 RVA: 0x000983DF File Offset: 0x000965DF
	private void Start()
	{
		MenuPagePopUp.instance = this;
		this.menuPage = base.GetComponent<MenuPage>();
	}

	// Token: 0x06001104 RID: 4356 RVA: 0x000983F3 File Offset: 0x000965F3
	public void ButtonEvent()
	{
		MenuManager.instance.PageReactivatePageUnderThisPage(this.menuPage);
		MenuManager.instance.MenuEffectPopUpClose();
		this.menuPage.PageStateSet(MenuPage.PageState.Closing);
	}

	// Token: 0x04001C46 RID: 7238
	public static MenuPagePopUp instance;

	// Token: 0x04001C47 RID: 7239
	internal MenuPage menuPage;

	// Token: 0x04001C48 RID: 7240
	internal UnityEvent option1Event;

	// Token: 0x04001C49 RID: 7241
	internal UnityEvent option2Event;

	// Token: 0x04001C4A RID: 7242
	public TextMeshProUGUI bodyTextMesh;

	// Token: 0x04001C4B RID: 7243
	public MenuButton okButton;
}
