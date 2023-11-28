using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScreenResolutionManager.Example
{
    public class ScreenLogger : MonoBehaviour
    {
        public enum LogAnchor
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        [FormerlySerializedAs("IsPersistent")]
        public bool isPersistent = true;
        [FormerlySerializedAs("ShowInEditor")]
        public bool showInEditor = true;

        [FormerlySerializedAs("Height")]
        [Tooltip("Height of the log area as a percentage of the screen height")]
        [Range(0.3f, 1.0f)]
        public float height = 0.5f;

        [FormerlySerializedAs("Width")]
        [Tooltip("Width of the log area as a percentage of the screen width")]
        [Range(0.3f, 1.0f)]
        public float width = 0.5f;

        [FormerlySerializedAs("Margin")]
        public int margin = 20;

        [FormerlySerializedAs("AnchorPosition")]
        public LogAnchor anchorPosition = LogAnchor.BottomLeft;

        [FormerlySerializedAs("FontSize")]
        public int fontSize = 14;

        [FormerlySerializedAs("BackgroundOpacity")]
        [Range(0f, 01f)]
        public float backgroundOpacity = 0.5f;
        [FormerlySerializedAs("BackgroundColor")]
        public Color backgroundColor = Color.black;

        [FormerlySerializedAs("LogMessages")]
        public bool logMessages = true;
        [FormerlySerializedAs("LogWarnings")]
        public bool logWarnings = true;
        [FormerlySerializedAs("LogErrors")]
        public bool logErrors = true;

        [FormerlySerializedAs("MessageColor")]
        public Color messageColor = Color.white;
        [FormerlySerializedAs("WarningColor")]
        public Color warningColor = Color.yellow;
        [FormerlySerializedAs("ErrorColor")]
        public Color errorColor = new Color(1, 0.5f, 0.5f);

        [FormerlySerializedAs("StackTraceMessages")]
        public bool stackTraceMessages = false;
        [FormerlySerializedAs("StackTraceWarnings")]
        public bool stackTraceWarnings = false;
        [FormerlySerializedAs("StackTraceErrors")]
        public bool stackTraceErrors = true;

        static Queue<LogMessage> _queue = new Queue<LogMessage>();

        GUIStyle styleContainer, styleText;
        int padding = 5;

        public void Awake()
        {
            Texture2D _back = new Texture2D(1, 1);
            backgroundColor.a = backgroundOpacity;
            _back.SetPixel(0, 0, backgroundColor);
            _back.Apply();

            styleContainer = new GUIStyle();
            styleContainer.normal.background = _back;
            styleContainer.wordWrap = true;
            styleContainer.padding = new RectOffset(padding, padding, padding, padding);

            styleText = new GUIStyle();
            styleText.fontSize = fontSize;

            if (isPersistent)
                DontDestroyOnLoad(this);
        }

        void OnEnable()
        {
            if (!showInEditor && Application.isEditor) return;

            _queue = new Queue<LogMessage>();

#if UNITY_4_5 || UNITY_4_6
        Application.RegisterLogCallback(HandleLog);
#else
            Application.logMessageReceived += HandleLog;
#endif
        }

        void OnDisable()
        {
            if (!showInEditor && Application.isEditor) return;

#if UNITY_4_5 || UNITY_4_6
        Application.RegisterLogCallback(null);
#else
            Application.logMessageReceived -= HandleLog;
#endif
        }

        void Update()
        {
            if (!showInEditor && Application.isEditor) return;

            while (_queue.Count > ((Screen.height - 2 * margin) * height - 2 * padding) / styleText.lineHeight)
                _queue.Dequeue();
        }

        void OnGUI()
        {
            if (!showInEditor && Application.isEditor) return;

            float _w = (Screen.width - 2 * margin) * width;
            float _h = (Screen.height - 2 * margin) * height;
            float _x = 1, _y = 1;

            switch (anchorPosition)
            {
                case LogAnchor.BottomLeft:
                    _x = margin;
                    _y = margin + (Screen.height - 2 * margin) * (1 - height);
                    break;

                case LogAnchor.BottomRight:
                    _x = margin + (Screen.width - 2 * margin) * (1 - width);
                    _y = margin + (Screen.height - 2 * margin) * (1 - height);
                    break;

                case LogAnchor.TopLeft:
                    _x = margin;
                    _y = margin;
                    break;

                case LogAnchor.TopRight:
                    _x = margin + (Screen.width - 2 * margin) * (1 - width);
                    _y = margin;
                    break;
            }

            GUILayout.BeginArea(new Rect(_x, _y, _w, _h), styleContainer);

            foreach (LogMessage _m in _queue)
            {
                switch (_m.Type)
                {
                    case LogType.Warning:
                        styleText.normal.textColor = warningColor;
                        break;

                    case LogType.Log:
                        styleText.normal.textColor = messageColor;
                        break;

                    case LogType.Assert:
                    case LogType.Exception:
                    case LogType.Error:
                        styleText.normal.textColor = errorColor;
                        break;

                    default:
                        styleText.normal.textColor = messageColor;
                        break;
                }

                GUILayout.Label(_m.Message, styleText);
            }

            GUILayout.EndArea();
        }

        void HandleLog(string _message, string _stackTrace, LogType _type)
        {
            if (_type == LogType.Assert && !logErrors) return;
            if (_type == LogType.Error && !logErrors) return;
            if (_type == LogType.Exception && !logErrors) return;
            if (_type == LogType.Log && !logMessages) return;
            if (_type == LogType.Warning && !logWarnings) return;

            _queue.Enqueue(new LogMessage(_message, _type));

            if (_type == LogType.Assert && !stackTraceErrors) return;
            if (_type == LogType.Error && !stackTraceErrors) return;
            if (_type == LogType.Exception && !stackTraceErrors) return;
            if (_type == LogType.Log && !stackTraceMessages) return;
            if (_type == LogType.Warning && !stackTraceWarnings) return;

            string[] _trace = _stackTrace.Split(new char[] { '\n' });

            foreach (string _t in _trace)
                if (_t.Length != 0) _queue.Enqueue(new LogMessage("  " + _t, _type));
        }
    }

    class LogMessage
    {
        public string Message;
        public LogType Type;

        public LogMessage(string _msg, LogType _type)
        {
            Message = _msg;
            Type = _type;
        }
    }
}