using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000185 RID: 389
public class MapCustom : MonoBehaviour
{
	// Token: 0x06000CE3 RID: 3299 RVA: 0x00071324 File Offset: 0x0006F524
	private void Start()
	{
		base.StartCoroutine(this.AddToMap());
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x00071333 File Offset: 0x0006F533
	private IEnumerator AddToMap()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Map.Instance.AddCustom(this, this.sprite, this.color);
		yield break;
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x00071342 File Offset: 0x0006F542
	public void Hide()
	{
		if (this.mapCustomEntity)
		{
			this.mapCustomEntity.Hide();
		}
	}

	// Token: 0x040014A2 RID: 5282
	public Sprite sprite;

	// Token: 0x040014A3 RID: 5283
	public Color color = new Color(0f, 1f, 0.92f);

	// Token: 0x040014A4 RID: 5284
	public MapCustomEntity mapCustomEntity;
}
