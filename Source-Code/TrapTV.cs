using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000236 RID: 566
public class TrapTV : Trap
{
	// Token: 0x06001202 RID: 4610 RVA: 0x0009F620 File Offset: 0x0009D820
	protected override void Start()
	{
		base.Start();
		this.TVStaticOutro = false;
		this.TVBackground.enabled = false;
		this.TVStatic.enabled = false;
		this.TVStaticIntro = true;
		this.TVLight.enabled = false;
		this.CatObject.SetActive(false);
		this.MouseObject.SetActive(false);
		this.CatMaterial = this.CatObject.GetComponent<Renderer>().material;
		this.MouseMaterial = this.MouseObject.GetComponent<Renderer>().material;
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0)
		{
			this.isLocal = true;
		}
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x0009F6C8 File Offset: 0x0009D8C8
	private IEnumerator AnimationCoroutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(this.updateInterval);
			float num = this.updateInterval;
			if (!this.trapActive)
			{
				break;
			}
			float y = 0.5f;
			if (this.state == this.stateCatTalk)
			{
				y = 0f;
			}
			if (this.CatMaterial.mainTextureOffset.x < 1f)
			{
				this.CatMaterial.mainTextureOffset = new Vector2(this.CatMaterial.mainTextureOffset.x + 0.33f, y);
			}
			else
			{
				this.CatMaterial.mainTextureOffset = new Vector2(0f, y);
			}
			float y2 = 0.5f;
			if (this.state == this.stateMouseTalk)
			{
				y2 = 0f;
			}
			if (this.CatMaterial.mainTextureOffset.x < 1f)
			{
				this.MouseMaterial.mainTextureOffset = new Vector2(this.MouseMaterial.mainTextureOffset.x + 0.33f, y2);
			}
			else
			{
				this.MouseMaterial.mainTextureOffset = new Vector2(0f, y2);
			}
			if (this.state == this.stateRunning)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.runTime)
				{
					this.Timer = 0f;
					this.state = this.stateCatTalk;
					this.catTalkSound1.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
				}
			}
			if (this.state == this.stateCatTalk)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.catTalkTime)
				{
					this.Timer = 0f;
					this.state = this.stateCatTalkPause;
					this.catTalkCounter++;
				}
			}
			if (this.state == this.stateCatTalkPause)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.catTalkPauseTime)
				{
					this.Timer = 0f;
					if (this.catTalkCounter < this.catTalkCount)
					{
						this.state = this.stateCatTalk;
						this.catTalkSound2.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
					}
					else
					{
						this.catTalkCounter = 0;
						this.state = this.stateMouseTalk;
						this.mouseTalkSound1.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
					}
				}
			}
			if (this.state == this.stateMouseTalk)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.mouseTalkTime)
				{
					this.Timer = 0f;
					this.state = this.stateMouseTalkPause;
					this.mouseTalkCounter++;
				}
			}
			if (this.state == this.stateMouseTalkPause)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.mouseTalkPauseTime)
				{
					this.Timer = 0f;
					if (this.mouseTalkCounter < this.mouseTalkCount)
					{
						this.state = this.stateMouseTalk;
						this.mouseTalkSound2.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
					}
					else
					{
						this.mouseTalkCounter = 0;
						this.state = this.stateRunning;
					}
				}
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x06001204 RID: 4612 RVA: 0x0009F6D7 File Offset: 0x0009D8D7
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.trapActive = true;
			this.trapTriggered = true;
			this.TVStart = true;
			this.TVTimer.Invoke();
		}
	}

	// Token: 0x06001205 RID: 4613 RVA: 0x0009F701 File Offset: 0x0009D901
	public void TrapStop()
	{
		this.TVStaticOutro = true;
		this.TVStaticTimer = 0f;
		this.StopSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x0009F740 File Offset: 0x0009D940
	protected override void Update()
	{
		base.Update();
		this.LoopSound.PlayLoop(this.trapActive, 0.9f, 0.9f, 1f);
		if (this.trapStart)
		{
			this.TrapActivate();
		}
		if (this.trapActive)
		{
			this.enemyInvestigate = true;
			if (this.TVStart)
			{
				this.TVBackground.enabled = true;
				this.TVLight.enabled = true;
				this.TVStart = false;
				this.StartSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
				this.CatObject.SetActive(true);
				this.MouseObject.SetActive(true);
				base.StartCoroutine(this.AnimationCoroutine());
			}
			if (this.TVStaticIntro || this.TVStaticOutro)
			{
				float num = this.TVStaticCurve.Evaluate(this.TVStaticTimer / this.TVStaticTime);
				this.TVStaticTimer += 1f * Time.deltaTime * this.speedMulti;
				if (num > 0.5f)
				{
					this.TVStatic.enabled = true;
				}
				else
				{
					this.TVStatic.enabled = false;
				}
				if (this.TVStaticTimer > this.TVStaticTime)
				{
					this.TVStaticIntro = false;
					this.TVStaticTimer = 0f;
					this.TVStatic.enabled = false;
					if (this.TVStaticOutro)
					{
						this.trapActive = false;
						this.TVScreen.SetActive(false);
					}
				}
			}
		}
	}

	// Token: 0x04001E59 RID: 7769
	public GameObject TVScreen;

	// Token: 0x04001E5A RID: 7770
	public UnityEvent TVTimer;

	// Token: 0x04001E5B RID: 7771
	public float runTime = 10f;

	// Token: 0x04001E5C RID: 7772
	private float Timer;

	// Token: 0x04001E5D RID: 7773
	private float speedMulti = 1f;

	// Token: 0x04001E5E RID: 7774
	public MeshRenderer TVBackground;

	// Token: 0x04001E5F RID: 7775
	public GameObject CatObject;

	// Token: 0x04001E60 RID: 7776
	public GameObject MouseObject;

	// Token: 0x04001E61 RID: 7777
	public MeshRenderer TVStatic;

	// Token: 0x04001E62 RID: 7778
	public Light TVLight;

	// Token: 0x04001E63 RID: 7779
	public AnimationCurve TVStaticCurve;

	// Token: 0x04001E64 RID: 7780
	public float TVStaticTime = 0.5f;

	// Token: 0x04001E65 RID: 7781
	public float TVStaticTimer;

	// Token: 0x04001E66 RID: 7782
	[Space]
	[Header("___________________ Cartoon Cat Talk ___________________")]
	public float catTalkTime = 10f;

	// Token: 0x04001E67 RID: 7783
	public float catTalkPauseTime = 2f;

	// Token: 0x04001E68 RID: 7784
	public int catTalkCount = 2;

	// Token: 0x04001E69 RID: 7785
	private int catTalkCounter;

	// Token: 0x04001E6A RID: 7786
	public float catTalkStartScale = 0.2f;

	// Token: 0x04001E6B RID: 7787
	public float catTalkEndScale = 0.277f;

	// Token: 0x04001E6C RID: 7788
	public Sound catTalkSound1;

	// Token: 0x04001E6D RID: 7789
	public Sound catTalkSound2;

	// Token: 0x04001E6E RID: 7790
	[Space]
	[Header("___________________ Cartoon Mouse Talk ___________________")]
	public float mouseTalkTime = 10f;

	// Token: 0x04001E6F RID: 7791
	public float mouseTalkPauseTime = 2f;

	// Token: 0x04001E70 RID: 7792
	public int mouseTalkCount = 2;

	// Token: 0x04001E71 RID: 7793
	private int mouseTalkCounter;

	// Token: 0x04001E72 RID: 7794
	public float mouseTalkStartScale = 0.12f;

	// Token: 0x04001E73 RID: 7795
	public float mouseTalkEndScale = 0.155f;

	// Token: 0x04001E74 RID: 7796
	public Sound mouseTalkSound1;

	// Token: 0x04001E75 RID: 7797
	public Sound mouseTalkSound2;

	// Token: 0x04001E76 RID: 7798
	private int state;

	// Token: 0x04001E77 RID: 7799
	private int stateRunning;

	// Token: 0x04001E78 RID: 7800
	private int stateCatTalk = 1;

	// Token: 0x04001E79 RID: 7801
	private int stateCatTalkPause = 2;

	// Token: 0x04001E7A RID: 7802
	private int stateMouseTalk = 3;

	// Token: 0x04001E7B RID: 7803
	private int stateMouseTalkPause = 4;

	// Token: 0x04001E7C RID: 7804
	private float updateInterval = 0.083333336f;

	// Token: 0x04001E7D RID: 7805
	private bool TVStaticIntro = true;

	// Token: 0x04001E7E RID: 7806
	public bool TVStaticOutro;

	// Token: 0x04001E7F RID: 7807
	[Space]
	[Header("___________________ TV Sounds ___________________")]
	public Sound LoopSound;

	// Token: 0x04001E80 RID: 7808
	public Sound StartSound;

	// Token: 0x04001E81 RID: 7809
	public Sound StopSound;

	// Token: 0x04001E82 RID: 7810
	[HideInInspector]
	public bool TrapDone;

	// Token: 0x04001E83 RID: 7811
	private bool TVStart = true;

	// Token: 0x04001E84 RID: 7812
	private Material CatMaterial;

	// Token: 0x04001E85 RID: 7813
	private Material MouseMaterial;
}
