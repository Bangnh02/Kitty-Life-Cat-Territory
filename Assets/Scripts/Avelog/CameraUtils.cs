using UnityEngine;

namespace Avelog
{
	public class CameraUtils
	{
		private static Camera _mainCamera;

		private static Camera _playerCamera;

		public static Camera MainCamera
		{
			get
			{
				if (_mainCamera == null || !_mainCamera.enabled || !_mainCamera.gameObject.activeInHierarchy)
				{
					_mainCamera = Camera.main;
				}
				return _mainCamera;
			}
		}

		public static Camera PlayerCamera
		{
			get
			{
				if (_playerCamera == null)
				{
					PlayerCamera playerCamera = Object.FindObjectOfType<PlayerCamera>();
					if (playerCamera != null)
					{
						_playerCamera = playerCamera.Camera;
					}
				}
				if (_playerCamera == null)
				{
					return _mainCamera;
				}
				return _playerCamera;
			}
		}
	}
}
