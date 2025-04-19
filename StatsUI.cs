using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200025E RID: 606
public class StatsUI : SemiUI
{
	// Token: 0x060012BF RID: 4799 RVA: 0x000A40B0 File Offset: 0x000A22B0
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		StatsUI.instance = this;
		this.textNumbers = base.transform.Find("StatsNumbers").GetComponent<TextMeshProUGUI>();
		this.Text.text = "";
		this.upgradesHeader = base.transform.Find("Upgrades Header").GetComponent<TextMeshProUGUI>();
		this.textNumbers.text = "";
		this.upgradesHeader.enabled = false;
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x000A4138 File Offset: 0x000A2338
	public void Fetch()
	{
		this.playerUpgrades = StatsManager.instance.FetchPlayerUpgrades(PlayerController.instance.playerSteamID);
		this.Text.text = "";
		this.textNumbers.text = "";
		this.upgradesHeader.enabled = false;
		this.scanlineObject.SetActive(false);
		foreach (KeyValuePair<string, int> keyValuePair in this.playerUpgrades)
		{
			string str = keyValuePair.Key.ToUpper();
			if (keyValuePair.Value > 0)
			{
				TextMeshProUGUI text = this.Text;
				text.text = text.text + str + "\n";
				TextMeshProUGUI textMeshProUGUI = this.textNumbers;
				textMeshProUGUI.text = textMeshProUGUI.text + "<b>" + keyValuePair.Value.ToString() + "\n</b>";
			}
		}
		if (this.Text.text != "")
		{
			this.upgradesHeader.enabled = true;
			this.scanlineObject.SetActive(true);
		}
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x000A4268 File Offset: 0x000A2468
	public void ShowStats()
	{
		base.SemiUISpringShakeY(20f, 10f, 0.3f);
		base.SemiUISpringScale(0.4f, 5f, 0.2f);
		this.showStatsTimer = 5f;
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x000A42A0 File Offset: 0x000A24A0
	protected override void Update()
	{
		base.Update();
		base.Hide();
		if (this.showStatsTimer > 0f)
		{
			this.showStatsTimer -= Time.deltaTime;
			base.Show();
		}
		if (this.showTimer > 0f)
		{
			if (!this.fetched)
			{
				this.Fetch();
				return;
			}
		}
		else
		{
			this.fetched = false;
		}
	}

	// Token: 0x04001FBC RID: 8124
	private TextMeshProUGUI Text;

	// Token: 0x04001FBD RID: 8125
	private TextMeshProUGUI textNumbers;

	// Token: 0x04001FBE RID: 8126
	private TextMeshProUGUI upgradesHeader;

	// Token: 0x04001FBF RID: 8127
	public GameObject scanlineObject;

	// Token: 0x04001FC0 RID: 8128
	public static StatsUI instance;

	// Token: 0x04001FC1 RID: 8129
	private Dictionary<string, int> playerUpgrades = new Dictionary<string, int>();

	// Token: 0x04001FC2 RID: 8130
	private float showStatsTimer;

	// Token: 0x04001FC3 RID: 8131
	private bool fetched;
}
