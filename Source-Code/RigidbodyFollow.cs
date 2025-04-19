using System;
using UnityEngine;

// Token: 0x02000134 RID: 308
public class RigidbodyFollow : MonoBehaviour
{
	// Token: 0x06000A85 RID: 2693 RVA: 0x0005CDC1 File Offset: 0x0005AFC1
	private void Start()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x0005CDD0 File Offset: 0x0005AFD0
	private void FixedUpdate()
	{
		this.Rigidbody.position = this.Target.position;
		this.Rigidbody.rotation = this.Target.rotation;
		if (this.Scale)
		{
			base.transform.localScale = this.Target.localScale;
		}
	}

	// Token: 0x0400110F RID: 4367
	public Transform Target;

	// Token: 0x04001110 RID: 4368
	public bool Scale;

	// Token: 0x04001111 RID: 4369
	private Rigidbody Rigidbody;
}
