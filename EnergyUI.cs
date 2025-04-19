using System;
using TMPro;
using UnityEngine;

// Token: 0x0200024E RID: 590
public class EnergyUI : SemiUI
{
	// Token: 0x0600125F RID: 4703 RVA: 0x000A178A File Offset: 0x0009F98A
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		EnergyUI.instance = this;
		this.textEnergyMax = base.transform.Find("EnergyMax").GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x000A17C0 File Offset: 0x0009F9C0
	protected override void Update()
	{
		base.Update();
		this.Text.text = Mathf.Ceil(PlayerController.instance.EnergyCurrent).ToString();
		this.textEnergyMax.text = "<b><color=orange>/</color></b>" + Mathf.Ceil(PlayerController.instance.EnergyStart).ToString();
	}

	// Token: 0x04001F32 RID: 7986
	private TextMeshProUGUI Text;

	// Token: 0x04001F33 RID: 7987
	public static EnergyUI instance;

	// Token: 0x04001F34 RID: 7988
	private TextMeshProUGUI textEnergyMax;
}
