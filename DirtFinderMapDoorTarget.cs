using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class DirtFinderMapDoorTarget : MonoBehaviour
{
	// Token: 0x06000CBE RID: 3262 RVA: 0x00070261 File Offset: 0x0006E461
	private void Start()
	{
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x00070270 File Offset: 0x0006E470
	public IEnumerator Logic()
	{
		while (this.Target && this.Target.gameObject.activeSelf)
		{
			if (Map.Instance.Active)
			{
				Map.Instance.DoorUpdate(this.HingeTransform, this.Target.transform, this.Layer);
			}
			yield return new WaitForSeconds(0.1f);
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0400145D RID: 5213
	public Transform Target;

	// Token: 0x0400145E RID: 5214
	public Transform HingeTransform;

	// Token: 0x0400145F RID: 5215
	public MapLayer Layer;
}
