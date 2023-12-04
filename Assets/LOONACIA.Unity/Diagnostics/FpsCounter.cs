using UnityEngine;

namespace LOONACIA.Unity.Diagnostics
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 100)]
        private int _fontSize = 50;

        [SerializeField]
        private Color _fontColor = Color.black;

        private float _deltaTime;

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            Rect rect = new(0, 0, w, h * 0.02f);
			
            float ms = _deltaTime * 1000.0f;
            float fps = 1.0f / _deltaTime;
            string text = $"{ms:0.0} ms ({fps:0.} fps)";
			
            GUIStyle style = new()
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = h * 2 / _fontSize,
                normal =
                {
                    textColor = _fontColor
                }
            };
			
            GUI.Label(rect, text, style);
        }
    }
}