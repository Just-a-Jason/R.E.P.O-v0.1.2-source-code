using System;
using TMPro;
using UnityEngine;

// Token: 0x020001DD RID: 477
public class MenuButtonHoverArea : MonoBehaviour
{
	// Token: 0x06000FEB RID: 4075 RVA: 0x0009131A File Offset: 0x0008F51A
	private void Start()
	{
		this.menuButton = base.GetComponentInParent<MenuButton>();
		this.rectTransform = base.GetComponent<RectTransform>();
	}

	// Token: 0x06000FEC RID: 4076 RVA: 0x00091334 File Offset: 0x0008F534
	private void Update()
	{
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x00091336 File Offset: 0x0008F536
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		this.rectTransform = base.GetComponent<RectTransform>();
	}

	// Token: 0x04001ABF RID: 6847
	private MenuButton menuButton;

	// Token: 0x04001AC0 RID: 6848
	public TextMeshProUGUI text;

	// Token: 0x04001AC1 RID: 6849
	private RectTransform rectTransform;
}
