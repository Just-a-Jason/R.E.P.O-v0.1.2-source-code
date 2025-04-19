using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001FF RID: 511
public class MenuElementSaveFile : MonoBehaviour
{
	// Token: 0x060010D3 RID: 4307 RVA: 0x0009711E File Offset: 0x0009531E
	private void Start()
	{
		this.menuElementHover = base.GetComponent<MenuElementHover>();
		this.initialFadeAlpha = this.fadePanel.color.a;
		this.parentPageSaves = base.GetComponentInParent<MenuPageSaves>();
	}

	// Token: 0x060010D4 RID: 4308 RVA: 0x00097150 File Offset: 0x00095350
	private void Update()
	{
		if (this.menuElementHover.isHovering)
		{
			Color color = this.fadePanel.color;
			color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * 10f);
			this.fadePanel.color = color;
			if (SemiFunc.InputDown(InputKey.Confirm) || SemiFunc.InputDown(InputKey.Grab))
			{
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, -1f, -1f, false);
				this.parentPageSaves.SaveFileSelected(this.saveFileName);
				return;
			}
		}
		else
		{
			Color color2 = this.fadePanel.color;
			color2.a = Mathf.Lerp(color2.a, this.initialFadeAlpha, Time.deltaTime * 10f);
			this.fadePanel.color = color2;
		}
	}

	// Token: 0x04001C0F RID: 7183
	public Image fadePanel;

	// Token: 0x04001C10 RID: 7184
	private MenuElementHover menuElementHover;

	// Token: 0x04001C11 RID: 7185
	private float initialFadeAlpha;

	// Token: 0x04001C12 RID: 7186
	private MenuPageSaves parentPageSaves;

	// Token: 0x04001C13 RID: 7187
	internal string saveFileName;

	// Token: 0x04001C14 RID: 7188
	public TextMeshProUGUI saveFileHeader;

	// Token: 0x04001C15 RID: 7189
	public TextMeshProUGUI saveFileHeaderLevel;

	// Token: 0x04001C16 RID: 7190
	public TextMeshProUGUI saveFileHeaderDate;

	// Token: 0x04001C17 RID: 7191
	public TextMeshProUGUI saveFileInfoRow1;
}
