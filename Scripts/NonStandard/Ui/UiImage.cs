using NonStandard;
using NonStandard.Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NonStandard.Ui {
	public class UiImage : MonoBehaviour {
		public static Dictionary<string, Sprite> allSprites = new Dictionary<string, Sprite>();

		private void Start() {
			Object[] spriteList = Resources.FindObjectsOfTypeAll(typeof(Sprite));//FindObjectsOfTypeIncludingAssets(typeof(Sprite));
			// Show.Log(spriteList.JoinToString("\n", s => s.name));
			for (int i = 0; i < spriteList.Length; ++i) {
				allSprites[spriteList[i].name] = spriteList[i] as Sprite;
			}
		}
		public static Component SetSprite(GameObject go, Sprite sprite) {
			UiImage uiImage = go.GetComponentInChildren<UiImage>(true);
			if (uiImage != null) { go = uiImage.gameObject; }
			Image img = go.GetComponentInChildren<Image>();
			if (img != null) { img.sprite = sprite; return img; }
			SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
			if (sr != null) { sr.sprite = sprite; return sr; }
			return null;
		}
		public static void SetSpriteByName(GameObject go, string name) {
			SetSprite(go, GetImageByName(name));
		}
		public static Sprite GetImageByName(string name) {
			if (allSprites.TryGetValue(name, out Sprite sprite)) { return sprite; }
			return null;
		}
		public static bool HasImageHolder(GameObject go) {
			Image img = go.GetComponentInChildren<Image>();
			if (img) return true;
			SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
			if (sr) return true;
			return false;
		}
		public static Object GetImageHolder(GameObject go) {
			Image img = go.GetComponentInChildren<Image>();
			if (img) return img;
			SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
			if (sr) return sr;
			return null;
		}
		public static Sprite GetSprute(GameObject go) {
			Image img = go.GetComponentInChildren<Image>();
			if (img) return img.sprite;
			SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
			if (sr) return sr.sprite;
			return null;
		}
		public static string GetImageName(GameObject go) {
			Image img = go.GetComponentInChildren<Image>();
			if (img != null) { return img.sprite.name; }
			SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
			if (sr != null) { return sr.sprite.name; }
			return null;
		}
	}
}