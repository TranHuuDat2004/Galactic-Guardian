using System.Collections;
using TMPro;
using UnityEngine;

namespace BitWave_Labs.AnimatedTextReveal
{
    /// <summary>
    /// This class animates the fade-in effect of a TextMeshProUGUI component, 
    /// smoothly revealing the text from left to right with adjustable speed and spread.
    /// </summary>
    public class AnimatedTextReveal : MonoBehaviour
    {
        // The TextMeshProUGUI component to animate.
        [SerializeField] private TextMeshProUGUI textMesh;

        // The speed at which the text fades in. Higher values result in faster fading.
        [SerializeField] private float fadeSpeed = 20.0f;

        // The number of characters affected at a time, creating a smoother transition effect.
        [SerializeField] private int characterSpread = 10;

        // Stores the running coroutine instance.
        private Coroutine _fadeCoroutine;

        // Tracks which text to display.
        private int _textCount;

        /// <summary>
        /// Gets the <see cref="TextMeshProUGUI"/> component that this script will animate.
        /// </summary>
        public TextMeshProUGUI TextMesh => textMesh;

        /// <summary>
        /// Gradually fades the text in or out by sweeping character transparency from left to right.
        /// </summary>
        /// <param name="fadeIn">Fade in or fade out the text
        /// </param>
        /// <returns> An <see cref="IEnumerator"/> that performs the fade animation over multiple frames.</returns>
        public IEnumerator FadeText(bool fadeIn)
        {
            // Prepare the mesh & textInfo
            textMesh.ForceMeshUpdate();
            TMP_TextInfo textInfo = textMesh.textInfo;
            int totalChars = textInfo.characterCount;
            Color32[] newVertexColors = null;

            // Initialize per‐character alpha
            if (fadeIn)
            {
                // Start everything invisible
                SetAllCharactersAlpha(0);
            }
            else
            {
                // Ensure material tint is opaque
                textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1f);
                // Start everything fully visible
                SetAllCharactersAlpha(255);
            }

            // Compute one “step” of alpha change per frame
            byte fadeStep = (byte)Mathf.Max(1, 255 / characterSpread);

            int charsProcessed = 0;
            bool done = false;

            // Sweep across the characters
            while (!done)
            {
                for (int i = 0; i < charsProcessed + 1 && i < totalChars; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible)
                        continue;

                    int matIdx = textInfo.characterInfo[i].materialReferenceIndex;
                    int vertIdx = textInfo.characterInfo[i].vertexIndex;
                    newVertexColors = textInfo.meshInfo[matIdx].colors32;

                    // Pick current alpha
                    byte currentAlpha = newVertexColors[vertIdx].a;

                    // Compute new alpha up or down
                    int delta = fadeIn ? +fadeStep : -fadeStep;
                    byte nextAlpha = (byte)Mathf.Clamp(currentAlpha + delta, 0, 255);

                    // Apply to all four verts
                    newVertexColors[vertIdx + 0].a = nextAlpha;
                    newVertexColors[vertIdx + 1].a = nextAlpha;
                    newVertexColors[vertIdx + 2].a = nextAlpha;
                    newVertexColors[vertIdx + 3].a = nextAlpha;
                }

                // Push to mesh
                textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                // Advance the sweep
                if (charsProcessed < totalChars)
                    charsProcessed++;

                // Detect completion on the last character
                if (charsProcessed >= totalChars && newVertexColors != null)
                {
                    TMP_CharacterInfo lastChar = textInfo.characterInfo[totalChars - 1];
                    int finalAlpha = newVertexColors[lastChar.vertexIndex].a;
                    done = fadeIn ? finalAlpha == 255 : finalAlpha == 0;
                }

                // Wait just like your old methods
                yield return new WaitForSeconds(0.02f + (0.25f - fadeSpeed * 0.01f));
            }
        }

        /// <summary>
        /// Sets every visible character’s vertex alpha to the given value (0–255).
        /// </summary>
        /// <param name="alpha">
        /// The alpha value to apply to all characters (0 = fully transparent, 255 = fully opaque).
        /// </param>
        public void SetAllCharactersAlpha(byte alpha)
        {
            // Rebuild the mesh so textInfo and meshInfo are valid
            textMesh.ForceMeshUpdate();
            TMP_TextInfo textInfo = textMesh.textInfo;

            // Loop each character
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;

                int matIdx = charInfo.materialReferenceIndex;
                var verts = textInfo.meshInfo[matIdx].colors32;
                int vIdx = charInfo.vertexIndex;

                // Set all four vertices to the requested alpha
                verts[vIdx + 0].a = alpha;
                verts[vIdx + 1].a = alpha;
                verts[vIdx + 2].a = alpha;
                verts[vIdx + 3].a = alpha;
            }

            // Push the updated colors back into each mesh
            for (int m = 0; m < textInfo.meshInfo.Length; m++)
                textInfo.meshInfo[m].mesh.colors32 = textInfo.meshInfo[m].colors32;
        }
    }
}