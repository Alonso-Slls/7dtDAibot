using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using SevenDtDAibot;

namespace Modules
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
            InitializeCanvas();
            InitializePools();
            CacheCamera();
            
            RobustDebugger.Log("[CanvasESP] Canvas-based ESP system initialized");
        }
        
        void InitializeCanvas()
        {
            if (espCanvas == null)
            {
                // Create canvas if not assigned
                GameObject canvasObj = new GameObject("ESP_Canvas");
                espCanvas = canvasObj.AddComponent<Canvas>();
                espCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                espCanvas.sortingOrder = 1000; // Render on top
                
                // Add CanvasScaler for resolution independence
                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                
                // Add GraphicRaycaster for UI interaction
                canvasObj.AddComponent<GraphicRaycaster>();
                
                DontDestroyOnLoad(canvasObj);
            }
            
            // Create prefabs if not assigned
            CreatePrefabs();
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
            
            Image image = boxObj.AddComponent<Image>();
            image.color = Color.red;
            image.raycastTarget = false; // Disable raycast for performance
            
            ESPBox espBox = boxObj.AddComponent<ESPBox>();
            espBox.image = image;
            
            boxObj.SetActive(false);
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
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
            
            if (mainCamera != null)
            {
                RobustDebugger.Log("[CanvasESP] Main camera cached successfully");
            }
            else
            {
                RobustDebugger.LogWarning("[CanvasESP] No camera found - ESP will not function");
            }
        }
        
        // Main ESP rendering method - replaces the old esp_drawBox
        public void RenderESP(EntityEnemy[] entities)
        {
            if (mainCamera == null) return;
            
            // Clear previous frame's active elements
            ClearInactiveESP(entities);
            
            // Render each entity
            foreach (EntityEnemy entity in entities)
            {
                if (entity == null || !entity.IsAlive()) continue;
                
                RenderEntityESP(entity);
            }
        }
        
        void RenderEntityESP(EntityEnemy entity)
        {
            // Get or create ESP elements for this entity
            if (!activeESPElements.ContainsKey(entity))
            {
                activeESPElements[entity] = new ESPElements();
            }
            
            ESPElements elements = activeESPElements[entity];
            
            // Calculate screen positions
            Vector3 entityPos = entity.transform.position;
            EntityConfig config = GetEntityConfig(entity);
            
            Vector3 headPos = entityPos + Vector3.up * config.headOffset;
            Vector3 feetPos = entityPos;
            
            Vector3 w2s_head = mainCamera.WorldToScreenPoint(headPos);
            Vector3 w2s_feet = mainCamera.WorldToScreenPoint(feetPos);
            
            // Visibility checks
            if (!IsVisibleOnScreen(w2s_head) || !IsVisibleOnScreen(w2s_feet))
            {
                ReturnElementsToPool(elements);
                return;
            }
            
            // Distance check
            float distance = Vector3.Distance(mainCamera.transform.position, entityPos);
            if (distance > SevenDtDAibot.ESPSettings.MaxESPDistance)
            {
                ReturnElementsToPool(elements);
                return;
            }
            
            // Convert to canvas coordinates
            Vector2 screenHead = WorldToCanvasPoint(w2s_head);
            Vector2 screenFeet = WorldToCanvasPoint(w2s_feet);
            
            // Calculate box dimensions
            float boxHeight = Mathf.Abs(screenFeet.y - screenHead.y);
            float boxWidth = CalculateBoxWidth(boxHeight, distance, config);
            
            // Render ESP box
            RenderESPBox(elements, screenHead, boxWidth, boxHeight, Color.red, distance);
            
            // Render entity info
            RenderEntityInfo(elements, screenHead, screenFeet, entity, distance);
        }
        
        void RenderESPBox(ESPElements elements, Vector2 screenHead, float width, float height, Color color, float distance)
        {
            // Get or create box
            if (elements.box == null)
            {
                elements.box = GetBoxFromPool();
            }
            
            ESPBox espBox = elements.box;
            espBox.image.color = color;
            
            // Position and size the box
            RectTransform rectTransform = espBox.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = screenHead;
            rectTransform.sizeDelta = new Vector2(width, height);
            
            espBox.gameObject.SetActive(true);
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
        Vector2 WorldToCanvasPoint(Vector3 worldToScreenPoint)
        {
            // Convert from Unity screen coordinates to canvas coordinates
            return new Vector2(worldToScreenPoint.x, Screen.height - worldToScreenPoint.y);
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
            float distanceScale = Mathf.Clamp01(1f - (distance / SevenDtDAibot.ESPSettings.MaxESPDistance) * 0.3f);
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
