using System;

// Token: 0x02000255 RID: 597
public class InventoryUI : SemiUI
{
	// Token: 0x0600128E RID: 4750 RVA: 0x000A26A3 File Offset: 0x000A08A3
	private void Awake()
	{
		InventoryUI.instance = this;
	}

	// Token: 0x0600128F RID: 4751 RVA: 0x000A26AB File Offset: 0x000A08AB
	protected override void Start()
	{
		base.Start();
		this.uiText = null;
	}

	// Token: 0x06001290 RID: 4752 RVA: 0x000A26BA File Offset: 0x000A08BA
	protected override void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		base.Update();
		if (SemiFunc.RunIsShop())
		{
			base.Hide();
		}
	}

	// Token: 0x04001F58 RID: 8024
	public static InventoryUI instance;
}
