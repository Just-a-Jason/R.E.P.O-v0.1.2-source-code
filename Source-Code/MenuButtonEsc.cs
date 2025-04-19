using System;
using UnityEngine;

// Token: 0x020001DC RID: 476
public class MenuButtonEsc : MonoBehaviour
{
	// Token: 0x06000FE8 RID: 4072 RVA: 0x000912FD File Offset: 0x0008F4FD
	private void Start()
	{
		this.parentTransform = base.GetComponentInParent<MenuPage>().transform;
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x00091310 File Offset: 0x0008F510
	private void Update()
	{
	}

	// Token: 0x04001ABE RID: 6846
	private Transform parentTransform;
}
