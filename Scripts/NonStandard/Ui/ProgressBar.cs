using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NonStandard.Ui {
	public class ProgressBar : MonoBehaviour {
		public RectTransform progressBar;
		public RectTransform progressBacking;
		public GameObject label;
		public Button cancelButton;
		public bool canCancel;
		public bool canPause;
		[Range(0, 1)]
		public float progress = 1f;
		public UnityEvent_float onProgress;
		public UnityEvent onComplete;
		public UnityEvent onCancel;

		public class UnityEvent_float : UnityEvent<float> { }
#if UNITY_EDITOR
		private void OnValidate() {
			Progress = progress;
			if (cancelButton != null) {
				EventBind.IfNotAlready(cancelButton.onClick, this, nameof(Cancel));
				cancelButton.gameObject.SetActive(canCancel);
			}
		}
		private void Reset() {
			EventBind.IfNotAlready(onComplete, this, nameof(Hide));
			EventBind.IfNotAlready(onCancel, this, nameof(Hide));
		}
#endif
		public void Hide() { gameObject.SetActive(false); }
		public void Show() { gameObject.SetActive(true); }
		public void Cancel() {
			onCancel?.Invoke();
		}
		public float Progress {
			get { return progress; }
			set {
				progress = Mathf.Clamp01(value);
				Vector2 size = progressBar.sizeDelta;
				Rect r = progressBacking.rect;
				size.x = progress * r.width;
				progressBar.sizeDelta = size;
				onProgress?.Invoke(progress);
				if (progress >= 1) {
					onComplete?.Invoke();
				}
			}
		}
	}
}
