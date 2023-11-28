using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SceneManagement
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Image))]
    public class ChangeScene : MonoBehaviour
    {
        [FormerlySerializedAs("Fader")]
        [SerializeField] private Image fader;
        [SerializeField] private Animator anim;

        private void Start()
        {
            anim.Play("SceneManager_Fade_1_to_0");
        }

        public void LoadScene(string _scene)
        {
            StartCoroutine(Fading(_scene));
        }

        private IEnumerator Fading(string _sceneName)
        {
            anim.Play("SceneManager_Fade_0_to_1");
            yield return new WaitUntil(()=>Math.Abs(fader.color.a - 1) < 0.08f);
            SceneManager.LoadScene(_sceneName);
            anim.Play("SceneManager_Fade_1_to_0");
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}