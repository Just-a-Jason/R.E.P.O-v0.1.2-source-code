using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000239 RID: 569
public class Aim : MonoBehaviour
{
	// Token: 0x0600120F RID: 4623 RVA: 0x0009FD51 File Offset: 0x0009DF51
	private void Awake()
	{
		Aim.instance = this;
		this.image = base.GetComponent<Image>();
		this.defaultState = this.aimStates[0];
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x0009FD78 File Offset: 0x0009DF78
	private void Update()
	{
		if (this.stateTimer > 0f)
		{
			this.stateTimer -= 1f * Time.deltaTime;
		}
		else if (this.currentState != Aim.State.Default)
		{
			this.animLerp = 0f;
			this.currentState = Aim.State.Default;
			this.currentSprite = this.defaultState.Sprite;
			this.currentColor = this.defaultState.Color;
		}
		if (this.currentState == this.previousState)
		{
			if (this.animLerp < 1f)
			{
				this.animLerp += 10f * Time.deltaTime;
				base.transform.localScale = Vector3.one * this.curveOutro.Evaluate(this.animLerp);
			}
		}
		else
		{
			this.animLerp += 15f * Time.deltaTime;
			base.transform.localScale = Vector3.one * this.curveIntro.Evaluate(this.animLerp);
			if (this.animLerp >= 1f)
			{
				this.image.sprite = this.currentSprite;
				this.image.color = this.currentColor;
				this.previousState = this.currentState;
				this.animLerp = 0f;
			}
		}
		if (this.previousState == this.currentState)
		{
			if (this.currentState == Aim.State.Rotate)
			{
				base.transform.localRotation = Quaternion.Euler(0f, 0f, base.transform.localRotation.eulerAngles.z - 100f * Time.deltaTime);
				return;
			}
			base.transform.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x0009FF34 File Offset: 0x0009E134
	public void SetState(Aim.State _state)
	{
		if (_state == this.currentState)
		{
			this.stateTimer = 0.25f;
			return;
		}
		foreach (Aim.AimState aimState in this.aimStates)
		{
			if (aimState.State == _state)
			{
				this.currentState = aimState.State;
				this.currentSprite = aimState.Sprite;
				this.currentColor = aimState.Color;
				this.animLerp = 0f;
				this.stateTimer = 0.2f;
				break;
			}
		}
	}

	// Token: 0x04001EBC RID: 7868
	public static Aim instance;

	// Token: 0x04001EBD RID: 7869
	[Space]
	public AnimationCurve curveIntro;

	// Token: 0x04001EBE RID: 7870
	public AnimationCurve curveOutro;

	// Token: 0x04001EBF RID: 7871
	private float animLerp;

	// Token: 0x04001EC0 RID: 7872
	[Space]
	public List<Aim.AimState> aimStates;

	// Token: 0x04001EC1 RID: 7873
	private Aim.AimState defaultState;

	// Token: 0x04001EC2 RID: 7874
	private Image image;

	// Token: 0x04001EC3 RID: 7875
	private float stateTimer;

	// Token: 0x04001EC4 RID: 7876
	private Aim.State currentState;

	// Token: 0x04001EC5 RID: 7877
	private Aim.State previousState;

	// Token: 0x04001EC6 RID: 7878
	private Sprite currentSprite;

	// Token: 0x04001EC7 RID: 7879
	private Color currentColor;

	// Token: 0x020003B1 RID: 945
	public enum State
	{
		// Token: 0x04002897 RID: 10391
		Default,
		// Token: 0x04002898 RID: 10392
		Grabbable,
		// Token: 0x04002899 RID: 10393
		Grab,
		// Token: 0x0400289A RID: 10394
		Rotate,
		// Token: 0x0400289B RID: 10395
		Hidden
	}

	// Token: 0x020003B2 RID: 946
	[Serializable]
	public class AimState
	{
		// Token: 0x0400289C RID: 10396
		public Aim.State State;

		// Token: 0x0400289D RID: 10397
		public Sprite Sprite;

		// Token: 0x0400289E RID: 10398
		public Color Color;
	}
}
