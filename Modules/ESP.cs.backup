using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Modules
{
    public class ESP
    {
        // ESP functionality - only box drawing
        
        // Entity type configurations
        private static readonly Dictionary<string, EntityConfig> ENTITY_CONFIGS = new Dictionary<string, EntityConfig>
        {
            {"zombie", new EntityConfig { height = 1.8f, widthMultiplier = 0.6f, headOffset = 1.7f }},
            {"animal", new EntityConfig { height = 1.2f, widthMultiplier = 0.8f, headOffset = 1.0f }},
            {"player", new EntityConfig { height = 1.8f, widthMultiplier = 0.4f, headOffset = 1.7f }},
            {"default", new EntityConfig { height = 1.8f, widthMultiplier = 0.5f, headOffset = 1.7f }}
        };
        
        public struct EntityConfig
        {
            public float height;
            public float widthMultiplier;
            public float headOffset;
        }
        public static void esp_drawBox(EntityEnemy entity, Color color)
        {
            if (entity == null || !entity.IsAlive()) return;
            
            // Get entity configuration based on type
            EntityConfig config = GetEntityConfig(entity);
            
            // Get entity position
            Vector3 entityPos = entity.transform.position;
            
            // Calculate head and feet positions with proper offsets
            Vector3 headPos = entityPos + Vector3.up * config.headOffset;
            Vector3 feetPos = entityPos;
            
            // Convert to screen coordinates
            Vector3 w2s_head = Camera.main.WorldToScreenPoint(headPos);
            Vector3 w2s_feet = Camera.main.WorldToScreenPoint(feetPos);
            
            // Enhanced visibility checks
            if (!IsVisibleOnScreen(w2s_head) || !IsVisibleOnScreen(w2s_feet)) return;
            
            // Check distance
            float distance = Vector3.Distance(Camera.main.transform.position, entityPos);
            if (distance > ESPSettings.MaxESPDistance) return;
            
            // Convert to GUI coordinates
            Vector2 screenHead = WorldToGUIPoint(w2s_head);
            Vector2 screenFeet = WorldToGUIPoint(w2s_feet);
            
            // Calculate dynamic box dimensions with distance scaling
            float boxHeight = screenFeet.y - screenHead.y;
            float boxWidth = CalculateBoxWidth(boxHeight, distance, config);
            
            // Draw enhanced ESP box
            DrawEnhancedESPBox(screenHead.x, screenHead.y, boxWidth, boxHeight, color, distance);
            
            // Draw entity information
            DrawEntityInfo(screenHead, screenFeet, entity, distance);
        }
        
        #region Helper Methods
        
        private static EntityConfig GetEntityConfig(EntityEnemy entity)
        {
            string entityName = entity.name.ToLower();
            
            foreach (var kvp in ENTITY_CONFIGS)
            {
                if (entityName.Contains(kvp.Key))
                    return kvp.Value;
            }
            
            return ENTITY_CONFIGS["default"];
        }
        
        private static bool IsVisibleOnScreen(Vector3 worldToScreenPoint)
        {
            return worldToScreenPoint.z > 0 && 
                   worldToScreenPoint.x >= 0 && 
                   worldToScreenPoint.x <= Screen.width &&
                   worldToScreenPoint.y >= 0 && 
                   worldToScreenPoint.y <= Screen.height;
        }
        
        private static Vector2 WorldToGUIPoint(Vector3 worldToScreenPoint)
        {
            // Convert from Unity screen coordinates to GUI coordinates
            // Unity: (0,0) is bottom-left, GUI: (0,0) is top-left
            return new Vector2(worldToScreenPoint.x, Screen.height - worldToScreenPoint.y);
        }
        
        private static float CalculateBoxWidth(float height, float distance, EntityConfig config)
        {
            // Base width from entity configuration
            float baseWidth = height * config.widthMultiplier;
            
            // Distance scaling - boxes get smaller at distance
            float distanceScale = Mathf.Clamp01(1f - (distance / ESPSettings.MaxESPDistance) * 0.3f);
            
            return baseWidth * (0.8f + distanceScale * 0.4f);
        }
        
        private static void DrawEntityInfo(Vector2 screenHead, Vector2 screenFeet, EntityEnemy entity, float distance)
        {
            // Entity name above box - center it properly
            string entityName = entity.name;
            Vector2 nameSize = Render.GetStringSize(entityName);
            Render.DrawString(screenHead.x - nameSize.x / 2, screenHead.y - 20, entityName, Color.white);
            
            // Distance below box - center it
            string distanceText = $"{distance:F1}m";
            Vector2 distanceSize = Render.GetStringSize(distanceText);
            Render.DrawString(screenHead.x - distanceSize.x / 2, screenFeet.y + 5, distanceText, Color.yellow);
            
            // Health indicator disabled for compatibility
            // TODO: Implement proper health display based on game API
        }
        
        #endregion
        
        private static void DrawEnhancedESPBox(float x, float y, float width, float height, Color color, float distance)
        {
            // Ensure coordinates are positive and within screen bounds
            x = Mathf.Max(0, x);
            y = Mathf.Max(0, y);
            width = Mathf.Max(1, width);
            height = Mathf.Max(1, height);
            
            // Main box outline - use filled = false for outline only
            Render.DrawBox(x - width/2, y, width, height, color, false);
            
            // Dynamic corner length based on distance
            float cornerLength = Mathf.Max(3f, 10f * (1f - distance / ESPSettings.MaxESPDistance));
            
            // Enhanced corner lines with better visibility
            float halfWidth = width / 2f;
            
            // Top-left corner
            Render.DrawLine(x - halfWidth, y, x - halfWidth + cornerLength, y, color, 2f);
            Render.DrawLine(x - halfWidth, y, x - halfWidth, y + cornerLength, color, 2f);
            
            // Top-right corner
            Render.DrawLine(x + halfWidth, y, x + halfWidth - cornerLength, y, color, 2f);
            Render.DrawLine(x + halfWidth, y, x + halfWidth, y + cornerLength, color, 2f);
            
            // Bottom-left corner
            Render.DrawLine(x - halfWidth, y + height, x - halfWidth + cornerLength, y + height, color, 2f);
            Render.DrawLine(x - halfWidth, y + height, x - halfWidth, y + height - cornerLength, color, 2f);
            
            // Bottom-right corner
            Render.DrawLine(x + halfWidth, y + height, x + halfWidth - cornerLength, y + height, color, 2f);
            Render.DrawLine(x + halfWidth, y + height, x + halfWidth, y + height - cornerLength, color, 2f);
            
            // Add center crosshair for better targeting (only at close range)
            if (distance < 30f)
            {
                float crossSize = 4f;
                Render.DrawLine(x - crossSize, y + height/2, x + crossSize, y + height/2, color, 1f);
                Render.DrawLine(x, y + height/2 - crossSize, x, y + height/2 + crossSize, color, 1f);
            }
        }
    }
}
