using NonStandard.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace NonStandard.Ui {
	public class DescriptionTextWithIcon : MonoBehaviour {
		public GameObject textObject;
		public Image image;
		public bool UseImage {
			get => image.gameObject.activeSelf;
			set {
				RectTransform irect = image.rectTransform;
				RectTransform trect = textObject.GetComponent<RectTransform>();
				RectTransform prect = textObject.transform.parent.GetComponent<RectTransform>();
				Rect imgR = irect.rect;
				Rect parR = prect.rect;
				if (trect.anchorMin == trect.anchorMax) {
					if (value) {
						Vector2 size = trect.sizeDelta;
						size.x = parR.width;
						trect.sizeDelta = size;
					} else {
						Vector2 size = trect.sizeDelta;
						size.x = parR.width - imgR.width;
						trect.sizeDelta = size;
					}
				}
				image.gameObject.SetActive(value);
			}
		}
		public Sprite Sprite { get => image != null ? image.sprite : null; set { if (image == null) { return; } image.sprite = value; } }
		public string Text {
			get => UiText.GetText(textObject);
			set {
				UiText.SetText(textObject, value);
			}
		}
		public Color32 TextColor { get => UiText.GetColor(textObject); set => UiText.SetColor(textObject, value); }
		public Color32 SpriteColor { get => image != null ? image.color : Color.clear; set { if (image == null) { return; } image.color = value; } }
		public HorizontalLayoutGroup ParentHorizontalGroup => textObject.transform.parent.GetComponentInParent<HorizontalLayoutGroup>();
		private void OnEnable() { Refresh(); }
		public void Refresh() {
			if (textObject == null) textObject = UiText.GetTextComponent(textObject).gameObject;
			if (image == null) image = GetComponentInChildren<Image>();
			UseImage = UseImage;
		}
	}
}