using NonStandard.Ui;
using UnityEngine;
namespace NonStandard.Extension {
	public static class RectTransformExtensions {
		public static float GetTop(this RectTransform rt) { return -rt.offsetMax.y; }
		public static float GetLeft(this RectTransform rt) { return rt.offsetMin.x; }
		public static float GetRight(this RectTransform rt) { return -rt.offsetMax.x; }
		public static float GetBottom(this RectTransform rt) { return rt.offsetMin.y; }
		public static void SetTop(this RectTransform rt, float top) { rt.offsetMax = new Vector2(rt.offsetMax.x, -top); }
		public static void SetLeft(this RectTransform rt, float left) { rt.offsetMin = new Vector2(left, rt.offsetMin.y); }
		public static void SetRight(this RectTransform rt, float right) { rt.offsetMax = new Vector2(-right, rt.offsetMax.y); }
		public static void SetBottom(this RectTransform rt, float bottom) { rt.offsetMin = new Vector2(rt.offsetMin.x, bottom); }
		public static bool SetPositionOver(this RectTransform rt, Transform transformIn3d, Camera camera = null) {
			if (camera == null) { camera = UiCamera.Get; }
			Vector2 screenPosition = camera.WorldToScreenPoint(transformIn3d.position);
			rt.position = screenPosition;
			// TODO implement screen culling calculation here
			return true;
		}
	}
}
