using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

namespace TMPro
{

    public class MainMenu : MonoBehaviour
    {
        [System.Serializable]
        public struct DiageticUIElement
        {
            [TextArea(5,6)]
            public List<string> potentialMessages;
            public TextMeshPro diageticTextMesh;

            public string GenerateRandomMessage()
            {
                return potentialMessages[Mathf.FloorToInt(Random.Range(0, potentialMessages.Count - 1))];
            }
        }

        bool input = false;

        public DiageticUIElement diageticTest;

        public void Start()
        {
            StartCoroutine(StartGameSequence(5f, 1f));
            diageticTest.diageticTextMesh.text = diageticTest.GenerateRandomMessage();
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                input = true;
            }


        }


        public IEnumerator StartGameSequence(float lerpSpeed, float fade)
        {
            while (!input)
            {
                yield return new WaitForEndOfFrame();
            }

            var fader = FindObjectOfType<OVRScreenFade>();
            fader.FadeOut();

            while (!SqueezeValue(0.01f, fade, fader.currentAlpha))
            {
                yield return new WaitForEndOfFrame();
            }

            print("Start Game");
        }

        public bool SqueezeValue(float squeeze, float pos1, float pos2)
        {
            float toSqueeze = Mathf.Abs(pos1-pos2);
            if (toSqueeze >= -squeeze && toSqueeze < squeeze)
            {
                return true;
            }
            return false;
        }

    }
}
