using System;
using UnityEngine;

// Token: 0x02000261 RID: 609
public class TutorialTruckTrigger : MonoBehaviour
{
	// Token: 0x060012F0 RID: 4848 RVA: 0x000A59C3 File Offset: 0x000A3BC3
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.triggered = true;
		}
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x000A59DC File Offset: 0x000A3BDC
	private void Update()
	{
		if (this.triggered && base.GetComponent<Collider>().enabled)
		{
			this.lockLookTimer = 0.5f;
			base.GetComponent<Collider>().enabled = false;
			CameraGlitch.Instance.PlayLong();
		}
		if (this.lockLookTimer > 0f)
		{
			this.lockLookTimer -= Time.deltaTime;
			CameraAim.Instance.AimTargetSet(this.lookTarget.position + Vector3.down, 0.1f, 5f, base.gameObject, 90);
		}
		if (this.triggered)
		{
			if (this.messageDelay > 0f)
			{
				this.messageDelay -= Time.deltaTime;
				return;
			}
			if (!this.messageSent)
			{
				TruckScreenText component = this.lookTarget.GetComponent<TruckScreenText>();
				if (!component.isTyping && component.delayTimer <= 0f)
				{
					component.GotoPage(1);
				}
				this.messageSent = true;
			}
		}
	}

	// Token: 0x04001FF3 RID: 8179
	private float lockLookTimer;

	// Token: 0x04001FF4 RID: 8180
	public Transform lookTarget;

	// Token: 0x04001FF5 RID: 8181
	private float messageDelay = 1.5f;

	// Token: 0x04001FF6 RID: 8182
	private bool messageSent;

	// Token: 0x04001FF7 RID: 8183
	private bool triggered;
}
