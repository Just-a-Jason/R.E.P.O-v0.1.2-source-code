using System;
using UnityEngine;

// Token: 0x02000248 RID: 584
public class ArrowUI : MonoBehaviour
{
	// Token: 0x06001248 RID: 4680 RVA: 0x000A0F59 File Offset: 0x0009F159
	private void Awake()
	{
		ArrowUI.instance = this;
		this.arrowMesh.enabled = false;
		this.mainCamera = Camera.main;
	}

	// Token: 0x06001249 RID: 4681 RVA: 0x000A0F78 File Offset: 0x0009F178
	public void ArrowShow(Vector3 startPos, Vector3 endPos, float rotation)
	{
		if (this.endPosition != endPos)
		{
			this.arrowCurveMoveEval = 0f;
			base.transform.localPosition = startPos;
		}
		startPos.z = 0f;
		endPos.z = 0f;
		this.targetWorldPos = false;
		this.startPosition = startPos;
		this.endPosition = endPos;
		this.endRotation = rotation;
		this.showArrowTimer = 0.2f;
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x000A0FEC File Offset: 0x0009F1EC
	public void ArrowShowWorldPos(Vector3 startPos, Vector3 endPos, float rotation)
	{
		if (this.endPosition != endPos)
		{
			this.arrowCurveMoveEval = 0f;
			base.transform.position = startPos;
		}
		this.targetWorldPos = true;
		this.startPosition = startPos;
		this.endPosition = endPos;
		this.endRotation = rotation;
		this.showArrowTimer = 0.2f;
	}

	// Token: 0x0600124B RID: 4683 RVA: 0x000A1048 File Offset: 0x0009F248
	private void Update()
	{
		if (this.targetWorldPos)
		{
			this.endPosition = this.mainCamera.WorldToScreenPoint(this.endPosition).normalized;
			this.endPosition.z = 0f;
		}
		this.bopEval += Time.deltaTime;
		this.bopEval = Mathf.Clamp01(this.bopEval);
		float num = this.arrowCurveBop.Evaluate(this.bopEval);
		this.arrowMesh.transform.localPosition = new Vector3(-51f + -30f * num, 0f, 0f);
		if (this.bopEval >= 1f)
		{
			this.bopEval = 0f;
		}
		if (this.showArrowTimer > 0f)
		{
			this.arrowMesh.enabled = true;
			this.endShow = false;
			this.showArrowTimer -= Time.deltaTime;
			this.arrowCurveMoveEval += Time.deltaTime;
			this.arrowCurveMoveEval = Mathf.Clamp01(this.arrowCurveMoveEval);
			float t = this.arrowCurveMove.Evaluate(this.arrowCurveMoveEval);
			base.transform.localPosition = Vector3.LerpUnclamped(this.startPosition, this.endPosition, t);
			base.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(90f, this.endRotation, t));
			float num2 = this.arrowCurveMove.Evaluate(this.arrowCurveMoveEval * 2f);
			base.transform.localScale = new Vector3(num2, num2, num2);
			return;
		}
		if (!this.endShow)
		{
			this.arrowCurveMoveEval = 0f;
			this.endShow = true;
		}
		if (this.arrowCurveMoveEval >= 1f)
		{
			this.arrowMesh.enabled = false;
			this.arrowCurveMoveEval = 1f;
			return;
		}
		this.arrowCurveMoveEval += Time.deltaTime * 4f;
		float num3 = this.arrowCurveMove.Evaluate(this.arrowCurveMoveEval);
		base.transform.localScale = new Vector3(1f - num3, 1f - num3, 1f - num3);
		this.startPosition = Vector3.zero;
		this.endPosition = Vector3.one;
	}

	// Token: 0x04001F0A RID: 7946
	public static ArrowUI instance;

	// Token: 0x04001F0B RID: 7947
	public AnimationCurve arrowCurveMove;

	// Token: 0x04001F0C RID: 7948
	private float arrowCurveMoveEval;

	// Token: 0x04001F0D RID: 7949
	public AnimationCurve arrowCurveBop;

	// Token: 0x04001F0E RID: 7950
	private float showArrowTimer;

	// Token: 0x04001F0F RID: 7951
	private Vector3 startPosition;

	// Token: 0x04001F10 RID: 7952
	private Vector3 endPosition;

	// Token: 0x04001F11 RID: 7953
	private float endRotation;

	// Token: 0x04001F12 RID: 7954
	private bool endShow;

	// Token: 0x04001F13 RID: 7955
	public MeshRenderer arrowMesh;

	// Token: 0x04001F14 RID: 7956
	private float bopEval;

	// Token: 0x04001F15 RID: 7957
	private bool targetWorldPos;

	// Token: 0x04001F16 RID: 7958
	private Camera mainCamera;
}
