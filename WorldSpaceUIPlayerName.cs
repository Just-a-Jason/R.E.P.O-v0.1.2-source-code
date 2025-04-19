using System;
using TMPro;
using UnityEngine;

// Token: 0x0200026E RID: 622
public class WorldSpaceUIPlayerName : WorldSpaceUIChild
{
	// Token: 0x06001349 RID: 4937 RVA: 0x000A8FDB File Offset: 0x000A71DB
	private void OnDisable()
	{
		this.text.color = new Color(1f, 1f, 1f, 0f);
	}

	// Token: 0x0600134A RID: 4938 RVA: 0x000A9004 File Offset: 0x000A7204
	protected override void Update()
	{
		base.Update();
		if (!this.playerAvatar)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (this.showTimeTotalResetTimer > 0f)
		{
			this.showTimeTotalResetTimer -= Time.deltaTime;
			if (this.showTimeTotalResetTimer <= 0f)
			{
				this.showTimeTotal = 0f;
			}
		}
		if (SpectateCamera.instance || this.playerAvatar.isDisabled || this.showTimer <= 0f)
		{
			this.text.color = Color.Lerp(this.text.color, new Color(1f, 1f, 1f, 0f), Time.deltaTime * 20f);
		}
		else
		{
			this.showTimer -= Time.deltaTime;
			this.text.color = Color.Lerp(this.text.color, new Color(1f, 1f, 1f, 0.5f), Time.deltaTime * 5f);
		}
		Vector3 position = this.playerAvatar.playerAvatarVisuals.headLookAtTransform.position;
		position.y = this.playerAvatar.playerAvatarVisuals.transform.position.y;
		if (this.playerAvatar == SessionManager.instance.CrownedPlayerGet())
		{
			position.y += 0.02f;
		}
		this.followTarget = Vector3.Lerp(this.followTarget, position, Time.deltaTime * 30f);
		float num = this.playerAvatar.playerAvatarVisuals.headLookAtTransform.position.y - this.playerAvatar.playerAvatarVisuals.transform.position.y + 0.35f;
		if (Mathf.Abs(this.followTargetY - num) > 0.2f)
		{
			this.followTargetY = Mathf.Lerp(this.followTargetY, num, Time.deltaTime * 20f);
		}
		else
		{
			this.followTargetY = Mathf.Lerp(this.followTargetY, num, Time.deltaTime * 3f);
		}
		this.worldPosition = this.followTarget + Vector3.up * this.followTargetY;
		float num2 = Vector3.Distance(this.worldPosition, Camera.main.transform.position);
		this.text.fontSize = 20f - num2;
	}

	// Token: 0x0600134B RID: 4939 RVA: 0x000A9274 File Offset: 0x000A7474
	public void Show()
	{
		this.showTimeTotal += 0.25f;
		this.showTimeTotalResetTimer = 0.5f;
		if (this.showTimeTotal >= 1f)
		{
			this.showTimer = 0.5f;
		}
	}

	// Token: 0x040020D5 RID: 8405
	public TextMeshProUGUI text;

	// Token: 0x040020D6 RID: 8406
	internal PlayerAvatar playerAvatar;

	// Token: 0x040020D7 RID: 8407
	private Vector3 followTarget;

	// Token: 0x040020D8 RID: 8408
	private float followTargetY;

	// Token: 0x040020D9 RID: 8409
	private float showTimer;

	// Token: 0x040020DA RID: 8410
	private float showTimeTotal;

	// Token: 0x040020DB RID: 8411
	private float showTimeTotalResetTimer;
}
