using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cells;
using Editor.GridGenerators;
using GridObjects;
using Players;
using StateMachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UserInterface;

namespace Editor
{
    class GridHelper : EditorWindow
    {
        private static string _mapType3D = "XZ Plane (3D)";
        private static string _mapType2D = "XY Plane (2D)";

        public bool useMovableCamera = false;
        public float cameraScrollSpeed = 15f;
        public float cameraScrollEdge = 0.01f;

        public int nHumanPlayer = 2;
        public int nComputerPlayer;

        public int generatorIndex;
        public int mapTypeIndex;

        public static List<Type> generators;
        public static string[] generatorNames;
        public static string[] mapTypes;

        GameObject cellGrid;
        GameObject objects;
        GameObject players;
        GameObject guiController;
        GameObject directionalLight;

        Dictionary<string, object> parameterValues;

        BoolWrapper tileEditModeOn = new BoolWrapper(false);
        [SerializeField] private CellSo cellType;
        int tilePaintingRadius = 1;
        int lastPaintedHash = -1;

        BoolWrapper unitEditModeOn = new BoolWrapper(false);
        [SerializeField]
        GridObjects.GridObject unitPrefab;

        int playerNumber;

        bool gridGameObjectPresent;
        bool unitsGameObjectPresent;
        GameObject gridGameObject;
        GameObject unitsGameObject;

        BoolWrapper toToggle = null;

        bool shouldDisplayCollider2DWarning;
        private Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Window/Grid Helper")]
        public static void ShowWindow()
        {
            EditorWindow _window = GetWindow(typeof(GridHelper));
            _window.titleContent.text = "Grid Helper";
        }

        void Initialize()
        {
            if (parameterValues == null)
            {
                parameterValues = new Dictionary<string, object>();
            }

            if (generators == null)
            {
                Type _generatorInterface = typeof(CellGridGenerator);
                Assembly _assembly = _generatorInterface.Assembly;

                generators = new List<Type>();
                foreach (Type _t in _assembly.GetTypes())
                {
                    if (_generatorInterface.IsAssignableFrom(_t) && _t != _generatorInterface)
                        generators.Add(_t);
                }
            }

            if (generatorNames == null)
            {
                generatorNames = generators.Select(_t => _t.Name).ToArray();
            }

            if (mapTypes == null)
            {
                mapTypes = new string[2];
                mapTypes[0] = _mapType2D;
                mapTypes[1] = _mapType3D;
            }
        }

        public void OnEnable()
        {
            Initialize();

            GameObject _gridGameObject = GameObject.Find("CellGrid");
            GameObject _unitsGameObject = GameObject.Find("Objects");

            gridGameObjectPresent = _gridGameObject != null;
            unitsGameObjectPresent = _unitsGameObject != null;

            tileEditModeOn = new BoolWrapper(false);
            unitEditModeOn = new BoolWrapper(false);

            EnableSceneViewInteraction();
            Selection.selectionChanged += OnSelectionChanged;
            Undo.undoRedoPerformed += OnUndoPerformed;
        }


        public void OnDestroy()
        {
            DisableSceneViewInteraction();
            tileEditModeOn = new BoolWrapper(false);
            unitEditModeOn = new BoolWrapper(false);

            Selection.selectionChanged -= OnSelectionChanged;
            Undo.undoRedoPerformed -= OnUndoPerformed;
        }

        void OnHierarchyChange()
        {
            GameObject _gridGameObject = GameObject.Find("CellGrid");
            GameObject _unitsGameObject = GameObject.Find("Objects");

            gridGameObjectPresent = _gridGameObject != null;
            unitsGameObjectPresent = _unitsGameObject != null;

            if (_unitsGameObject != null)
            {
                this.unitsGameObject = null;
            }
            if (_gridGameObject != null)
            {
                this.gridGameObject = null;
            }

            Repaint();
        }

        void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none);
            MapGenerationGUI();
            TilePaintingGUI();
            UnitPaintingGUI();
            PrefabHelperGUI();

            Event _e = Event.current;
            if (_e.type == EventType.KeyDown && _e.control && _e.keyCode == KeyCode.R)
            {
                ToggleEditMode();
            }
            GUILayout.EndScrollView();
        }

        private void ToggleEditMode()
        {
            if (toToggle == null)
            {
                return;
            }
            toToggle.value = !toToggle.value;
            if (toToggle.value)
            {
                EnableSceneViewInteraction();
            }
            Repaint();
        }

        private void PrefabHelperGUI()
        {
            GUILayout.Label("Prefab helper", EditorStyles.boldLabel);
            GUILayout.Label("Select multiple objects in hierarchy and click button below to create multiple prefabs at once. Please note that this may take a while", EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("Selection to prefabs"))
            {
                string _path = EditorUtility.SaveFolderPanel("Save prefabs", "", "");
                if (_path.Length != 0)
                {
                    _path = _path.Replace(Application.dataPath, "Assets");

                    GameObject[] _objectArray = Selection.gameObjects;
                    for (int _i = 0; _i < _objectArray.Length; _i++)
                    {
                        GameObject _gameObject = _objectArray[_i];
                        string _localPath = _path + "/" + _gameObject.name + ".prefab";
                        _localPath = AssetDatabase.GenerateUniqueAssetPath(_localPath);
                        PrefabUtility.SaveAsPrefabAssetAndConnect(_gameObject, _localPath, InteractionMode.UserAction);
                    }
                    Debug.Log(string.Format("{0} prefabs saved to {1}", _objectArray.Length, _path));
                }
            }
        }

        private void UnitPaintingGUI()
        {
            GUILayout.Label("Unit painting", EditorStyles.boldLabel);
            if (!unitsGameObjectPresent)
            {
                if (unitsGameObject == null)
                {
                    GUIStyle _style = new GUIStyle(EditorStyles.boldLabel);
                    _style.normal.textColor = Color.red;
                    GUILayout.Label("Unit parent GameObject missing", _style);
                }
                unitsGameObject = (GameObject)EditorGUILayout.ObjectField("Units parent", unitsGameObject, typeof(GameObject), true, new GUILayoutOption[0]);
            }
            unitPrefab = (GridObjects.GridObject)EditorGUILayout.ObjectField("Unit prefab", unitPrefab, typeof(GridObjects.GridObject), false, new GUILayoutOption[0]);
            playerNumber = EditorGUILayout.IntField(new GUIContent("Player number"), playerNumber);
            GUILayout.Label(string.Format("Unit Edit Mode is {0}", unitEditModeOn.value ? "on" : "off"));

            if (toToggle != null && toToggle == unitEditModeOn)
            {
                GUILayout.Label("Press Ctrl + R to toggle unit painting mode on / off");
            }

            if (unitEditModeOn.value)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Enter Unit Edit Mode"))
            {
                unitEditModeOn = new BoolWrapper(true);
                tileEditModeOn = new BoolWrapper(false);
                toToggle = unitEditModeOn;

                GameObject _unitsParent = unitsGameObjectPresent ? GameObject.Find("Objects") : unitsGameObject;
                if (_unitsParent == null)
                {
                    Debug.LogError("Units parent gameObject is missing, assign it in GridHelper");
                }
            }

            GUI.enabled = true;
            if (!unitEditModeOn.value)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Exit Unit Edit Mode"))
            {
                unitEditModeOn = new BoolWrapper(false);
                DisableSceneViewInteraction();
            }
            GUI.enabled = true;
        }

        private void TilePaintingGUI()
        {
            GUILayout.Label("Tile painting", EditorStyles.boldLabel);
            if (!gridGameObjectPresent)
            {
                if (gridGameObject == null)
                {
                    GUIStyle _style = new GUIStyle(EditorStyles.boldLabel);
                    _style.normal.textColor = Color.red;
                    GUILayout.Label("CellGrid GameObject missing", _style);
                }
                gridGameObject = (GameObject)EditorGUILayout.ObjectField("CellGrid", gridGameObject, typeof(GameObject), true, new GUILayoutOption[0]);
            }
            tilePaintingRadius = EditorGUILayout.IntSlider(new GUIContent("Brush radius"), tilePaintingRadius, 1, 4);
            EditorGUI.BeginChangeCheck();
            cellType = (CellSo)EditorGUILayout.ObjectField("Tile prefab", cellType, typeof(CellSo), true, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                lastPaintedHash = -1;
            }
            GUILayout.Label(string.Format("Tile Edit Mode is {0}", tileEditModeOn.value ? "on" : "off"));

            if (toToggle != null && toToggle == tileEditModeOn)
            {
                GUILayout.Label("Press Ctrl + R to toggle tile painting mode on / off");
            }

            if (tileEditModeOn.value)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Enter Tile Edit Mode"))
            {
                tileEditModeOn = new BoolWrapper(true);
                unitEditModeOn = new BoolWrapper(false);
                toToggle = tileEditModeOn;
                EnableSceneViewInteraction();

                GameObject _cellGrid = gridGameObjectPresent ? GameObject.Find("CellGrid") : gridGameObject;
                if (_cellGrid == null)
                {
                    Debug.LogError("CellGrid gameobject is missing, assign it in GridHelper");
                }
            }
            GUI.enabled = true;
            if (!tileEditModeOn.value)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Exit Tile Edit Mode"))
            {
                tileEditModeOn = new BoolWrapper(false);
                DisableSceneViewInteraction();
            }
            GUI.enabled = true;
        }

        private void MapGenerationGUI()
        {
            GUILayout.Label("Grid generation", EditorStyles.boldLabel);
            GUILayout.Label("Camera", EditorStyles.boldLabel);
            useMovableCamera = EditorGUILayout.Toggle("Use movable camera", useMovableCamera, new GUILayoutOption[0]);

            if (useMovableCamera)
            {
                cameraScrollSpeed = EditorGUILayout.FloatField(new GUIContent("Scroll Speed"), cameraScrollSpeed);
                cameraScrollEdge = EditorGUILayout.Slider("Scroll Edge", cameraScrollEdge, 0.05f, 0.25f, new GUILayoutOption[0]);
            }

            GUILayout.Label("Players", EditorStyles.boldLabel);
            nHumanPlayer = EditorGUILayout.IntField(new GUIContent("Human players No"), nHumanPlayer);
            nComputerPlayer = EditorGUILayout.IntField(new GUIContent("AI players No"), nComputerPlayer);

            GUILayout.Label("Grid", EditorStyles.boldLabel);

            mapTypeIndex = EditorGUILayout.Popup("Plane", mapTypeIndex, mapTypes, new GUILayoutOption[0]);
            GUI.changed = false;
            generatorIndex = EditorGUILayout.Popup("Generator", generatorIndex, generatorNames, new GUILayoutOption[0]);
            if (GUI.changed)
            {
                parameterValues = new Dictionary<string, object>();
            }

            foreach (FieldInfo _field in generators[generatorIndex].GetFields().Where(_f => _f.IsPublic))
            {
                if (_field.FieldType == typeof(int))
                {
                    int _x = 0;
                    if (parameterValues.ContainsKey(_field.Name))
                    {
                        _x = (int)(parameterValues[_field.Name]);
                    }
                    _x = EditorGUILayout.IntField(new GUIContent(_field.Name), _x);
                    parameterValues[_field.Name] = _x;
                }
                else if (_field.FieldType == typeof(GameObject))
                {
                    GameObject _g = null;
                    if (parameterValues.ContainsKey(_field.Name))
                    {
                        _g = (GameObject)(parameterValues[_field.Name]);
                    }
                    _g = (GameObject)EditorGUILayout.ObjectField(_field.Name, _g, typeof(GameObject), false, new GUILayoutOption[0]);
                    parameterValues[_field.Name] = _g;

                    if (mapTypes[mapTypeIndex].Equals(_mapType3D) && _g != null && _g.GetComponent<Collider2D>() != null)
                    {
                        shouldDisplayCollider2DWarning = true;
                    }
                    else
                    {
                        shouldDisplayCollider2DWarning = false;
                    }
                }
            }

            if (shouldDisplayCollider2DWarning)
            {
                GUIStyle _style = new GUIStyle(EditorStyles.wordWrappedLabel);
                _style.fontStyle = FontStyle.Bold;
                _style.normal.textColor = Color.red;
                GUILayout.Label("You are trying to generate a map on XZ plane with a prefab containg a 2D collider. 2D colliders will not work in XZ axis. Add a 3D collider to your tile prefab or select XY plane", _style);
            }

            if (GUILayout.Button("Generate scene"))
            {
                string _dialogTitle = "Confirm delete";
                string _dialogMessage = "This will delete all objects on the scene. Do you wish to continue?";
                string _dialogOk = "Ok";
                string _dialogCancel = "Cancel";

                bool _shouldDelete = EditorUtility.DisplayDialog(_dialogTitle, _dialogMessage, _dialogOk, _dialogCancel);
                if (_shouldDelete)
                {
                    Undo.ClearAll();
                    GenerateBaseStructure();
                }
            }
            
            if (GUILayout.Button("Generate Grid"))
            {
                GenerateGrid();
            }
            
            if (GUILayout.Button("Clear scene"))
            {
                string _dialogTitle = "Confirm delete";
                string _dialogMessage = "This will delete all objects on the scene. Do you wish to continue?";
                string _dialogOk = "Ok";
                string _dialogCancel = "Cancel";

                bool _shouldDelete = EditorUtility.DisplayDialog(_dialogTitle, _dialogMessage, _dialogOk, _dialogCancel);
                if (_shouldDelete)
                {
                    Undo.ClearAll();
                    GridHelperUtils.ClearScene();
                }
            }
        }

        private void OnSceneGUI(SceneView _sceneView)
        {
            Event _e = Event.current;
            if (_e.type == EventType.KeyDown && _e.control && _e.keyCode == KeyCode.R)
            {
                ToggleEditMode();
            }

            if (tileEditModeOn.value || unitEditModeOn.value)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            if (tileEditModeOn.value && cellType != null)
            {
                PaintTiles();
            }
            if (unitEditModeOn.value && unitPrefab != null)
            {
                PaintUnits();
            }
        }

        private void PaintUnits()
        {
            GameObject _unitsParent = unitsGameObjectPresent ? GameObject.Find("Objects") : unitsGameObject;
            if (_unitsParent == null)
            {
                return;
            }

            Cell _selectedCell = GetSelectedCell();
            if (_selectedCell == null)
            {
                return;
            }

            string _mapType = mapTypes[mapTypeIndex];
            Handles.color = Color.red;
            Handles.DrawWireDisc(_selectedCell.transform.position, Vector3.up, _selectedCell.GetCellDimensions().y / 2);
            Handles.DrawWireDisc(_selectedCell.transform.position, Vector3.forward, _selectedCell.GetCellDimensions().y / 2);
            HandleUtility.Repaint();
            if (Event.current.button == 0 && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)) 
            {
                if (unitEditModeOn.value && _selectedCell.IsTaken)
                {
                    return;
                }

                Undo.SetCurrentGroupName("Unit painting");
                int _group = Undo.GetCurrentGroup();

                Undo.RecordObject(_selectedCell, "Unit painting");
                
                GridObject _newUnit = (PrefabUtility.InstantiatePrefab(unitPrefab.gameObject) as GameObject).GetComponent<GridObject>();
                _selectedCell.Take(_newUnit);

                Vector3 _offset = new Vector3(0, 0, _selectedCell.GetCellDimensions().z);
                _newUnit.transform.position = _selectedCell.transform.position;
                _newUnit.transform.parent = _unitsParent.transform;
                _newUnit.transform.localPosition -= _offset;
                _newUnit.transform.rotation = _selectedCell.transform.rotation;

                Undo.RegisterCreatedObjectUndo(_newUnit.gameObject, "Unit painting");
            }
        }

        private void PaintTiles()
        {
            GameObject _cellGrid = gridGameObjectPresent ? GameObject.Find("CellGrid") : gridGameObject;
            if (_cellGrid == null)
            {
                return;
            }

            Cell _selectedCell = GetSelectedCell();
            if (_selectedCell == null)
            {
                return;
            }

            string _mapType = mapTypes[mapTypeIndex];
            Handles.color = Color.red;
            Handles.DrawWireDisc(_selectedCell.transform.position, Vector3.up, _selectedCell.GetCellDimensions().y * (tilePaintingRadius - 0.5f));
            Handles.DrawWireDisc(_selectedCell.transform.position, Vector3.forward, _selectedCell.GetCellDimensions().y * (tilePaintingRadius - 0.5f));
            HandleUtility.Repaint();
            int _selectedCellHash = _selectedCell.GetHashCode();
            if (lastPaintedHash != _selectedCellHash)
            {
                if (Event.current.button == 0 && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown))
                {
                    lastPaintedHash = _selectedCellHash;
                    Undo.SetCurrentGroupName("Tile painting");
                    int _group = Undo.GetCurrentGroup();
                    Cell[] _cells = _cellGrid.GetComponentsInChildren<Cell>();
                    List<Cell> _cellsInRange = _cells.Where(_c => _c.GetDistance(_selectedCell) <= tilePaintingRadius - 1).ToList();
                    foreach (Cell _c in _cellsInRange)
                    {
                        _c.SetCellSo(cellType);
                        cellType.Spawn(_c as TileIsometric);
                    }
                    Undo.CollapseUndoOperations(_group);
                    Undo.IncrementCurrentGroup();
                }
            }
        }

        private Cell GetSelectedCell()
        {
            RaycastHit2D _raycastHit2D = Physics2D.GetRayIntersection(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), Mathf.Infinity);
            if (_raycastHit2D.transform != null && _raycastHit2D.transform.GetComponent<Cell>() != null)
            {
                return _raycastHit2D.transform.GetComponent<Cell>();
            }

            RaycastHit _raycastHit3D;
            bool _isRaycast3D = Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out _raycastHit3D);
            if (_isRaycast3D && _raycastHit3D.transform.GetComponent<Cell>() != null)
            {
                return _raycastHit3D.transform.GetComponent<Cell>();
            }

            return null;
        }

        void GenerateBaseStructure()
        {
            if (GridHelperUtils.CheckMissingParameters(parameterValues))
            {
                return;
            }

            GridHelperUtils.ClearScene();
            
            string _mapType = mapTypes[mapTypeIndex];
            Camera _camera = Camera.main;
            if (_camera == null)
            {
                GameObject _cameraObject = new GameObject("Main Camera");
                _cameraObject.tag = "MainCamera";
                _cameraObject.AddComponent<Camera>();
                _camera = _cameraObject.GetComponent<Camera>();
            }

            if (useMovableCamera)
            {
                _camera.gameObject.AddComponent<CameraController>();
                _camera.gameObject.GetComponent<CameraController>().scrollSpeed = cameraScrollSpeed;
                _camera.gameObject.GetComponent<CameraController>().scrollEdge = cameraScrollEdge;
            }

            cellGrid = new GameObject("CellGrid");
            players = new GameObject("Players");
            objects = new GameObject("Objects");
            guiController = new GameObject("GUIController");

            directionalLight = new GameObject("DirectionalLight");
            Light _light = directionalLight.AddComponent<Light>();
            _light.type = LightType.Directional;
            _light.transform.Rotate(45f, 0, 0);

            for (int _i = 0; _i < nHumanPlayer; _i++)
            {
                GameObject _player = new GameObject(string.Format("Player_{0}", players.transform.childCount));
                _player.AddComponent<HumanPlayer>();
                _player.transform.parent = players.transform;
            }

            for (int _i = 0; _i < nComputerPlayer; _i++)
            {
                GameObject _aiPlayer = new GameObject(string.Format("AI_Player_{0}", players.transform.childCount));
                _aiPlayer.AddComponent<AIPlayer>();
                _aiPlayer.transform.parent = players.transform;
            }

            Type _selectedGenerator = generators[generatorIndex];

            BattleStateManager _cellGridScript = cellGrid.AddComponent<BattleStateManager>();
            CellGridGenerator _generator = (CellGridGenerator)Activator.CreateInstance(_selectedGenerator);
            _generator.CellsParent = cellGrid.transform;

            foreach (string _fieldName in parameterValues.Keys)
            {
                FieldInfo _prop = _generator.GetType().GetField(_fieldName);
                if (null != _prop)
                {
                    _prop.SetValue(_generator, parameterValues[_fieldName]);
                }
            }
            GridInfo _gridInfo = _generator.GenerateGrid();

            _camera.transform.position = _gridInfo.Center;
            _camera.transform.position -= new Vector3(0, 0, (_gridInfo.Dimensions.x > _gridInfo.Dimensions.y ? _gridInfo.Dimensions.x : _gridInfo.Dimensions.y) * Mathf.Sqrt(3) / 2);

            if (_mapType.Equals(_mapType3D))
            {
                Vector3 _rotationVector = new Vector3(90f, 0f, 0f);

                _camera.transform.parent = cellGrid.transform;
                cellGrid.transform.Rotate(_rotationVector);
                players.transform.Rotate(_rotationVector);
                objects.transform.Rotate(_rotationVector);
                directionalLight.transform.Rotate(_rotationVector);

                _camera.transform.parent = null;
                _camera.transform.SetAsFirstSibling();
            }
        }
        
        void GenerateGrid()
        {
            if (GridHelperUtils.CheckMissingParameters(parameterValues))
            {
                return;
            }
            
            cellGrid = GameObject.Find("CellGrid");
            players = GameObject.Find("Players");
            objects = GameObject.Find("Objects");
            guiController = GameObject.Find("GUIController");
            Image _background = GameObject.Find("Environment/Canvas/Background").GetComponent<Image>();

            while (cellGrid.transform.childCount > 0)
            {
                DestroyImmediate(cellGrid.transform.GetChild(0).gameObject);
            }
            
            while (GameObject.Find("Objects").transform.childCount > 0)
            {
                DestroyImmediate(GameObject.Find("Objects").transform.GetChild(0).gameObject);
            }
            
            _background.sprite = null;
            
            string _mapType = mapTypes[mapTypeIndex];

            Type _selectedGenerator = generators[generatorIndex];

            CellGridGenerator _generator = (CellGridGenerator)Activator.CreateInstance(_selectedGenerator);
            _generator.CellsParent = cellGrid.transform;

            foreach (string _fieldName in parameterValues.Keys)
            {
                FieldInfo _prop = _generator.GetType().GetField(_fieldName);
                if (null != _prop)
                {
                    _prop.SetValue(_generator, parameterValues[_fieldName]);
                }
            }
            GridInfo _gridInfo = _generator.GenerateGrid();
            
            Camera _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            _camera.transform.position = _gridInfo.Center;
            _camera.transform.position -= new Vector3(0, 0, (_gridInfo.Dimensions.x > _gridInfo.Dimensions.y ? _gridInfo.Dimensions.x : _gridInfo.Dimensions.y) * Mathf.Sqrt(3) / 2);
            _camera.orthographicSize = _camera.transform.position.x + 1;
            _background.transform.position = _gridInfo.Center;

            if (_mapType.Equals(_mapType3D))
            {
                Vector3 _rotationVector = new Vector3(90f, 0f, 0f);

                _camera.transform.parent = cellGrid.transform;
                cellGrid.transform.Rotate(_rotationVector);
                players.transform.Rotate(_rotationVector);
                objects.transform.Rotate(_rotationVector);
                directionalLight.transform.Rotate(_rotationVector);

                _camera.transform.parent = null;
                _camera.transform.SetAsFirstSibling();
            }
        }

        void EnableSceneViewInteraction()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
        }

        void DisableSceneViewInteraction()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif
        }

        private void OnSelectionChanged()
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }

            if (PrefabUtility.GetPrefabAssetType(Selection.activeGameObject) != PrefabAssetType.NotAPrefab)
            {
                if (tileEditModeOn.value || toToggle == tileEditModeOn)
                {
                    if (Selection.activeGameObject.GetComponent<Cell>() != null)
                    {
                        lastPaintedHash = -1;
                        if (PrefabUtility.GetPrefabInstanceStatus(Selection.activeGameObject) == PrefabInstanceStatus.Connected)
                        {
                            cellType = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject).GetComponent<Cell>().CellSo;
                        }
                        else
                        {
                            cellType = Selection.activeGameObject.GetComponent<Cell>().CellSo;
                        }
                        Repaint();
                    }
                }

                else if (unitEditModeOn.value || toToggle == unitEditModeOn)
                {
                    if (Selection.activeGameObject.GetComponent<GridObjects.GridObject>() != null)
                    {
                        if (PrefabUtility.GetPrefabInstanceStatus(Selection.activeGameObject) == PrefabInstanceStatus.Connected)
                        {
                            unitPrefab = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject).GetComponent<GridObjects.GridObject>();
                        }
                        else
                        {
                            unitPrefab = Selection.activeGameObject.GetComponent<GridObjects.GridObject>();
                        }
                        Repaint();
                    }
                }
            }
        }

        private void OnUndoPerformed()
        {
            lastPaintedHash = -1;
        }

        internal class BoolWrapper
        {
            public bool value;
            public BoolWrapper(bool _value)
            {
                this.value = _value;
            }
        }
    }
}