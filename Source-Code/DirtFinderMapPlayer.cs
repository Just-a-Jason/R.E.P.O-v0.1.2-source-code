using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class DirtFinderMapPlayer : MonoBehaviour
{
	// Token: 0x06000CC6 RID: 3270 RVA: 0x000702C4 File Offset: 0x0006E4C4
	private void Awake()
	{
		DirtFinderMapPlayer.Instance = this;
		this.StartOffset = base.transform.position;
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x000702DD File Offset: 0x0006E4DD
	private void OnEnable()
	{
		this.PlayerTransform = null;
		base.StartCoroutine(this.FindPlayer());
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x000702F3 File Offset: 0x0006E4F3
	private IEnumerator FindPlayer()
	{
		yield return new WaitForSeconds(0.1f);
		while (!this.PlayerTransform)
		{
			if (PlayerController.instance)
			{
				this.PlayerTransform = PlayerController.instance.transform;
				base.StartCoroutine(this.Logic());
			}
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00070302 File Offset: 0x0006E502
	private IEnumerator Logic()
	{
		for (;;)
		{
			base.transform.position = this.PlayerTransform.transform.position * Map.Instance.Scale + Map.Instance.OverLayerParent.position + this.StartOffset;
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, 0f, base.transform.localPosition.z);
			base.transform.rotation = this.PlayerTransform.rotation;
			MapLayer layerParent = Map.Instance.GetLayerParent(this.PlayerTransform.position.y + 0.01f);
			Map.Instance.PlayerLayer = layerParent.layer;
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x04001463 RID: 5219
	public static DirtFinderMapPlayer Instance;

	// Token: 0x04001464 RID: 5220
	private Transform PlayerTransform;

	// Token: 0x04001465 RID: 5221
	private Vector3 StartOffset;
}
