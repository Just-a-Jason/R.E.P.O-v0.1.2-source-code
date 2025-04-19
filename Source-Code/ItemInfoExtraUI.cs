using System;
using TMPro;
using UnityEngine;

// Token: 0x02000256 RID: 598
public class ItemInfoExtraUI : SemiUI
{
	// Token: 0x06001292 RID: 4754 RVA: 0x000A26E4 File Offset: 0x000A08E4
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		ItemInfoExtraUI.instance = this;
		this.Text.text = "";
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x000A2710 File Offset: 0x000A0910
	public void ItemInfoText(string message, Color color)
	{
		if (this.messageTimer > 0f)
		{
			return;
		}
		this.messageTimer = 0.2f;
		if (message != this.messagePrev)
		{
			this.Text.text = message;
			base.SemiUISpringShakeY(20f, 10f, 0.3f);
			base.SemiUISpringScale(0.4f, 5f, 0.2f);
			this.textColor = color;
			this.Text.color = this.textColor;
			this.messagePrev = message;
		}
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x000A279C File Offset: 0x000A099C
	protected override void Update()
	{
		base.Update();
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		if (!SemiFunc.RunIsShop())
		{
			this.Text.fontSize = 12f;
		}
		if (this.messageTimer > 0f)
		{
			this.messageTimer -= Time.deltaTime;
			ItemInfoUI.instance.SemiUIScoot(new Vector2(0f, 8f));
			return;
		}
		this.Text.color = Color.white;
		this.messagePrev = "prev";
		base.Hide();
	}

	// Token: 0x04001F59 RID: 8025
	private TextMeshProUGUI Text;

	// Token: 0x04001F5A RID: 8026
	public static ItemInfoExtraUI instance;

	// Token: 0x04001F5B RID: 8027
	private string messagePrev = "prev";

	// Token: 0x04001F5C RID: 8028
	private float messageTimer;

	// Token: 0x04001F5D RID: 8029
	private GameObject bigMessageEmojiObject;

	// Token: 0x04001F5E RID: 8030
	private TextMeshProUGUI emojiText;

	// Token: 0x04001F5F RID: 8031
	private Color textColor;
}
