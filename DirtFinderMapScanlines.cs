using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class DirtFinderMapScanlines : MonoBehaviour
{
	// Token: 0x06000CCB RID: 3275 RVA: 0x00070319 File Offset: 0x0006E519
	private void OnEnable()
	{
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x00070328 File Offset: 0x0006E528
	private IEnumerator Logic()
	{
		for (;;)
		{
			base.transform.localPosition += new Vector3(0f, 0f, this.Speed);
			if (base.transform.localPosition.z < this.MaxZ)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, 0f);
				base.transform.localPosition += new Vector3(0f, 0f, this.Speed);
			}
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x04001466 RID: 5222
	public float Speed;

	// Token: 0x04001467 RID: 5223
	public float MaxZ;
}
