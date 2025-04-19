using System;
using TMPro;
using UnityEngine;

// Token: 0x02000257 RID: 599
public class ItemInfoUI : SemiUI
{
	// Token: 0x06001296 RID: 4758 RVA: 0x000A283B File Offset: 0x000A0A3B
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		ItemInfoUI.instance = this;
		this.Text.text = "";
		this.originalGradient = this.Text.colorGradient;
	}

	// Token: 0x06001297 RID: 4759 RVA: 0x000A2878 File Offset: 0x000A0A78
	public void ItemInfoText(ItemAttributes _itemAttributes, string message, bool enemy = false)
	{
		ItemAttributes currentlyLookingAtItemAttributes = PhysGrabber.instance.currentlyLookingAtItemAttributes;
		if (!PhysGrabber.instance.grabbed && _itemAttributes && currentlyLookingAtItemAttributes && currentlyLookingAtItemAttributes != _itemAttributes)
		{
			return;
		}
		if (message != this.Text.text)
		{
			this.messageTimer = 0f;
			base.SemiUIResetAllShakeEffects();
		}
		if (enemy)
		{
			VertexGradient colorGradient = new VertexGradient(new Color(1f, 0f, 0f), new Color(1f, 0f, 0f), new Color(1f, 0.1f, 0f), new Color(1f, 0.1f, 0f));
			this.Text.fontSize = 35f;
			this.Text.colorGradient = colorGradient;
		}
		else
		{
			this.Text.colorGradient = this.originalGradient;
			if (!SemiFunc.RunIsShop())
			{
				this.Text.fontSize = 15f;
			}
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

	// Token: 0x06001298 RID: 4760 RVA: 0x000A29D0 File Offset: 0x000A0BD0
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

	// Token: 0x04001F60 RID: 8032
	private TextMeshProUGUI Text;

	// Token: 0x04001F61 RID: 8033
	public static ItemInfoUI instance;

	// Token: 0x04001F62 RID: 8034
	private string messagePrev = "prev";

	// Token: 0x04001F63 RID: 8035
	private float messageTimer;

	// Token: 0x04001F64 RID: 8036
	private GameObject bigMessageEmojiObject;

	// Token: 0x04001F65 RID: 8037
	private TextMeshProUGUI emojiText;

	// Token: 0x04001F66 RID: 8038
	private VertexGradient originalGradient;
}
