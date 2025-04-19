using System;
using TMPro;
using UnityEngine;

// Token: 0x02000259 RID: 601
public class MissionUI : SemiUI
{
	// Token: 0x0600129D RID: 4765 RVA: 0x000A2CC3 File Offset: 0x000A0EC3
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		MissionUI.instance = this;
		this.Text.text = "";
	}

	// Token: 0x0600129E RID: 4766 RVA: 0x000A2CF0 File Offset: 0x000A0EF0
	public void MissionText(string message, Color colorMain, Color colorFlash, float time = 3f)
	{
		if (this.messageTimer > 0f)
		{
			return;
		}
		this.bigMessageColor = colorMain;
		this.bigMessageFlashColor = colorFlash;
		this.messageTimer = time;
		message = "<b>FOCUS > </b>" + message;
		if (message != this.messagePrev)
		{
			this.Text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, this.bigMessageColor);
			this.Text.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, this.bigMessageColor);
			this.Text.color = this.bigMessageColor;
			this.Text.text = message;
			base.SemiUISpringShakeY(20f, 10f, 0.3f);
			base.SemiUITextFlashColor(this.bigMessageFlashColor, 0.2f);
			base.SemiUISpringScale(0.4f, 5f, 0.2f);
			this.messagePrev = message;
		}
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x000A2DD4 File Offset: 0x000A0FD4
	protected override void Update()
	{
		base.Update();
		if (this.messageTimer > 0f)
		{
			this.messageTimer -= Time.deltaTime;
			return;
		}
		this.messagePrev = "prev";
		base.Hide();
	}

	// Token: 0x04001F71 RID: 8049
	internal TextMeshProUGUI Text;

	// Token: 0x04001F72 RID: 8050
	public static MissionUI instance;

	// Token: 0x04001F73 RID: 8051
	private string messagePrev = "prev";

	// Token: 0x04001F74 RID: 8052
	private Color bigMessageColor = Color.white;

	// Token: 0x04001F75 RID: 8053
	private Color bigMessageFlashColor = Color.white;

	// Token: 0x04001F76 RID: 8054
	private float messageTimer;

	// Token: 0x04001F77 RID: 8055
	private GameObject bigMessageEmojiObject;

	// Token: 0x04001F78 RID: 8056
	private TextMeshProUGUI emojiText;
}
