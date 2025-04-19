using System;
using TMPro;
using UnityEngine;

// Token: 0x02000251 RID: 593
public class HealthUI : SemiUI
{
	// Token: 0x06001268 RID: 4712 RVA: 0x000A1A5D File Offset: 0x0009FC5D
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		HealthUI.instance = this;
		this.textMaxHealth = base.transform.Find("HealthMax").GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x000A1A94 File Offset: 0x0009FC94
	protected override void Update()
	{
		base.Update();
		if (this.playerHealth)
		{
			this.playerHealthValue = this.playerHealth.health;
			if (this.playerHealthValue != this.playerHealthPrevious)
			{
				base.SemiUISpringShakeY(20f, 10f, 0.3f);
				Color color = Color.white;
				if (this.playerHealthValue < this.playerHealthPrevious)
				{
					color = Color.red;
				}
				base.SemiUITextFlashColor(color, 0.2f);
				base.SemiUISpringScale(0.3f, 5f, 0.2f);
				this.playerHealthPrevious = this.playerHealthValue;
			}
		}
		if (this.setup)
		{
			if (LevelGenerator.Instance.Generated)
			{
				this.playerHealth = PlayerController.instance.playerAvatarScript.playerHealth;
				this.setup = false;
				return;
			}
		}
		else
		{
			this.Text.text = this.playerHealthValue.ToString();
			this.textMaxHealth.text = "<b><color=#008b20>/</color></b>" + this.playerHealth.maxHealth.ToString();
		}
	}

	// Token: 0x04001F3B RID: 7995
	private TextMeshProUGUI Text;

	// Token: 0x04001F3C RID: 7996
	private PlayerHealth playerHealth;

	// Token: 0x04001F3D RID: 7997
	private bool setup = true;

	// Token: 0x04001F3E RID: 7998
	public static HealthUI instance;

	// Token: 0x04001F3F RID: 7999
	private int playerHealthValue;

	// Token: 0x04001F40 RID: 8000
	private int playerHealthPrevious;

	// Token: 0x04001F41 RID: 8001
	private TextMeshProUGUI textMaxHealth;
}
