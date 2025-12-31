using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using SevenDtDAibot;

namespace Game_7D2D
{
    public class CanvasESPManager : MonoBehaviour
    {
        [Header("Canvas Setup")]
        [SerializeField] private Canvas espCanvas;
        [SerializeField] private GameObject espBoxPrefab;
        [SerializeField] private GameObject espTextPrefab;
        [SerializeField] private GameObject espLinePrefab;
        
        [Header("Performance Settings")]
        [SerializeField] private int maxPooledBoxes = 50;
        [SerializeField] private int maxPooledTexts = 100;
        [SerializeField] private int maxPooledLines = 200;
        
        // Object pools
        private Queue<ESPBox> boxPool = new Queue<ESPBox>();
        private Queue<ESPText> textPool = new Queue<ESPText>();
        private Queue<ESPLine> linePool = new Queue<ESPLine>();
        
        // Active elements tracking
        private Dictionary<EntityEnemy, ESPElements> activeESPElements = new Dictionary<EntityEnemy, ESPElements>();
        
        // Camera cache
        private Camera mainCamera;
        
        // Entity configurations
        private struct EntityConfig
        {
            public float height;
            public float widthMultiplier;
            public float headOffset;
        }
        
        private static readonly Dictionary<string, EntityConfig> ENTITY_CONFIGS = new Dictionary<string, EntityConfig>
        {
            {"zombie", new EntityConfig { height = 1.8f, widthMultiplier = 0.6f, headOffset = 1.7f }},
            {"animal", new EntityConfig { height = 1.2f, widthMultiplier = 0.8f, headOffset = 1.0f }},
            {"player", new EntityConfig { height = 1.8f, widthMultiplier = 0.4f, headOffset = 1.7f }},
            {"default", new EntityConfig { height = 1.8f, widthMultiplier = 0.5f, headOffset = 1.7f }}
        };
        
        void Awake()
        {
            RobustDebugger.Log("[CanvasESP] Awake called - starting initialization");
            InitializeCanvas();
            RobustDebugger.Log("[CanvasESP] Canvas initialized");
            InitializePools();
            RobustDebugger.Log("[CanvasESP] Pools initialized");
            CacheCamera();
            RobustDebugger.Log("[CanvasESP] Canvas-based ESP system initialized successfully");
        }
        
        void InitializeCanvas()
        {
            RobustDebugger.Log("[CanvasESP] Initializing canvas...");
            if (espCanvas == null)
            {
                // Create canvas if not assigned
                GameObject canvasObj = new GameObject("ESP_Canvas");
                espCanvas = canvasObj.AddComponent<Canvas>();
                espCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                espCanvas.sortingOrder = 1000; // Render on top
                RobustDebugger.Log("[CanvasESP] Canvas created with ScreenSpaceOverlay mode");
                
                // Set canvas to match OnGUI coordinate system (top-left origin)
                RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
                canvasRect.anchorMin = Vector2.zero;
                canvasRect.anchorMax = Vector2.one;
                canvasRect.sizeDelta = Vector2.zero;
                canvasRect.anchoredPosition = Vector2.zero;
                
                // Add CanvasScaler for resolution independence
                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                scaler.scaleFactor = 1f;
                scaler.referencePixelsPerUnit = 100;
                
                // Add GraphicRaycaster for UI interaction
                canvasObj.AddComponent<GraphicRaycaster>();
                
                DontDestroyOnLoad(canvasObj);
                RobustDebugger.Log("[CanvasESP] Canvas setup completed with OnGUI-compatible coordinates");
            }
            
            // Create prefabs if not assigned
            CreatePrefabs();
            RobustDebugger.Log("[CanvasESP] Prefabs created");
        }
        
        void CreatePrefabs()
        {
            // ESP Box Prefab
            if (espBoxPrefab == null)
            {
                espBoxPrefab = CreateBoxPrefab();
            }
            
            // ESP Text Prefab  
            if (espTextPrefab == null)
            {
                espTextPrefab = CreateTextPrefab();
            }
            
            // ESP Line Prefab
            if (espLinePrefab == null)
            {
                espLinePrefab = CreateLinePrefab();
            }
        }
        
        GameObject CreateBoxPrefab()
        {
            GameObject boxObj = new GameObject("ESP_Box");
            boxObj.transform.SetParent(espCanvas.transform);
            
            // Create 4 separate lines for box outline
            // Top line
            GameObject topLine = new GameObject("TopLine");
            topLine.transform.SetParent(boxObj.transform);
            Image topImage = topLine.AddComponent<Image>();
            topImage.color = Color.red;
            topImage.raycastTarget = false;
            RectTransform topRect = topLine.GetComponent<RectTransform>();
            topRect.anchorMin = new Vector2(0, 1);
            topRect.anchorMax = new Vector2(1, 1);
            topRect.sizeDelta = new Vector2(0, 2); // 2px thick top line
            topRect.anchoredPosition = Vector2.zero;
            
            // Bottom line
            GameObject bottomLine = new GameObject("BottomLine");
            bottomLine.transform.SetParent(boxObj.transform);
            Image bottomImage = bottomLine.AddComponent<Image>();
            bottomImage.color = Color.red;
            bottomImage.raycastTarget = false;
            RectTransform bottomRect = bottomLine.GetComponent<RectTransform>();
            bottomRect.anchorMin = new Vector2(0, 0);
            bottomRect.anchorMax = new Vector2(1, 0);
            bottomRect.sizeDelta = new Vector2(0, 2); // 2px thick bottom line
            bottomRect.anchoredPosition = Vector2.zero;
            
            // Left line
            GameObject leftLine = new GameObject("LeftLine");
            leftLine.transform.SetParent(boxObj.transform);
            Image leftImage = leftLine.AddComponent<Image>();
            leftImage.color = Color.red;
            leftImage.raycastTarget = false;
            RectTransform leftRect = leftLine.GetComponent<RectTransform>();
            leftRect.anchorMin = new Vector2(0, 0);
            leftRect.anchorMax = new Vector2(0, 1);
            leftRect.sizeDelta = new Vector2(2, 0); // 2px thick left line
            leftRect.anchoredPosition = Vector2.zero;
            
            // Right line
            GameObject rightLine = new GameObject("RightLine");
            rightLine.transform.SetParent(boxObj.transform);
            Image rightImage = rightLine.AddComponent<Image>();
            rightImage.color = Color.red;
            rightImage.raycastTarget = false;
            RectTransform rightRect = rightLine.GetComponent<RectTransform>();
            rightRect.anchorMin = new Vector2(1, 0);
            rightRect.anchorMax = new Vector2(1, 1);
            rightRect.sizeDelta = new Vector2(2, 0); // 2px thick right line
            rightRect.anchoredPosition = Vector2.zero;
            
            ESPBox espBox = boxObj.AddComponent<ESPBox>();
            espBox.image = topImage; // Use top image as reference
            
            boxObj.SetActive(false);
            RobustDebugger.Log("[CanvasESP] Box outline prefab created with 4 border lines");
            return boxObj;
        }
        
        GameObject CreateTextPrefab()
        {
            GameObject textObj = new GameObject("ESP_Text");
            textObj.transform.SetParent(espCanvas.transform);
            
            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 12;
            text.color = Color.white;
            text.raycastTarget = false;
            
            ESPText espText = textObj.AddComponent<ESPText>();
            espText.text = text;
            
            textObj.SetActive(false);
            return textObj;
        }
        
        GameObject CreateLinePrefab()
        {
            GameObject lineObj = new GameObject("ESP_Line");
            lineObj.transform.SetParent(espCanvas.transform);
            
            Image image = lineObj.AddComponent<Image>();
            image.color = Color.green;
            image.raycastTarget = false;
            
            ESPLine espLine = lineObj.AddComponent<ESPLine>();
            espLine.image = image;
            
            lineObj.SetActive(false);
            return lineObj;
        }
        
        void InitializePools()
        {
            // Pre-warm box pool
            for (int i = 0; i < maxPooledBoxes; i++)
            {
                GameObject box = Instantiate(espBoxPrefab, espCanvas.transform);
                box.SetActive(false);
                boxPool.Enqueue(box.GetComponent<ESPBox>());
            }
            
            // Pre-warm text pool
            for (int i = 0; i < maxPooledTexts; i++)
            {
                GameObject text = Instantiate(espTextPrefab, espCanvas.transform);
                text.SetActive(false);
                textPool.Enqueue(text.GetComponent<ESPText>());
            }
            
            // Pre-warm line pool
            for (int i = 0; i < maxPooledLines; i++)
            {
                GameObject line = Instantiate(espLinePrefab, espCanvas.transform);
                line.SetActive(false);
                linePool.Enqueue(line.GetComponent<ESPLine>());
            }
            
            RobustDebugger.Log($"[CanvasESP] Pools initialized: {maxPooledBoxes} boxes, {maxPooledTexts} texts, {maxPooledLines} lines");
        }
        
        void CacheCamera()
        {
            // Use Camera.main directly like the old ESP code
            mainCamera = Camera.main;
            
            if (mainCamera != null)
            {
                RobustDebugger.Log("[CanvasESP] Main camera cached successfully");
            }
            else
            {
                RobustDebugger.LogWarning("[CanvasESP] Camera.main not found - ESP will not function");
            }
        }
        
        // Main ESP rendering method - replaces the old esp_drawBox
        public void RenderESP(EntityEnemy[] entities)
        {
            RobustDebugger.Log($"[CanvasESP] RenderESP called with {entities.Length} entities");
            
            if (mainCamera == null) 
            {
                RobustDebugger.LogWarning("[CanvasESP] No camera available for ESP rendering");
                return;
            }
            
            // Clear previous frame's active elements
            ClearInactiveESP(entities);
            
            // Render each entity
            int renderedCount = 0;
            foreach (EntityEnemy entity in entities)
            {
                if (entity == null || !entity.IsAlive()) continue;
                
                RenderEntityESP(entity);
                renderedCount++;
            }
            
            if (renderedCount > 0)
            {
                RobustDebugger.Log($"[CanvasESP] Successfully rendered ESP for {renderedCount} entities");
            }
        }
        
        void RenderEntityESP(EntityEnemy entity)
        {
            RobustDebugger.Log($"[CanvasESP] Rendering ESP for entity: {entity.EntityName}");
            
            // Get or create ESP elements for this entity
            if (!activeESPElements.ContainsKey(entity))
            {
                activeESPElements[entity] = new ESPElements();
            }
            
            ESPElements elements = activeESPElements[entity];
            
            // Calculate screen positions using the same method as old ESP
            Vector3 entity_head = entity.transform.position;
            Vector3 entity_feet = new Vector3(entity_head.x, entity_head.y + entity.height, entity_head.z);
            
            Vector3 w2s_head = mainCamera.WorldToScreenPoint(entity_head);
            Vector3 w2s_feet = mainCamera.WorldToScreenPoint(entity_feet);
            
            RobustDebugger.Log($"[CanvasESP] Entity {entity.EntityName} - Head: {w2s_head}, Feet: {w2s_feet}");
            
            // Distance calculation using local player
            float distance = Vector3.Distance(entity.transform.position, GetLocalPlayerPosition());
            
            // Visibility checks using old ESP method
            if (w2s_head.z <= 0f || w2s_head.x <= 0 || w2s_head.x >= Screen.width || w2s_head.y <= 0)
            {
                RobustDebugger.Log($"[CanvasESP] Entity {entity.EntityName} not visible on screen");
                ReturnElementsToPool(elements);
                return;
            }
            
            // Distance check using old ESP limit (100f)
            if (distance > 100f)
            {
                RobustDebugger.Log($"[CanvasESP] Entity {entity.EntityName} too far: {distance}m");
                ReturnElementsToPool(elements);
                return;
            }
            
            // Convert to canvas coordinates
            Vector2 screenHead = WorldToCanvasPoint(w2s_head);
            Vector2 screenFeet = WorldToCanvasPoint(w2s_feet);
            
            RobustDebugger.Log($"[CanvasESP] Canvas coords - Head: {screenHead}, Feet: {screenFeet}");
            
            // Calculate box dimensions
            float boxHeight = Mathf.Abs(screenFeet.y - screenHead.y);
            float boxWidth = CalculateBoxWidth(boxHeight, distance, GetEntityConfig(entity));
            
            RobustDebugger.Log($"[CanvasESP] Box dimensions - Width: {boxWidth}, Height: {boxHeight}");
            
            // Render ESP box
            RenderESPBox(elements, screenHead, boxWidth, boxHeight, Color.red, distance);
            
            // Render entity info
            RenderEntityInfo(elements, screenHead, screenFeet, entity, distance);
            
            RobustDebugger.Log($"[CanvasESP] Successfully rendered ESP for {entity.EntityName}");
        }
        
        void RenderESPBox(ESPElements elements, Vector2 screenHead, float width, float height, Color color, float distance)
        {
            RobustDebugger.Log($"[CanvasESP] Rendering box at {screenHead} with size {width}x{height}");
            
            // Get or create box
            if (elements.box == null)
            {
                elements.box = GetBoxFromPool();
                RobustDebugger.Log("[CanvasESP] Created new box from pool");
            }
            
            ESPBox espBox = elements.box;
            espBox.image.color = color;
            
            // Position and size the box
            RectTransform rectTransform = espBox.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = screenHead;
            rectTransform.sizeDelta = new Vector2(width, height);
            
            RobustDebugger.Log($"[CanvasESP] Box positioned at {rectTransform.anchoredPosition} with size {rectTransform.sizeDelta}");
            
            espBox.gameObject.SetActive(true);
            RobustDebugger.Log("[CanvasESP] Box activated and visible");
        }
        
        void RenderEntityInfo(ESPElements elements, Vector2 screenHead, Vector2 screenFeet, EntityEnemy entity, float distance)
        {
            // Entity name text
            if (elements.nameText == null)
            {
                elements.nameText = GetTextFromPool();
            }
            
            string entityName = entity.EntityName;
            elements.nameText.text.text = entityName;
            elements.nameText.text.color = Color.white;
            
            RectTransform nameRect = elements.nameText.GetComponent<RectTransform>();
            nameRect.anchoredPosition = new Vector2(screenHead.x, screenHead.y - 20);
            elements.nameText.gameObject.SetActive(true);
            
            // Distance text
            if (elements.distanceText == null)
            {
                elements.distanceText = GetTextFromPool();
            }
            
            string distanceText = $"{distance:F1}m";
            elements.distanceText.text.text = distanceText;
            elements.distanceText.text.color = Color.yellow;
            
            RectTransform distanceRect = elements.distanceText.GetComponent<RectTransform>();
            distanceRect.anchoredPosition = new Vector2(screenHead.x, screenFeet.y + 15);
            elements.distanceText.gameObject.SetActive(true);
        }
        
        void ClearInactiveESP(EntityEnemy[] currentEntities)
        {
            HashSet<EntityEnemy> currentEntitySet = new HashSet<EntityEnemy>(currentEntities);
            
            List<EntityEnemy> entitiesToRemove = new List<EntityEnemy>();
            
            foreach (var kvp in activeESPElements)
            {
                if (!currentEntitySet.Contains(kvp.Key) || kvp.Key == null || !kvp.Key.IsAlive())
                {
                    ReturnElementsToPool(kvp.Value);
                    entitiesToRemove.Add(kvp.Key);
                }
            }
            
            foreach (EntityEnemy entity in entitiesToRemove)
            {
                activeESPElements.Remove(entity);
            }
        }
        
        void ReturnElementsToPool(ESPElements elements)
        {
            if (elements.box != null)
            {
                ReturnBoxToPool(elements.box);
                elements.box = null;
            }
            
            if (elements.nameText != null)
            {
                ReturnTextToPool(elements.nameText);
                elements.nameText = null;
            }
            
            if (elements.distanceText != null)
            {
                ReturnTextToPool(elements.distanceText);
                elements.distanceText = null;
            }
            
            // Return all lines
            foreach (ESPLine line in elements.lines)
            {
                ReturnLineToPool(line);
            }
            elements.lines.Clear();
        }
        
        // Pool management methods
        ESPBox GetBoxFromPool()
        {
            if (boxPool.Count > 0)
            {
                return boxPool.Dequeue();
            }
            
            // Create new if pool is empty
            GameObject newBox = Instantiate(espBoxPrefab, espCanvas.transform);
            return newBox.GetComponent<ESPBox>();
        }
        
        void ReturnBoxToPool(ESPBox box)
        {
            box.gameObject.SetActive(false);
            boxPool.Enqueue(box);
        }
        
        ESPText GetTextFromPool()
        {
            if (textPool.Count > 0)
            {
                return textPool.Dequeue();
            }
            
            GameObject newText = Instantiate(espTextPrefab, espCanvas.transform);
            return newText.GetComponent<ESPText>();
        }
        
        void ReturnTextToPool(ESPText text)
        {
            text.gameObject.SetActive(false);
            textPool.Enqueue(text);
        }
        
        ESPLine GetLineFromPool()
        {
            if (linePool.Count > 0)
            {
                return linePool.Dequeue();
            }
            
            GameObject newLine = Instantiate(espLinePrefab, espCanvas.transform);
            return newLine.GetComponent<ESPLine>();
        }
        
        void ReturnLineToPool(ESPLine line)
        {
            line.gameObject.SetActive(false);
            linePool.Enqueue(line);
        }
        
        // Helper methods
        Vector3 GetLocalPlayerPosition()
        {
            // Try to find local player like the old ESP code
            EntityPlayer localPlayer = FindObjectOfType<EntityPlayer>();
            if (localPlayer != null)
            {
                return localPlayer.transform.position;
            }
            
            // Fallback to camera position
            if (mainCamera != null)
            {
                return mainCamera.transform.position;
            }
            
            return Vector3.zero;
        }
        
        Vector2 WorldToCanvasPoint(Vector3 worldToScreenPoint)
        {
            // Use old ESP coordinate system directly for now
            // Old ESP used: new Vector2(w2s_head.x, (float)Screen.height - w2s_head.y)
            float oldX = worldToScreenPoint.x;
            float oldY = Screen.height - worldToScreenPoint.y;
            
            RobustDebugger.Log($"[CanvasESP] Using old ESP coords: Screen({worldToScreenPoint.x}, {worldToScreenPoint.y}) -> Old({oldX}, {oldY})");
            
            return new Vector2(oldX, oldY);
        }
        
        bool IsVisibleOnScreen(Vector3 worldToScreenPoint)
        {
            return worldToScreenPoint.z > 0 && 
                   worldToScreenPoint.x >= 0 && 
                   worldToScreenPoint.x <= Screen.width &&
                   worldToScreenPoint.y >= 0 && 
                   worldToScreenPoint.y <= Screen.height;
        }
        
        EntityConfig GetEntityConfig(EntityEnemy entity)
        {
            string entityName = entity.EntityName.ToLower();
            
            foreach (var kvp in ENTITY_CONFIGS)
            {
                if (entityName.Contains(kvp.Key))
                    return kvp.Value;
            }
            
            return ENTITY_CONFIGS["default"];
        }
        
        float CalculateBoxWidth(float height, float distance, EntityConfig config)
        {
            float baseWidth = height * config.widthMultiplier;
            float distanceScale = Mathf.Clamp01(1f - (distance / ESPSettings.MaxESPDistance) * 0.3f);
            return baseWidth * (0.8f + distanceScale * 0.4f);
        }
        
        void OnDestroy()
        {
            // Cleanup all pooled objects
            foreach (ESPBox box in boxPool)
            {
                if (box != null && box.gameObject != null)
                    Destroy(box.gameObject);
            }
            
            foreach (ESPText text in textPool)
            {
                if (text != null && text.gameObject != null)
                    Destroy(text.gameObject);
            }
            
            foreach (ESPLine line in linePool)
            {
                if (line != null && line.gameObject != null)
                    Destroy(line.gameObject);
            }
            
            RobustDebugger.Log("[CanvasESP] Canvas ESP system cleaned up");
        }
    }
    
    // Helper classes for pooled components
    public class ESPBox : MonoBehaviour
    {
        public Image image;
    }
    
    public class ESPText : MonoBehaviour
    {
        public Text text;
    }
    
    public class ESPLine : MonoBehaviour
    {
        public Image image;
    }
    
    // Container class for entity ESP elements
    public class ESPElements
    {
        public ESPBox box;
        public ESPText nameText;
        public ESPText distanceText;
        public List<ESPLine> lines = new List<ESPLine>();
    }
}
