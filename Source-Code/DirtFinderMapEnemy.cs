using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class DirtFinderMapEnemy : MonoBehaviour
{
	// Token: 0x06000CC1 RID: 3265 RVA: 0x00070287 File Offset: 0x0006E487
	public IEnumerator Logic()
	{
		while (this.Parent != null && this.Parent.gameObject.activeSelf)
		{
			if (Map.Instance.Active)
			{
				Map.Instance.EnemyPositionSet(base.transform, this.Parent.transform);
			}
			yield return new WaitForSeconds(0.1f);
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04001460 RID: 5216
	public Transform Parent;
}
