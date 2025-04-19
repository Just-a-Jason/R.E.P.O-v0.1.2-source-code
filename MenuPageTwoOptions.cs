using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000204 RID: 516
public class MenuPageTwoOptions : MonoBehaviour
{
	// Token: 0x060010FC RID: 4348 RVA: 0x0009835B File Offset: 0x0009655B
	private void Start()
	{
		MenuPageTwoOptions.instance = this;
		this.menuPage = base.GetComponent<MenuPage>();
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x0009836F File Offset: 0x0009656F
	private void Update()
	{
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x00098371 File Offset: 0x00096571
	private void OnEnable()
	{
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x00098373 File Offset: 0x00096573
	private void OnDisable()
	{
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x00098375 File Offset: 0x00096575
	public void ButtonEventOption1()
	{
		if (this.option1Event != null)
		{
			this.option1Event.Invoke();
		}
		MenuManager.instance.PageReactivatePageUnderThisPage(this.menuPage);
		this.menuPage.PageStateSet(MenuPage.PageState.Closing);
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x000983A6 File Offset: 0x000965A6
	public void ButtonEventOption2()
	{
		if (this.option2Event != null)
		{
			this.option2Event.Invoke();
		}
		MenuManager.instance.PageReactivatePageUnderThisPage(this.menuPage);
		this.menuPage.PageStateSet(MenuPage.PageState.Closing);
	}

	// Token: 0x04001C3F RID: 7231
	public static MenuPageTwoOptions instance;

	// Token: 0x04001C40 RID: 7232
	internal MenuPage menuPage;

	// Token: 0x04001C41 RID: 7233
	internal UnityEvent option1Event;

	// Token: 0x04001C42 RID: 7234
	internal UnityEvent option2Event;

	// Token: 0x04001C43 RID: 7235
	public TextMeshProUGUI bodyTextMesh;

	// Token: 0x04001C44 RID: 7236
	public MenuButton option1Button;

	// Token: 0x04001C45 RID: 7237
	public MenuButton option2Button;
}
