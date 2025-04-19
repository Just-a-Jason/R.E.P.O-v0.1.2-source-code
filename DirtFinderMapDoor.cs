using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200017D RID: 381
public class DirtFinderMapDoor : MonoBehaviour
{
	// Token: 0x06000CBB RID: 3259 RVA: 0x0007022F File Offset: 0x0006E42F
	public void Start()
	{
		this.Hinge = base.GetComponent<PhysGrabHinge>();
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x0007024A File Offset: 0x0006E44A
	private IEnumerator Logic()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.MapObject = Map.Instance.AddDoor(this, this.DoorPrefab);
		while (!this.Hinge.broken)
		{
			yield return new WaitForSeconds(1f);
		}
		Object.Destroy(this.MapObject);
		yield break;
	}

	// Token: 0x04001459 RID: 5209
	public Transform Target;

	// Token: 0x0400145A RID: 5210
	public GameObject DoorPrefab;

	// Token: 0x0400145B RID: 5211
	public PhysGrabHinge Hinge;

	// Token: 0x0400145C RID: 5212
	private GameObject MapObject;
}
