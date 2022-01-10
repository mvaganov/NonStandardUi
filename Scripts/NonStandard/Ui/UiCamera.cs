using UnityEngine;

namespace NonStandard.Ui {
	public class UiCamera : MonoBehaviour {
		private static Camera _camera;
		public static Camera Get { get { return _camera != null ? _camera : _camera = Find(); } }
		public static Camera Find() {
			return Find((Transform)null);
		}
		public static Camera Find(MonoBehaviour startingFrom) {
			return Find(startingFrom.transform);
		}
		public static Camera Find(GameObject startingFrom) {
			return Find(startingFrom.transform);
		}
		public static Camera Find(Transform startingFrom) {
			Camera c = startingFrom != null ? startingFrom.GetComponent<Camera>() : null;
			if (c == null && startingFrom != null) { c = startingFrom.GetComponentInParent<Camera>(); }
			if (c == null) { c = Camera.main; }
			if (c == null) { c = FindObjectOfType<Camera>(); }
			if (c == null) { c = FindObjectOfType<Camera>(true); }
			return c;
		}
	}
}
