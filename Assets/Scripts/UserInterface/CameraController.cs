using UnityEngine;

namespace UserInterface
{
    /// <summary>
    /// Simple movable camera implementation.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        public float scrollSpeed = 15;
        public float scrollEdge = 0.01f;

        void Update()
        {
            if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width * (1 - scrollEdge))
            {
                transform.Translate(transform.right * Time.deltaTime * scrollSpeed, Space.World);
            }
            else if (Input.GetKey("q") || Input.mousePosition.x <= Screen.width * scrollEdge)
            {
                transform.Translate(transform.right * Time.deltaTime * -scrollSpeed, Space.World);
            }
            if (Input.GetKey("z") || Input.mousePosition.y >= Screen.height * (1 - scrollEdge))
            {
                transform.Translate(transform.up * Time.deltaTime * scrollSpeed, Space.World);
            }
            else if (Input.GetKey("s") || Input.mousePosition.y <= Screen.height * scrollEdge)
            {
                transform.Translate(transform.up * Time.deltaTime * -scrollSpeed, Space.World);
            }
        }
    }
}
 
