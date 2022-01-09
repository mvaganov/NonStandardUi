﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NonStandard.Ui.Mouse {
	public class DragWithMouseOrResize : DragWithMouse {
		private bool pointerDown;
		public bool disableReize;
		public bool dynamicMouseCursor = true;
		public float resizeEdgeRadius = 10;
		public Vector2 minimumSize = new Vector2(-1, -1);
		[Tooltip("if negative values, calculate a minimum size based on child elements")]
		private Vector2 startSize;
		/// <summary>
		/// multiple resizable windows can overlap, which means each one can have a different cursor state.
		/// </summary>
		private Direction2D mouseCursorState = Direction2D.None;
		private Direction2D heldDir = Direction2D.None;

		[System.Serializable] public class UnityEvent_Vector2 : UnityEvent<Vector2> { }

		public UnityEvent_Vector2 onResize = new UnityEvent_Vector2();
		public UnityEvent_Vector2 onDoubleClick = new UnityEvent_Vector2();

		public static void ResizeRect(RectTransform rectTransform, Vector2 move, Direction2D dir) {
			Vector2 min = rectTransform.offsetMin, max = rectTransform.offsetMax;
			if ((dir & Direction2D.Bottom) != 0) { min.y += move.y; }
			if ((dir & Direction2D.Left) != 0) { min.x += move.x; }
			if ((dir & Direction2D.Top) != 0) { max.y += move.y; }
			if ((dir & Direction2D.Right) != 0) { max.x += move.x; }
			rectTransform.offsetMin = min; rectTransform.offsetMax = max;
		}

		public void ResizeRect(Vector2 move, Direction2D dir) {
			ResizeRect(rt, move, dir);
			Rect size = rt.rect;
			if (size.width < minimumSize.x || size.height < minimumSize.y) {
				ResizeRect(rt, -move, dir);
			}
			onResize?.Invoke(rt.rect.size);
		}

		public static Direction2D CalculateEdgeDirection(Vector2 p, Vector2 min, Vector2 max, float edgeRadius) {
			float h = max.y - min.y, w = max.x - min.x;
			float top_ = Mathf.Abs(p.y - min.y);
			float left = Mathf.Abs(p.x - min.x);
			float bott = Mathf.Abs(p.y - max.y);
			float righ = Mathf.Abs(p.x - max.x);
			if (h < edgeRadius && w < edgeRadius) { // if the rectangle is too small, make sure at least one edge is always unmoved 
				float m = Mathf.Max(left, bott, righ, top_);
				if (top_ == m) { top_ = edgeRadius + 1; }
				if (left == m) { left = edgeRadius + 1; }
				if (bott == m) { bott = edgeRadius + 1; }
				if (righ == m) { righ = edgeRadius + 1; }
			}
			int result = 0;
			if (top_ < edgeRadius) result |= (int)Direction2D.Top;
			if (left < edgeRadius) result |= (int)Direction2D.Left;
			if (bott < edgeRadius) result |= (int)Direction2D.Bottom;
			if (righ < edgeRadius) result |= (int)Direction2D.Right;
			return (Direction2D)result;
		}

		protected override void Awake() {
			rt = GetComponent<RectTransform>();
			AddPointerEvent(EventTriggerType.PointerDown, this, PointerDown);
			AddPointerEvent(EventTriggerType.BeginDrag, this, BeginDrag);
			AddPointerEvent(EventTriggerType.Drag, this, OnDrag);
			//AddPointerEvent(EventTriggerType.EndDrag, this, OnEndDrag);
			AddPointerEvent(EventTriggerType.PointerUp, this, PointerUp);
			//AddPointerEvent(EventTriggerType.Move, this, CursorChangeOnMove);
			AddPointerEvent(EventTriggerType.PointerEnter, this, PointerEnter);
			AddPointerEvent(EventTriggerType.PointerExit, this, PointerExit);
			//enabled = false;
		}

		void Start() {
			startSize = rt.rect.size;
			if (minimumSize.x < 0 && minimumSize.y < 0) {
				minimumSize = CalculateMinimumSize();
			}
		}

		public Vector2 CalculateMinimumSize() {
			Vector2 s = Vector2.zero, prefSize;
			for (int i = 0; i < transform.childCount; ++i) {
				RectTransform child = transform.GetChild(i) as RectTransform;
				prefSize = new Vector2(LayoutUtility.GetPreferredSize(child, 0), LayoutUtility.GetPreferredSize(child, 1));
				s = new Vector2(Mathf.Max(prefSize.x, s.x), Mathf.Max(prefSize.y, s.y));
				//Debug.Log(child.name + " " + prefSize);
			}
			prefSize = new Vector2(LayoutUtility.GetPreferredSize(rt, 0), LayoutUtility.GetPreferredSize(rt, 1));
			s = new Vector2(Mathf.Max(prefSize.x, s.x), Mathf.Max(prefSize.y, s.y));
			s = new Vector2(Mathf.Min(startSize.x, s.x), Mathf.Min(startSize.y, s.y));
			//Debug.Log(rt.name + " " + prefSize);
			return s;
		}


		public override void PointerDown(BaseEventData data) {
			base.PointerDown(data);
			pointerDown = true;
			if (heldDir == Direction2D.None) {
				heldDir = mouseCursorState;
			}
			data.Use();
			if (data is PointerEventData  eventData && eventData.clickCount == 2) {
				onDoubleClick?.Invoke(eventData.position);
			}
		}

		public virtual void BeginDrag(BaseEventData data) {
			OnDrag(data);
			if (dynamicMouseCursor && heldDir == Direction2D.None) {
				MouseCursor mc = MouseCursor.Instance;
				if (mc != null) { mc.SetCursor(this, MouseCursor.CursorType.Move); }
			}
			data.Use();
		}

		public override void OnDrag(BaseEventData basedata) {
			if (disableDrag) return;
			//isDragging = true;
			PointerEventData data = basedata as PointerEventData;
			if (heldDir == Direction2D.None) {
				base.OnDrag(basedata);
			} else {
				ResizeRect(data.delta, heldDir);
			}
			beingDragged = gameObject;
			data.Use();
		}

		//public override void OnEndDrag(BaseEventData data) { base.OnEndDrag(data); }

		public override void PointerUp(BaseEventData data) {
			base.PointerUp(data);
			pointerDown = false;
			heldDir = Direction2D.None;
			//if (!disableReize) {
			CursorChangeOnMove(null);
			//}
		}

		public void FixedUpdate() {
			if (!disableReize && !pointerDown && heldDir == Direction2D.None && resizeEdgeRadius != 0 && 
				UiClick.GetMouseDelta() != Vector3.zero) {
				CursorChangeOnMove(null);
			}
		}

		public virtual void PointerEnter(BaseEventData data) {
			// Show.Log("enter " + this);
			if (disableReize) { mouseCursorState = Direction2D.None; return; }
			//enabled = resizeEdgeRadius != 0;
		}
		public virtual void PointerExit(BaseEventData data) {
			// Show.Log("exit " + this);
			//Direction2D dir = DragWithMouse.CalculatePointerOutOfBounds(rt, (data as PointerEventData).position, out Vector2 offset, -resizeEdgeRadius);
			//Show.Log(dir+" "+offset);
			if (disableReize) { mouseCursorState = Direction2D.None; return; }
			if (dynamicMouseCursor) {
				MouseCursor mc = MouseCursor.Instance;
				if (mc != null) { mc.UnsetCursor(this); }
			}
			//if (heldDir == Direction2D.None) {
			//	mouseCursorState = Direction2D.None;
			//	enabled = false;
			//}
		}

		public virtual void CursorChangeOnMove(BaseEventData data) {
			MouseCursor mc = MouseCursor.Instance;
			//Vector3[] corners = new Vector3[4];
			//rt.GetWorldCorners(corners);
			//Vector2 min = corners[1], max = corners[3];
			//Vector2 mousePosition = Input.mousePosition; // data.position;
			//mouseCursorState = CalculateEdgeDirection(mousePosition, min, max, resizeEdgeRadius);
			Vector2 pos = UiClick.GetMousePosition();//AppInput.MousePosition;
			if (data is PointerEventData ped) { pos = ped.position; }
			mouseCursorState = CalculatePointerOutOfBounds(rt, pos, out Vector2 offset, -resizeEdgeRadius);
			float r2 = resizeEdgeRadius * 2;
			if (Mathf.Abs(offset.x) > resizeEdgeRadius || Mathf.Abs(offset.y) > resizeEdgeRadius) {
				mouseCursorState = Direction2D.None;
			}
			if (!dynamicMouseCursor || mc == null) return;
			if (mouseCursorState != Direction2D.None) {
				mc.SetCursor(this, mouseCursorState);
			} else {
				mc.SetCursor(this, MouseCursor.CursorType.Cursor);
			}
			data?.Use();
		}

		public void RefreshContentRect(RectTransform contentRect) {
			contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentRect.rect.width);
		}
	}
}