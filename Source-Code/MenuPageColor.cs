using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000200 RID: 512
public class MenuPageColor : MonoBehaviour
{
	// Token: 0x060010D6 RID: 4310 RVA: 0x00097224 File Offset: 0x00095424
	private void Start()
	{
		this.menuPage = base.GetComponent<MenuPage>();
		List<Color> playerColors = AssetManager.instance.playerColors;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < playerColors.Count; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.colorButtonPrefab, this.colorButtonHolder);
			MenuButtonColor component = gameObject.GetComponent<MenuButtonColor>();
			MenuButton component2 = gameObject.GetComponent<MenuButton>();
			component.colorID = i;
			component.color = playerColors[i];
			component2.colorNormal = playerColors[i] + Color.black * 0.5f;
			component2.colorHover = playerColors[i];
			component2.colorClick = playerColors[i] + Color.white * 0.95f;
			RectTransform component3 = gameObject.GetComponent<RectTransform>();
			component3.SetSiblingIndex(0);
			component3.anchoredPosition = new Vector2((float)num, (float)(224 + num2));
			num += 38;
			if ((float)num > this.colorButtonHolder.rect.width)
			{
				num = 0;
				num2 -= 30;
			}
		}
		Object.Destroy(this.colorButtonPrefab);
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x00097335 File Offset: 0x00095535
	public void SetColor(int colorID, RectTransform buttonTransform)
	{
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, -1f, -1f, false);
		this.menuColorSelected.SetColor(AssetManager.instance.playerColors[colorID], buttonTransform.position);
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x0009736F File Offset: 0x0009556F
	public void ConfirmButton()
	{
		MenuManager.instance.PageReactivatePageUnderThisPage(this.menuPage);
		MenuManager.instance.MenuEffectPopUpClose();
		this.menuPage.PageStateSet(MenuPage.PageState.Closing);
	}

	// Token: 0x04001C18 RID: 7192
	public GameObject colorButtonPrefab;

	// Token: 0x04001C19 RID: 7193
	public RectTransform colorButtonHolder;

	// Token: 0x04001C1A RID: 7194
	public MenuColorSelected menuColorSelected;

	// Token: 0x04001C1B RID: 7195
	private MenuPage menuPage;
}
