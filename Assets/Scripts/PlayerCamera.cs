using Avelog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCamera : MonoBehaviour, IInitializablePlayerComponent
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass36_0
	{
		public Vector3 tempCurPos;

		public PlayerCamera _003C_003E4__this;

		public Quaternion tempCurRot;

		public float alignLerp;

		public float moveLerp;

		public float rotLerp;

		private void _003CUpdateCamTransform_003Eg__SaveTempPos_007C0()
		{
			tempCurPos = _003C_003E4__this.transform.position;
			tempCurRot = _003C_003E4__this.transform.rotation;
		}

		private void _003CUpdateCamTransform_003Eg__CalcNextPos_007C1(Vector3 inputPos, Quaternion inputRot)
		{
			_003C_003E4__this.transform.rotation = Quaternion.identity;
			_003C_003E4__this.transform.localEulerAngles = new Vector3(_003C_003E4__this.startXAngle, _003C_003E4__this.transform.localEulerAngles.y, _003C_003E4__this.transform.localEulerAngles.z);
			_003C_003E4__this.transform.position = inputPos + _003C_003E4__this.offset;
			float x = EulerUtils.ToLowestEuler(inputRot.eulerAngles).x;
			x = Mathf.Clamp(x, _003C_003E4__this.minXAngle - _003C_003E4__this.startXAngle, _003C_003E4__this.maxXAngle - _003C_003E4__this.startXAngle);
			float num = Mathf.Lerp(_003C_003E4__this.camPrevXAngle, x, alignLerp);
			_003C_003E4__this.camPrevXAngle = num;
			_003C_003E4__this.transform.RotateAround(inputPos, _003C_003E4__this.transform.right, num);
			float angle = inputRot.eulerAngles.y + _003C_003E4__this.localRotAngle;
			_003C_003E4__this.transform.RotateAround(inputPos, Vector3.up, angle);
			_003CUpdateCamTransform_003Eg__CutoffObstacle_007C2();
		}

		private void _003CUpdateCamTransform_003Eg__CutoffObstacle_007C2()
		{
			if (_003C_003E4__this.cutoffMask.value != 0)
			{
				Vector3 direction = _003C_003E4__this.transform.position - _003C_003E4__this.closestCamPos.position;
				List<RaycastHit> list = Physics.SphereCastAll(_003C_003E4__this.closestCamPos.position, _003C_003E4__this.camera.nearClipPlane * 2f, direction, direction.magnitude, _003C_003E4__this.cutoffMask).ToList();
				if (_003C_003E4__this.cutoffTags.Count > 0)
				{
					list.RemoveAll((RaycastHit x) => !_003C_003E4__this.cutoffTags.Contains(x.collider.tag));
				}
				list.RemoveAll((RaycastHit x) => x.collider.gameObject == _003C_003E4__this.target.gameObject);
				if (list.Count > 0)
				{
					list.Sort((RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
					RaycastHit raycastHit = list[0];
					_003C_003E4__this.transform.position = _003C_003E4__this.closestCamPos.position + direction.normalized * raycastHit.distance;
				}
			}
		}

		internal bool _003CUpdateCamTransform_003Eb__4(RaycastHit x)
		{
			return !_003C_003E4__this.cutoffTags.Contains(x.collider.tag);
		}

		internal bool _003CUpdateCamTransform_003Eb__5(RaycastHit x)
		{
			return x.collider.gameObject == _003C_003E4__this.target.gameObject;
		}

		private void _003CUpdateCamTransform_003Eg__LerpToNextPos_007C3()
		{
			_003C_003E4__this.transform.position = Vector3.Lerp(tempCurPos, _003C_003E4__this.transform.position, moveLerp);
			_003C_003E4__this.transform.rotation = Quaternion.Lerp(tempCurRot, _003C_003E4__this.transform.rotation, rotLerp);
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Comparison<RaycastHit> _003C_003E9__36_6;

		internal int _003CUpdateCamTransform_003Eb__36_6(RaycastHit x, RaycastHit y)
		{
			return x.distance.CompareTo(y.distance);
		}
	}

	[SerializeField]
	private Vector3 offset;

	[SerializeField]
	private float startXAngle = 18f;

	[SerializeField]
	private float minXAngle = 10f;

	[SerializeField]
	private float maxXAngle = 50f;

	[SerializeField]
	private float moveSpeed = 1f;

	[SerializeField]
	private float rotSpeed = 1f;

	[SerializeField]
	private float alignRotSpeed = 10f;

	[SerializeField]
	private float resetTouchRotSpeed = 3f;

	[SerializeField]
	private LayerMask cutoffMask;

	[SerializeField]
	private List<string> cutoffTags;

	[Header("Отладка")]
	[SerializeField]
	[ReadonlyInspector]
	private float localRotAngle;

	[SerializeField]
	private bool resetLocalRotOnMove = true;

	[Header("Ссылки")]
	[SerializeField]
	private AudioListener audioListener;

	[SerializeField]
	private Camera camera;

	[SerializeField]
	private Transform closestCamPos;

	[SerializeField]
	private Transform target;

	private Quaternion startCamRot;

	[SerializeField]
	private bool enableStableLerp = true;

	private Vector3 prevTargetPos;

	private Quaternion prevTargetRot;

	private float camPrevXAngle;

	private bool enabledAtLeastOneTime;

	public Camera Camera => camera;

	private bool IsPlayerMoving => Avelog.Input.VertAxis != 0f;

	public static event Action initializeEvent;

	public void Initialize()
	{
		prevTargetPos = target.transform.position;
		prevTargetRot = target.transform.rotation;
		startCamRot = base.transform.localRotation;
		camPrevXAngle = startXAngle;
		SceneController.changeActiveSceneEvent += OnChangeActiveScene;
		UpdateState();
		Avelog.Input.touchRotateEvent += OnTouchRotate;
		PlayerCamera.initializeEvent?.Invoke();
	}

	private void OnDestroy()
	{
		SceneController.changeActiveSceneEvent -= OnChangeActiveScene;
		Avelog.Input.touchRotateEvent -= OnTouchRotate;
	}

	private void OnChangeActiveScene(SceneController.SceneType newActiveScene)
	{
		UpdateState();
	}

	private void OnTouchRotate(Vector2 touchDelta)
	{
		if (!IsPlayerMoving)
		{
			localRotAngle += touchDelta.x;
			localRotAngle = EulerUtils.ToLowestAngle(localRotAngle);
		}
	}

	private void UpdateState()
	{
		_003CUpdateState_003Eg__SwitchState_007C33_0(SceneController.Instance.CurActiveScene == SceneController.SceneType.Game);
	}

	private void LateUpdate()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		if (enableStableLerp)
		{
			float num = 0f;
			List<float> list = new List<float>();
			float num2 = Time.deltaTime;
			while (num2 > 0.005f)
			{
				num2 -= 0.005f;
				list.Add(0.005f);
			}
			list.Add(num2);
			foreach (float item in list)
			{
				num = ((!(Time.deltaTime > 0f)) ? 1f : (num + item / Time.deltaTime));
				Vector3 targetPos = Vector3.Lerp(prevTargetPos, target.transform.position, num);
				Quaternion targetRot = Quaternion.Lerp(prevTargetRot, target.transform.rotation, num);
				UpdateCamTransform(targetPos, targetRot, moveSpeed * item, rotSpeed * item, alignRotSpeed * item);
			}
			prevTargetPos = target.transform.position;
			prevTargetRot = target.transform.rotation;
		}
		else
		{
			UpdateCamTransform(target.transform.position, target.transform.rotation, moveSpeed * Time.deltaTime, rotSpeed * Time.deltaTime, alignRotSpeed * Time.deltaTime);
		}
		if (IsPlayerMoving)
		{
			localRotAngle = Mathf.Lerp(localRotAngle, 0f, resetTouchRotSpeed * Time.deltaTime);
		}
	}

	public void UpdateCamInstant()
	{
		prevTargetPos = target.transform.position;
		prevTargetRot = target.transform.rotation;
		UpdateCamTransform(target.transform.position, target.transform.rotation, 1f, 1f, 1f);
	}

    private void UpdateCamTransform(Vector3 targetPos, Quaternion targetRot, float moveLerp, float rotLerp, float alignLerp)
    {
        if (target == null) return;

        // ---- SaveTempPos (giữ trạng thái hiện tại để lerp về sau)
        Vector3 tempCurPos = transform.position;
        Quaternion tempCurRot = transform.rotation;

        // ---- CalcNextPos (tính vị trí/rotation tiếp theo trước khi cắt vật cản)
        transform.rotation = Quaternion.identity;
        transform.localEulerAngles = new Vector3(startXAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
        transform.position = targetPos + offset;

        float x = EulerUtils.ToLowestEuler(targetRot.eulerAngles).x;
        x = Mathf.Clamp(x, minXAngle - startXAngle, maxXAngle - startXAngle);

        float xAligned = Mathf.Lerp(camPrevXAngle, x, alignLerp);
        camPrevXAngle = xAligned;

        // Pitch quanh trục right (tâm là targetPos)
        transform.RotateAround(targetPos, transform.right, xAligned);

        // Yaw quanh up (thêm localRotAngle từ cảm ứng)
        float yawAngle = targetRot.eulerAngles.y + localRotAngle;
        transform.RotateAround(targetPos, Vector3.up, yawAngle);

        // ---- CutoffObstacle (dịch camera về gần nếu chạm vật cản giữa closestCamPos và camera)
        if (cutoffMask.value != 0)
        {
            Vector3 direction = transform.position - closestCamPos.position;
            float distance = direction.magnitude;

            var hits = Physics.SphereCastAll(
                closestCamPos.position,
                camera.nearClipPlane * 2f,
                direction,
                distance,
                cutoffMask
            ).ToList();

            if (cutoffTags.Count > 0)
            {
                // Chỉ giữ lại các tag được phép cắt
                hits.RemoveAll(h => !cutoffTags.Contains(h.collider.tag));
            }

            // Bỏ qua va chạm với chính target
            hits.RemoveAll(h => h.collider.gameObject == target.gameObject);

            if (hits.Count > 0)
            {
                hits.Sort((a, b) => a.distance.CompareTo(b.distance));
                RaycastHit nearest = hits[0];
                transform.position = closestCamPos.position + direction.normalized * nearest.distance;
            }
        }

        // ---- LerpToNextPos (nội suy mượt từ trạng thái cũ tới trạng thái mới)
        transform.position = Vector3.Lerp(tempCurPos, transform.position, moveLerp);
        transform.rotation = Quaternion.Lerp(tempCurRot, transform.rotation, rotLerp);
    } 

    [CompilerGenerated]
	private void _003CUpdateState_003Eg__SwitchState_007C33_0(bool enabled)
	{
		if (base.enabled == enabled && enabledAtLeastOneTime)
		{
			return;
		}
		base.enabled = enabled;
		camera.enabled = enabled;
		audioListener.enabled = enabled;
		if (enabled)
		{
			base.transform.parent = Singleton<PlayerSpawner>.Instance.SpawnedObjsParent;
			if (!enabledAtLeastOneTime)
			{
				enabledAtLeastOneTime = true;
				UpdateCamTransform(target.transform.position, target.transform.rotation, 1f, 1f, 1f);
			}
		}
	}
}
