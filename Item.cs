using System;
using UnityEngine;

// Token: 0x02000153 RID: 339
[CreateAssetMenu(fileName = "NewItem", menuName = "Other/Item")]
public class Item : ScriptableObject
{
	// Token: 0x06000B64 RID: 2916 RVA: 0x00064B77 File Offset: 0x00062D77
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		this.itemAssetName = base.name;
		this.prefab = Resources.Load<GameObject>("Items/" + this.itemAssetName);
	}

	// Token: 0x0400125C RID: 4700
	public bool disabled;

	// Token: 0x0400125D RID: 4701
	[Space]
	public string itemAssetName;

	// Token: 0x0400125E RID: 4702
	public string itemName = "N/A";

	// Token: 0x0400125F RID: 4703
	public string description;

	// Token: 0x04001260 RID: 4704
	[Space]
	public SemiFunc.itemType itemType;

	// Token: 0x04001261 RID: 4705
	public SemiFunc.emojiIcon emojiIcon;

	// Token: 0x04001262 RID: 4706
	public SemiFunc.itemVolume itemVolume;

	// Token: 0x04001263 RID: 4707
	public SemiFunc.itemSecretShopType itemSecretShopType;

	// Token: 0x04001264 RID: 4708
	[Space]
	public ColorPresets colorPreset;

	// Token: 0x04001265 RID: 4709
	public GameObject prefab;

	// Token: 0x04001266 RID: 4710
	public Value value;

	// Token: 0x04001267 RID: 4711
	[Space]
	public int maxAmount = 1;

	// Token: 0x04001268 RID: 4712
	public int maxAmountInShop = 1;

	// Token: 0x04001269 RID: 4713
	[Space]
	public bool maxPurchase;

	// Token: 0x0400126A RID: 4714
	public int maxPurchaseAmount = 1;

	// Token: 0x0400126B RID: 4715
	[Space]
	public Quaternion spawnRotationOffset;

	// Token: 0x0400126C RID: 4716
	public bool physicalItem = true;
}
