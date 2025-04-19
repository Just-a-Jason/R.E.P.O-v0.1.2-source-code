using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class DirtFinderMapFloor : MonoBehaviour
{
	// Token: 0x06000CC3 RID: 3267 RVA: 0x0007029E File Offset: 0x0006E49E
	private void Start()
	{
		base.StartCoroutine(this.Add());
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x000702AD File Offset: 0x0006E4AD
	private IEnumerator Add()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Map.Instance.AddFloor(this);
		yield break;
	}

	// Token: 0x04001461 RID: 5217
	public DirtFinderMapFloor.FloorType Type;

	// Token: 0x04001462 RID: 5218
	internal MapObject MapObject;

	// Token: 0x02000356 RID: 854
	public enum FloorType
	{
		// Token: 0x04002717 RID: 10007
		Floor_1x1,
		// Token: 0x04002718 RID: 10008
		Floor_1x1_Diagonal,
		// Token: 0x04002719 RID: 10009
		Floor_1x05,
		// Token: 0x0400271A RID: 10010
		Floor_1x025,
		// Token: 0x0400271B RID: 10011
		Floor_1x05_Diagonal,
		// Token: 0x0400271C RID: 10012
		Floor_1x025_Diagonal,
		// Token: 0x0400271D RID: 10013
		Truck_Floor,
		// Token: 0x0400271E RID: 10014
		Truck_Wall,
		// Token: 0x0400271F RID: 10015
		Used_Floor,
		// Token: 0x04002720 RID: 10016
		Used_Wall,
		// Token: 0x04002721 RID: 10017
		Inactive_Floor,
		// Token: 0x04002722 RID: 10018
		Inactive_Wall,
		// Token: 0x04002723 RID: 10019
		Floor_1x1_Curve,
		// Token: 0x04002724 RID: 10020
		Floor_1x1_Curve_Inverted,
		// Token: 0x04002725 RID: 10021
		Floor_1x05_Curve,
		// Token: 0x04002726 RID: 10022
		Floor_1x05_Curve_Inverted
	}
}
