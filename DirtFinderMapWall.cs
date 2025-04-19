using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000183 RID: 387
public class DirtFinderMapWall : MonoBehaviour
{
	// Token: 0x06000CCE RID: 3278 RVA: 0x0007033F File Offset: 0x0006E53F
	private void Start()
	{
		base.StartCoroutine(this.Add());
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x0007034E File Offset: 0x0006E54E
	private IEnumerator Add()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Map.Instance.AddWall(this);
		yield break;
	}

	// Token: 0x04001468 RID: 5224
	public DirtFinderMapWall.WallType Type;

	// Token: 0x0200035B RID: 859
	public enum WallType
	{
		// Token: 0x04002734 RID: 10036
		Wall_1x1,
		// Token: 0x04002735 RID: 10037
		Door_1x1,
		// Token: 0x04002736 RID: 10038
		Door_1x2,
		// Token: 0x04002737 RID: 10039
		Door_Blocked,
		// Token: 0x04002738 RID: 10040
		Door_1x1_Diagonal,
		// Token: 0x04002739 RID: 10041
		Wall_1x05,
		// Token: 0x0400273A RID: 10042
		Wall_1x025,
		// Token: 0x0400273B RID: 10043
		Wall_1x1_Diagonal,
		// Token: 0x0400273C RID: 10044
		Wall_1x05_Diagonal,
		// Token: 0x0400273D RID: 10045
		Wall_1x025_Diagonal,
		// Token: 0x0400273E RID: 10046
		Door_1x05_Diagonal,
		// Token: 0x0400273F RID: 10047
		Door_1x1_Wizard,
		// Token: 0x04002740 RID: 10048
		Door_Blocked_Wizard,
		// Token: 0x04002741 RID: 10049
		Stairs,
		// Token: 0x04002742 RID: 10050
		Door_1x05,
		// Token: 0x04002743 RID: 10051
		Door_1x1_Arctic,
		// Token: 0x04002744 RID: 10052
		Door_Blocked_Arctic,
		// Token: 0x04002745 RID: 10053
		Wall_1x1_Curve,
		// Token: 0x04002746 RID: 10054
		Wall_1x05_Curve
	}
}
