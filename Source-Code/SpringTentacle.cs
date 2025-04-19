using System;
using UnityEngine;

// Token: 0x0200007A RID: 122
public class SpringTentacle : MonoBehaviour
{
	// Token: 0x0600049B RID: 1179 RVA: 0x0002DAB5 File Offset: 0x0002BCB5
	private void Start()
	{
		this.offsetX = Random.Range(0f, 100f);
		this.offsetY = Random.Range(0f, 100f);
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0002DAE4 File Offset: 0x0002BCE4
	private void Update()
	{
		this.springStartTarget.transform.localRotation = Quaternion.Euler(Mathf.Sin(Time.time * 5f + this.offsetX) * 5f, Mathf.Sin(Time.time * 5f + this.offsetY) * 10f, 0f);
		this.springStartSource.rotation = SemiFunc.SpringQuaternionGet(this.springStart, this.springStartTarget.transform.rotation, -1f);
		this.springMidSource.rotation = SemiFunc.SpringQuaternionGet(this.springMid, this.springMidTarget.transform.rotation, -1f);
		this.springEndSource.rotation = SemiFunc.SpringQuaternionGet(this.springEnd, this.springEndTarget.transform.rotation, -1f);
	}

	// Token: 0x0400077E RID: 1918
	public SpringQuaternion springStart;

	// Token: 0x0400077F RID: 1919
	public Transform springStartTarget;

	// Token: 0x04000780 RID: 1920
	public Transform springStartSource;

	// Token: 0x04000781 RID: 1921
	[Space]
	public SpringQuaternion springMid;

	// Token: 0x04000782 RID: 1922
	public Transform springMidTarget;

	// Token: 0x04000783 RID: 1923
	public Transform springMidSource;

	// Token: 0x04000784 RID: 1924
	[Space]
	public SpringQuaternion springEnd;

	// Token: 0x04000785 RID: 1925
	public Transform springEndTarget;

	// Token: 0x04000786 RID: 1926
	public Transform springEndSource;

	// Token: 0x04000787 RID: 1927
	private float offsetX;

	// Token: 0x04000788 RID: 1928
	private float offsetY;
}
