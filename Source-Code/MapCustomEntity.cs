using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000186 RID: 390
public class MapCustomEntity : MonoBehaviour
{
	// Token: 0x06000CE7 RID: 3303 RVA: 0x0007137E File Offset: 0x0006F57E
	public IEnumerator Logic()
	{
		while (this.Parent && this.Parent.gameObject.activeSelf)
		{
			if (Map.Instance.Active)
			{
				Map.Instance.CustomPositionSet(base.transform, this.Parent.transform);
			}
			MapLayer layerParent = Map.Instance.GetLayerParent(this.Parent.transform.position.y + 1f);
			Color color = this.spriteRenderer.color;
			if (this.mapCustomHideTimer > 0f)
			{
				this.mapCustomHideTimer -= 0.1f;
				color.a = 0f;
			}
			else if (layerParent.layer == Map.Instance.PlayerLayer)
			{
				color.a = 1f;
			}
			else
			{
				color.a = 0.3f;
			}
			this.spriteRenderer.color = color;
			yield return new WaitForSeconds(0.1f);
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x0007138D File Offset: 0x0006F58D
	public void Hide()
	{
		this.mapCustomHideTimer = 0.5f;
	}

	// Token: 0x040014A5 RID: 5285
	public Transform Parent;

	// Token: 0x040014A6 RID: 5286
	public SpriteRenderer spriteRenderer;

	// Token: 0x040014A7 RID: 5287
	public MapCustom mapCustom;

	// Token: 0x040014A8 RID: 5288
	private float mapCustomHideTimer;
}
