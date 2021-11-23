﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cells;
using Editor.GridGenerators;
using Grid;
using Grid.UnitGenerators;
using GridObjects;
using Gui;
using Players;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    class GridHelper : EditorWindow
    {
        private static string MAP_TYPE_3D = "XZ Plane (3D)";
        private static string MAP_TYPE_2D = "XY Plane (2D)";

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
        [SerializeField] private CellSO tilePrefab;
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
            EditorWindow window = GetWindow(typeof(GridHelper));
            window.titleContent.text = "Grid Helper";
        }

        void Initialize()
        {
            if (parameterValues == null)
            {
                parameterValues = new Dictionary<string, object>();
            }

            if (generators == null)
            {
                Type generatorInterface = typeof(ICellGridGenerator);
                Assembly assembly = generatorInterface.Assembly;

                generators = new List<Type>();
                foreach (Type t in assembly.GetTypes())
                {
                    if (generatorInterface.IsAssignableFrom(t) && t != generatorInterface)
                        generators.Add(t);
                }
            }

            if (generatorNames == null)
            {
                generatorNames = generators.Select(t => t.Name).ToArray();
            }

            if (mapTypes == null)
            {
                mapTypes = new string[2];
                mapTypes[0] = MAP_TYPE_2D;
                mapTypes[1] = MAP_TYPE_3D;
            }
        }

        public void OnEnable()
        {
            Initialize();

            GameObject gridGameObject = GameObject.Find("CellGrid");
            GameObject unitsGameObject = GameObject.Find("Objects");

            gridGameObjectPresent = gridGameObject != null;
            unitsGameObjectPresent = unitsGameObject != null;

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
            GameObject gridGameObject = GameObject.Find("CellGrid");
            GameObject unitsGameObject = GameObject.Find("Objects");

            gridGameObjectPresent = gridGameObject != null;
            unitsGameObjectPresent = unitsGameObject != null;

            if (unitsGameObject != null)
            {
                this.unitsGameObject = null;
            }
            if (gridGameObject != null)
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

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.R)
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
                string path = EditorUtility.SaveFolderPanel("Save prefabs", "", "");
                if (path.Length != 0)
                {
                    path = path.Replace(Application.dataPath, "Assets");

                    GameObject[] objectArray = Selection.gameObjects;
                    for (int i = 0; i < objectArray.Length; i++)
                    {
                        GameObject gameObject = objectArray[i];
                        string localPath = path + "/" + gameObject.name + ".prefab";
                        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                        PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction);
                    }
                    Debug.Log(string.Format("{0} prefabs saved to {1}", objectArray.Length, path));
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
                    GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                    style.normal.textColor = Color.red;
                    GUILayout.Label("Unit parent GameObject missing", style);
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

                GameObject UnitsParent = unitsGameObjectPresent ? GameObject.Find("Objects") : unitsGameObject;
                if (UnitsParent == null)
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
                    GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                    style.normal.textColor = Color.red;
                    GUILayout.Label("CellGrid GameObject missing", style);
                }
                gridGameObject = (GameObject)EditorGUILayout.ObjectField("CellGrid", gridGameObject, typeof(GameObject), true, new GUILayoutOption[0]);
            }
            tilePaintingRadius = EditorGUILayout.IntSlider(new GUIContent("Brush radius"), tilePaintingRadius, 1, 4);
            EditorGUI.BeginChangeCheck();
            tilePrefab = (CellSO)EditorGUILayout.ObjectField("Tile prefab", tilePrefab, typeof(CellSO), true, new GUILayoutOption[0]);
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

                GameObject CellGrid = gridGameObjectPresent ? GameObject.Find("CellGrid") : gridGameObject;
                if (CellGrid == null)
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

            foreach (FieldInfo field in generators[generatorIndex].GetFields().Where(f => f.IsPublic))
            {
                if (field.FieldType == typeof(int))
                {
                    int x = 0;
                    if (parameterValues.ContainsKey(field.Name))
                    {
                        x = (int)(parameterValues[field.Name]);
                    }
                    x = EditorGUILayout.IntField(new GUIContent(field.Name), x);
                    parameterValues[field.Name] = x;
                }
                else if (field.FieldType == typeof(GameObject))
                {
                    GameObject g = null;
                    if (parameterValues.ContainsKey(field.Name))
                    {
                        g = (GameObject)(parameterValues[field.Name]);
                    }
                    g = (GameObject)EditorGUILayout.ObjectField(field.Name, g, typeof(GameObject), false, new GUILayoutOption[0]);
                    parameterValues[field.Name] = g;

                    if (mapTypes[mapTypeIndex].Equals(MAP_TYPE_3D) && g != null && g.GetComponent<Collider2D>() != null)
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
                GUIStyle style = new GUIStyle(EditorStyles.wordWrappedLabel);
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.red;
                GUILayout.Label("You are trying to generate a map on XZ plane with a prefab containg a 2D collider. 2D colliders will not work in XZ axis. Add a 3D collider to your tile prefab or select XY plane", style);
            }

            if (GUILayout.Button("Generate scene"))
            {
                string dialogTitle = "Confirm delete";
                string dialogMessage = "This will delete all objects on the scene. Do you wish to continue?";
                string dialogOK = "Ok";
                string dialogCancel = "Cancel";

                bool shouldDelete = EditorUtility.DisplayDialog(dialogTitle, dialogMessage, dialogOK, dialogCancel);
                if (shouldDelete)
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
                string dialogTitle = "Confirm delete";
                string dialogMessage = "This will delete all objects on the scene. Do you wish to continue?";
                string dialogOK = "Ok";
                string dialogCancel = "Cancel";

                bool shouldDelete = EditorUtility.DisplayDialog(dialogTitle, dialogMessage, dialogOK, dialogCancel);
                if (shouldDelete)
                {
                    Undo.ClearAll();
                    GridHelperUtils.ClearScene();
                }
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.R)
            {
                ToggleEditMode();
            }

            if (tileEditModeOn.value || unitEditModeOn.value)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            if (tileEditModeOn.value && tilePrefab != null)
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
            GameObject UnitsParent = unitsGameObjectPresent ? GameObject.Find("Objects") : unitsGameObject;
            if (UnitsParent == null)
            {
                return;
            }

            Cell selectedCell = GetSelectedCell();
            if (selectedCell == null)
            {
                return;
            }

            string mapType = mapTypes[mapTypeIndex];
            Handles.color = Color.red;
            Handles.DrawWireDisc(selectedCell.transform.position, Vector3.up, selectedCell.GetCellDimensions().y / 2);
            Handles.DrawWireDisc(selectedCell.transform.position, Vector3.forward, selectedCell.GetCellDimensions().y / 2);
            HandleUtility.Repaint();
            if (Event.current.button == 0 && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)) 
            {
                if (unitEditModeOn.value && selectedCell.IsTaken)
                {
                    return;
                }

                Undo.SetCurrentGroupName("Unit painting");
                int group = Undo.GetCurrentGroup();

                Undo.RecordObject(selectedCell, "Unit painting");
                
                GridObject newUnit = (PrefabUtility.InstantiatePrefab(unitPrefab.gameObject) as GameObject).GetComponent<GridObject>();
                selectedCell.Take(newUnit);

                Vector3 offset = new Vector3(0, 0, selectedCell.GetCellDimensions().z);
                newUnit.transform.position = selectedCell.transform.position;
                newUnit.transform.parent = UnitsParent.transform;
                newUnit.transform.localPosition -= offset;
                newUnit.transform.rotation = selectedCell.transform.rotation;
                newUnit.Cell = selectedCell;

                Undo.RegisterCreatedObjectUndo(newUnit.gameObject, "Unit painting");
            }
        }

        private void PaintTiles()
        {
            GameObject CellGrid = gridGameObjectPresent ? GameObject.Find("CellGrid") : gridGameObject;
            if (CellGrid == null)
            {
                return;
            }

            Cell selectedCell = GetSelectedCell();
            if (selectedCell == null)
            {
                return;
            }

            string mapType = mapTypes[mapTypeIndex];
            Handles.color = Color.red;
            Handles.DrawWireDisc(selectedCell.transform.position, Vector3.up, selectedCell.GetCellDimensions().y * (tilePaintingRadius - 0.5f));
            Handles.DrawWireDisc(selectedCell.transform.position, Vector3.forward, selectedCell.GetCellDimensions().y * (tilePaintingRadius - 0.5f));
            HandleUtility.Repaint();
            int selectedCellHash = selectedCell.GetHashCode();
            if (lastPaintedHash != selectedCellHash)
            {
                if (Event.current.button == 0 && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown))
                {
                    lastPaintedHash = selectedCellHash;
                    Undo.SetCurrentGroupName("Tile painting");
                    int group = Undo.GetCurrentGroup();
                    Cell[] cells = CellGrid.GetComponentsInChildren<Cell>();
                    List<Cell> cellsInRange = cells.Where(c => c.GetDistance(selectedCell) <= tilePaintingRadius - 1).ToList();
                    foreach (Cell c in cellsInRange)
                    {
                        c.CellSO = tilePrefab;
                        tilePrefab.Spawn(c as TileIsometric);
                    }
                    Undo.CollapseUndoOperations(group);
                    Undo.IncrementCurrentGroup();
                }
            }
        }

        private Cell GetSelectedCell()
        {
            RaycastHit2D raycastHit2D = Physics2D.GetRayIntersection(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), Mathf.Infinity);
            if (raycastHit2D.transform != null && raycastHit2D.transform.GetComponent<Cell>() != null)
            {
                return raycastHit2D.transform.GetComponent<Cell>();
            }

            RaycastHit raycastHit3D;
            bool isRaycast3D = Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out raycastHit3D);
            if (isRaycast3D && raycastHit3D.transform.GetComponent<Cell>() != null)
            {
                return raycastHit3D.transform.GetComponent<Cell>();
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
            
            string mapType = mapTypes[mapTypeIndex];
            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                cameraObject.AddComponent<Camera>();
                camera = cameraObject.GetComponent<Camera>();
            }

            if (useMovableCamera)
            {
                camera.gameObject.AddComponent<CameraController>();
                camera.gameObject.GetComponent<CameraController>().scrollSpeed = cameraScrollSpeed;
                camera.gameObject.GetComponent<CameraController>().scrollEdge = cameraScrollEdge;
            }

            cellGrid = new GameObject("CellGrid");
            players = new GameObject("Players");
            objects = new GameObject("Objects");
            guiController = new GameObject("GUIController");

            directionalLight = new GameObject("DirectionalLight");
            Light light = directionalLight.AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.Rotate(45f, 0, 0);

            for (int i = 0; i < nHumanPlayer; i++)
            {
                GameObject player = new GameObject(string.Format("Player_{0}", players.transform.childCount));
                player.AddComponent<HumanPlayer>();
                player.GetComponent<Player>().playerNumber = players.transform.childCount;
                player.transform.parent = players.transform;
            }

            for (int i = 0; i < nComputerPlayer; i++)
            {
                GameObject aiPlayer = new GameObject(string.Format("AI_Player_{0}", players.transform.childCount));
                aiPlayer.AddComponent<AIPlayer>();
                aiPlayer.GetComponent<Player>().playerNumber = players.transform.childCount;
                aiPlayer.transform.parent = players.transform;
            }

            Type selectedGenerator = generators[generatorIndex];

            BattleStateManager cellGridScript = cellGrid.AddComponent<BattleStateManager>();
            ICellGridGenerator generator = (ICellGridGenerator)Activator.CreateInstance(selectedGenerator);
            generator.CellsParent = cellGrid.transform;

            CustomUnitGenerator unitGenerator = cellGrid.AddComponent<CustomUnitGenerator>();
            unitGenerator.unitsParent = objects.transform;
            unitGenerator.cellsParent = cellGrid.transform;

            foreach (string fieldName in parameterValues.Keys)
            {
                FieldInfo prop = generator.GetType().GetField(fieldName);
                if (null != prop)
                {
                    prop.SetValue(generator, parameterValues[fieldName]);
                }
            }
            GridInfo gridInfo = generator.GenerateGrid();

            camera.transform.position = gridInfo.Center;
            camera.transform.position -= new Vector3(0, 0, (gridInfo.Dimensions.x > gridInfo.Dimensions.y ? gridInfo.Dimensions.x : gridInfo.Dimensions.y) * Mathf.Sqrt(3) / 2);

            if (mapType.Equals(MAP_TYPE_3D))
            {
                Vector3 rotationVector = new Vector3(90f, 0f, 0f);

                camera.transform.parent = cellGrid.transform;
                cellGrid.transform.Rotate(rotationVector);
                players.transform.Rotate(rotationVector);
                objects.transform.Rotate(rotationVector);
                directionalLight.transform.Rotate(rotationVector);

                camera.transform.parent = null;
                camera.transform.SetAsFirstSibling();
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
            SpriteRenderer background = GameObject.Find("Background").GetComponent<SpriteRenderer>();

            while (cellGrid.transform.childCount > 0)
            {
                DestroyImmediate(cellGrid.transform.GetChild(0).gameObject);
            }
            
            while (GameObject.Find("Objects").transform.childCount > 0)
            {
                DestroyImmediate(GameObject.Find("Objects").transform.GetChild(0).gameObject);
            }
            
            background.sprite = null;
            
            string mapType = mapTypes[mapTypeIndex];

            Type selectedGenerator = generators[generatorIndex];

            ICellGridGenerator generator = (ICellGridGenerator)Activator.CreateInstance(selectedGenerator);
            generator.CellsParent = cellGrid.transform;

            CustomUnitGenerator unitGenerator = cellGrid.GetComponent<CustomUnitGenerator>();
            unitGenerator.unitsParent = objects.transform;
            unitGenerator.cellsParent = cellGrid.transform;

            foreach (string fieldName in parameterValues.Keys)
            {
                FieldInfo prop = generator.GetType().GetField(fieldName);
                if (null != prop)
                {
                    prop.SetValue(generator, parameterValues[fieldName]);
                }
            }
            GridInfo gridInfo = generator.GenerateGrid();
            
            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            camera.transform.position = gridInfo.Center;
            camera.transform.position -= new Vector3(0, 0, (gridInfo.Dimensions.x > gridInfo.Dimensions.y ? gridInfo.Dimensions.x : gridInfo.Dimensions.y) * Mathf.Sqrt(3) / 2);
            camera.orthographicSize = camera.transform.position.x + 1;
            background.transform.position = gridInfo.Center;

            if (mapType.Equals(MAP_TYPE_3D))
            {
                Vector3 rotationVector = new Vector3(90f, 0f, 0f);

                camera.transform.parent = cellGrid.transform;
                cellGrid.transform.Rotate(rotationVector);
                players.transform.Rotate(rotationVector);
                objects.transform.Rotate(rotationVector);
                directionalLight.transform.Rotate(rotationVector);

                camera.transform.parent = null;
                camera.transform.SetAsFirstSibling();
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
                            tilePrefab = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject).GetComponent<Cell>().CellSO;
                        }
                        else
                        {
                            tilePrefab = Selection.activeGameObject.GetComponent<Cell>().CellSO;
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
            public BoolWrapper(bool value)
            {
                this.value = value;
            }
        }
    }
}