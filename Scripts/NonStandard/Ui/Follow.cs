using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NonStandard.Extension;

public class Follow : MonoBehaviour {
	public Transform followTarget;
	public GameObject disableWhenTargetDisables;
	RectTransform rt;
	private void Start() {
		rt = GetComponent<RectTransform>();
	}
	void LateUpdate() {
		if (followTarget.gameObject.activeInHierarchy) {
			disableWhenTargetDisables?.SetActive(true);
			rt.SetPositionOver(followTarget);
		} else {
			disableWhenTargetDisables?.SetActive(false);
		}
	}
}
