using System;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class EnemyHeadLoop : MonoBehaviour
{
	// Token: 0x0600030D RID: 781 RVA: 0x0001E274 File Offset: 0x0001C474
	private void Update()
	{
		if (this.Enemy.PlayerRoom.SameLocal || this.Enemy.OnScreen.OnScreenLocal)
		{
			if (!this.Active)
			{
				this.AudioSource.Play();
				this.Active = true;
			}
			if (this.AudioSource.volume < this.VolumeMax)
			{
				this.AudioSource.volume += this.FadeInSpeed * Time.deltaTime;
				this.AudioSource.volume = Mathf.Min(this.AudioSource.volume, this.VolumeMax);
				return;
			}
		}
		else if (this.Active && this.AudioSource.volume > 0f)
		{
			this.AudioSource.volume -= this.FadeOutSpeed * Time.deltaTime;
			if (this.AudioSource.volume <= 0f)
			{
				this.AudioSource.Stop();
				this.Active = false;
			}
		}
	}

	// Token: 0x04000551 RID: 1361
	public Enemy Enemy;

	// Token: 0x04000552 RID: 1362
	public AudioSource AudioSource;

	// Token: 0x04000553 RID: 1363
	[Space]
	public float VolumeMax;

	// Token: 0x04000554 RID: 1364
	public float FadeInSpeed;

	// Token: 0x04000555 RID: 1365
	public float FadeOutSpeed;

	// Token: 0x04000556 RID: 1366
	private bool Active;
}
