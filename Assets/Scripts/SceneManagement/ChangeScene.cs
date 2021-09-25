using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneManagement
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Image))]
    public class ChangeScene : MonoBehaviour
    {
        [SerializeField] private Image Fader;
        [SerializeField] private Animator anim;
        private static readonly int Fade = Animator.StringToHash("Fade");

        private void Start()
        {
            anim.Play("SceneManager_Fade_1_to_0");
        }

        public void LoadScene(string scene)
        {
            StartCoroutine(Fading(scene));
        }

        private IEnumerator Fading(string sceneName)
        {
            anim.Play("SceneManager_Fade_0_to_1");
            yield return new WaitUntil(()=>Math.Abs(Fader.color.a - 1) < 0.08f);
            SceneManager.LoadScene(sceneName);
            anim.Play("SceneManager_Fade_1_to_0");
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}