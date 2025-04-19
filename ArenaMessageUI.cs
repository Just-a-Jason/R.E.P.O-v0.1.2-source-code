using System;
using TMPro;
using UnityEngine;

// Token: 0x02000246 RID: 582
public class ArenaMessageUI : SemiUI
{
	// Token: 0x06001240 RID: 4672 RVA: 0x000A0CA0 File Offset: 0x0009EEA0
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		ArenaMessageUI.instance = this;
		this.Text.text = "";
		this.originalGradient = this.Text.colorGradient;
	}

	// Token: 0x06001241 RID: 4673 RVA: 0x000A0CDC File Offset: 0x0009EEDC
	public void ArenaText(string message)
	{
		if (message != this.Text.text)
		{
			this.messageTimer = 0f;
			base.SemiUIResetAllShakeEffects();
		}
		this.messageTimer = 0.1f;
		if (message != this.messagePrev)
		{
			this.Text.text = message;
			base.SemiUISpringShakeY(5f, 5f, 0.3f);
			base.SemiUISpringScale(0.1f, 2.5f, 0.2f);
			this.messagePrev = message;
		}
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x000A0D63 File Offset: 0x0009EF63
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

	// Token: 0x04001EFA RID: 7930
	private TextMeshProUGUI Text;

	// Token: 0x04001EFB RID: 7931
	public static ArenaMessageUI instance;

	// Token: 0x04001EFC RID: 7932
	private string messagePrev = "prev";

	// Token: 0x04001EFD RID: 7933
	private float messageTimer;

	// Token: 0x04001EFE RID: 7934
	private GameObject bigMessageEmojiObject;

	// Token: 0x04001EFF RID: 7935
	private TextMeshProUGUI emojiText;

	// Token: 0x04001F00 RID: 7936
	private VertexGradient originalGradient;
}
