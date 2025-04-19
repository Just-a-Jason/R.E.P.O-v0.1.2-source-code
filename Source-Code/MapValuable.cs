using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class MapValuable : MonoBehaviour
{
	// Token: 0x06000CF3 RID: 3315 RVA: 0x0007151A File Offset: 0x0006F71A
	private void Start()
	{
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x00071529 File Offset: 0x0006F729
	private IEnumerator Logic()
	{
		for (;;)
		{
			if (!Map.Instance.Active)
			{
				yield return new WaitForSeconds(0.25f);
			}
			else
			{
				if (!this.target)
				{
					break;
				}
				Map.Instance.CustomPositionSet(base.transform, this.target.transform);
				MapLayer layerParent = Map.Instance.GetLayerParent(this.target.transform.position.y + 1f);
				Color color = this.spriteRenderer.color;
				if (layerParent.layer == Map.Instance.PlayerLayer)
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
		}
		Object.Destroy(base.gameObject);
		yield break;
		yield break;
	}

	// Token: 0x040014B2 RID: 5298
	public ValuableObject target;

	// Token: 0x040014B3 RID: 5299
	[Space]
	public SpriteRenderer spriteRenderer;

	// Token: 0x040014B4 RID: 5300
	[Space]
	public Sprite spriteSmall;

	// Token: 0x040014B5 RID: 5301
	public Sprite spriteBig;
}
