using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000154 RID: 340
public class ItemVolume : MonoBehaviour
{
	// Token: 0x06000B66 RID: 2918 RVA: 0x00064BD7 File Offset: 0x00062DD7
	private void Start()
	{
		this.itemAttributes = base.GetComponentInParent<ItemAttributes>();
		if (this.itemAttributes)
		{
			base.gameObject.tag = "Untagged";
		}
		if (SemiFunc.IsNotMasterClient())
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x00064C10 File Offset: 0x00062E10
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		ItemAttributes componentInParent = base.GetComponentInParent<ItemAttributes>();
		if (componentInParent)
		{
			if (this.itemVolume != componentInParent.item.itemVolume)
			{
				this.itemVolume = componentInParent.item.itemVolume;
			}
			string text = "Item Volume " + this.itemVolume.ToString();
			if (base.gameObject.name != text)
			{
				base.gameObject.name = text;
			}
		}
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x00064C94 File Offset: 0x00062E94
	private void OnDrawGizmos()
	{
		ItemAttributes componentInParent = base.GetComponentInParent<ItemAttributes>();
		int num = 0;
		foreach (GameObject gameObject in this.volumes)
		{
			if (this.itemVolume == (SemiFunc.itemVolume)num)
			{
				Color yellow = Color.yellow;
				Gizmos.color = yellow;
				Gizmos.matrix = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.localScale);
				Gizmos.DrawWireCube(new Vector3(0f, 0f, 0f), Vector3.one);
				yellow.a = 0.5f;
				Gizmos.color = yellow;
				if (!componentInParent)
				{
					Gizmos.DrawCube(Vector3.zero, Vector3.one);
				}
				Gizmos.matrix = Matrix4x4.identity;
			}
			num++;
		}
	}

	// Token: 0x0400126D RID: 4717
	public SemiFunc.itemVolume itemVolume;

	// Token: 0x0400126E RID: 4718
	public SemiFunc.itemSecretShopType itemSecretShopType;

	// Token: 0x0400126F RID: 4719
	public List<GameObject> volumes = new List<GameObject>();

	// Token: 0x04001270 RID: 4720
	private ItemAttributes itemAttributes;
}
