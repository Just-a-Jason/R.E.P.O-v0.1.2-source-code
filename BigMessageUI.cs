using System;
using TMPro;
using UnityEngine;

// Token: 0x02000249 RID: 585
public class BigMessageUI : SemiUI
{
	// Token: 0x0600124D RID: 4685 RVA: 0x000A1294 File Offset: 0x0009F494
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		BigMessageUI.instance = this;
		this.bigMessageEmojiObject = base.transform.Find("Big Message Emoji").gameObject;
		this.emojiText = this.bigMessageEmojiObject.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x0600124E RID: 4686 RVA: 0x000A12E8 File Offset: 0x0009F4E8
	public void BigMessage(string message, string emoji, float size, Color colorMain, Color colorFlash)
	{
		this.bigMessageColor = colorMain;
		this.bigMessageFlashColor = colorFlash;
		this.bigMessageTimer = 0.2f;
		this.bigMessage = message;
		if (this.bigMessage != this.bigMessagePrev)
		{
			this.Text.fontSize = size;
			this.Text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, this.bigMessageColor);
			this.Text.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, this.bigMessageColor);
			this.Text.color = this.bigMessageColor;
			this.Text.text = this.bigMessage;
			this.bigMessageEmoji = SemiFunc.EmojiText(emoji);
			this.emojiText.text = this.bigMessageEmoji;
			base.SemiUISpringShakeY(20f, 10f, 0.3f);
			base.SemiUITextFlashColor(this.bigMessageFlashColor, 0.2f);
			base.SemiUISpringScale(0.4f, 5f, 0.2f);
			this.bigMessagePrev = this.bigMessage;
		}
	}

	// Token: 0x0600124F RID: 4687 RVA: 0x000A13F8 File Offset: 0x0009F5F8
	protected override void Update()
	{
		base.Update();
		this.bigMessageEmojiObject.SetActive(this.Text.enabled);
		if (this.bigMessageTimer > 0f)
		{
			this.bigMessageTimer -= Time.deltaTime;
			return;
		}
		this.bigMessage = "big";
		this.bigMessagePrev = "prev";
		base.Hide();
	}

	// Token: 0x04001F17 RID: 7959
	private TextMeshProUGUI Text;

	// Token: 0x04001F18 RID: 7960
	public static BigMessageUI instance;

	// Token: 0x04001F19 RID: 7961
	private string bigMessagePrev = "prev";

	// Token: 0x04001F1A RID: 7962
	private string bigMessage = "big";

	// Token: 0x04001F1B RID: 7963
	private Color bigMessageColor = Color.white;

	// Token: 0x04001F1C RID: 7964
	private Color bigMessageFlashColor = Color.white;

	// Token: 0x04001F1D RID: 7965
	private float bigMessageTimer;

	// Token: 0x04001F1E RID: 7966
	private string bigMessageEmoji = "";

	// Token: 0x04001F1F RID: 7967
	private GameObject bigMessageEmojiObject;

	// Token: 0x04001F20 RID: 7968
	private TextMeshProUGUI emojiText;
}
