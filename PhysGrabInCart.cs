using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000197 RID: 407
public class PhysGrabInCart : MonoBehaviour
{
	// Token: 0x06000DBD RID: 3517 RVA: 0x0007CC48 File Offset: 0x0007AE48
	private void Update()
	{
		for (int i = 0; i < this.inCartObjects.Count; i++)
		{
			PhysGrabInCart.CartObject cartObject = this.inCartObjects[i];
			cartObject.timer -= Time.deltaTime;
			if (cartObject.timer <= 0f || !cartObject.physGrabObject)
			{
				this.inCartObjects.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x0007CCB4 File Offset: 0x0007AEB4
	public void Add(PhysGrabObject _physGrabObject)
	{
		foreach (PhysGrabInCart.CartObject cartObject in this.inCartObjects)
		{
			if (cartObject.physGrabObject == _physGrabObject)
			{
				cartObject.timer = 1f;
				return;
			}
		}
		PhysGrabInCart.CartObject cartObject2 = new PhysGrabInCart.CartObject();
		cartObject2.physGrabObject = _physGrabObject;
		cartObject2.timer = 1f;
		this.inCartObjects.Add(cartObject2);
	}

	// Token: 0x040016A8 RID: 5800
	public PhysGrabCart cart;

	// Token: 0x040016A9 RID: 5801
	internal List<PhysGrabInCart.CartObject> inCartObjects = new List<PhysGrabInCart.CartObject>();

	// Token: 0x0200036A RID: 874
	public class CartObject
	{
		// Token: 0x04002776 RID: 10102
		public PhysGrabObject physGrabObject;

		// Token: 0x04002777 RID: 10103
		public float timer;
	}
}
