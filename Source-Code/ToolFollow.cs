using System;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class ToolFollow : MonoBehaviour
{
	// Token: 0x06000737 RID: 1847 RVA: 0x00044971 File Offset: 0x00042B71
	public void Activate()
	{
		this.Active = true;
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x0004497A File Offset: 0x00042B7A
	public void Deactivate()
	{
		this.Active = false;
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x00044983 File Offset: 0x00042B83
	private void Start()
	{
		this.StartPosition = base.transform.localPosition;
		this.StartRotation = base.transform.localEulerAngles;
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x000449A8 File Offset: 0x00042BA8
	private void Update()
	{
		if (this.Active)
		{
			base.transform.localPosition = new Vector3(this.StartPosition.x, this.StartPosition.y + this.CameraBob.transform.localPosition.y * 0.1f, this.StartPosition.z);
			base.transform.localRotation = Quaternion.Euler(this.StartRotation.x + this.CameraBob.transform.localPosition.y * 25f, this.StartRotation.y + this.CameraBob.transform.localEulerAngles.y * 2f, this.StartRotation.z + this.CameraBob.transform.localEulerAngles.z * 15f);
			return;
		}
		base.transform.localPosition = this.StartPosition;
		base.transform.localRotation = Quaternion.Euler(this.StartRotation);
	}

	// Token: 0x04000CA1 RID: 3233
	public CameraBob CameraBob;

	// Token: 0x04000CA2 RID: 3234
	private Vector3 StartPosition;

	// Token: 0x04000CA3 RID: 3235
	private Vector3 StartRotation;

	// Token: 0x04000CA4 RID: 3236
	private bool Active;
}
