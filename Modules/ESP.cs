using UnityEngine;
using System.Collections.Generic;

namespace Modules
{
    public class ESP
    {
        public static void esp_drawBox(EntityEnemy entity, Color color)
        {
            if (entity == null || !entity.IsAlive()) return;
            
            // Get entity position
            Vector3 entityPos = entity.transform.position;
            
            // Convert to screen coordinates
            Vector3 w2s_head = Camera.main.WorldToScreenPoint(entityPos + Vector3.up * 1.8f);
            Vector3 w2s_feet = Camera.main.WorldToScreenPoint(entityPos);
            
            // Check if entity is in front of camera
            if (w2s_head.z <= 0 || w2s_feet.z <= 0) return;
            
            // Check distance (100m max)
            float distance = Vector3.Distance(Camera.main.transform.position, entityPos);
            if (distance > 100f) return;
            
            // Invert Y for GUI coordinates
            w2s_head.y = Screen.height - w2s_head.y;
            w2s_feet.y = Screen.height - w2s_feet.y;
            
            // Calculate box dimensions
            float height = w2s_feet.y - w2s_head.y;
            float width = height * 0.4f; // Width is 40% of height
            
            // Draw ESP box
            DrawESPBox(w2s_head.x, w2s_head.y, width, height, color);
            
            // Draw entity name
            string entityName = entity.name;
            Render.DrawString(w2s_head.x, w2s_head.y - 15, entityName, Color.white);
            
            // Draw distance
            Render.DrawString(w2s_head.x, w2s_feet.y + 5, $"{distance:F1}m", Color.yellow);
            
            // Draw bones if enabled
            if (ESPSettings.ShowEnemyBones)
            {
                DrawBones(entity);
            }
        }
        
        public static void DrawESPBox(float x, float y, float width, float height, Color color)
        {
            // Draw box outline
            Render.DrawBox(x - width/2, y, width, height, color, false);
            
            // Draw corner lines for better visibility
            float cornerLength = 10f;
            
            // Top-left corner
            Render.DrawLine(x - width/2, y, x - width/2 + cornerLength, y, color);
            Render.DrawLine(x - width/2, y, x - width/2, y + cornerLength, color);
            
            // Top-right corner
            Render.DrawLine(x + width/2, y, x + width/2 - cornerLength, y, color);
            Render.DrawLine(x + width/2, y, x + width/2, y + cornerLength, color);
            
            // Bottom-left corner
            Render.DrawLine(x - width/2, y + height, x - width/2 + cornerLength, y + height, color);
            Render.DrawLine(x - width/2, y + height, x - width/2, y + height - cornerLength, color);
            
            // Bottom-right corner
            Render.DrawLine(x + width/2, y + height, x + width/2 - cornerLength, y + height, color);
            Render.DrawLine(x + width/2, y + height, x + width/2, y + height - cornerLength, color);
        }
        
        public static void DrawBones(EntityEnemy entity)
        {
            try
            {
                // Get the entity's skinned mesh renderer
                SkinnedMeshRenderer[] renderers = entity.GetComponentsInChildren<SkinnedMeshRenderer>();
                
                foreach (var renderer in renderers)
                {
                    if (renderer == null) continue;
                    
                    // Get bones
                    Transform[] bones = renderer.bones;
                    if (bones == null || bones.Length == 0) continue;
                    
                    // Find important bones and draw connections
                    Transform head = null;
                    Transform spine = null;
                    Transform leftArm = null;
                    Transform rightArm = null;
                    Transform leftLeg = null;
                    Transform rightLeg = null;
                    
                    foreach (var bone in bones)
                    {
                        if (bone == null) continue;
                        
                        string boneName = bone.name.ToLower();
                        
                        if (boneName.Contains("head"))
                            head = bone;
                        else if (boneName.Contains("spine") || boneName.Contains("chest"))
                            spine = bone;
                        else if (boneName.Contains("arm") && boneName.Contains("left"))
                            leftArm = bone;
                        else if (boneName.Contains("arm") && boneName.Contains("right"))
                            rightArm = bone;
                        else if (boneName.Contains("leg") && boneName.Contains("left"))
                            leftLeg = bone;
                        else if (boneName.Contains("leg") && boneName.Contains("right"))
                            rightLeg = bone;
                    }
                    
                    // Draw bone connections
                    if (head != null && spine != null)
                        DrawBoneLine(head, spine, Color.green);
                        
                    if (spine != null)
                    {
                        if (leftArm != null)
                            DrawBoneLine(spine, leftArm, Color.green);
                        if (rightArm != null)
                            DrawBoneLine(spine, rightArm, Color.green);
                        if (leftLeg != null)
                            DrawBoneLine(spine, leftLeg, Color.green);
                        if (rightLeg != null)
                            DrawBoneLine(spine, rightLeg, Color.green);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error drawing bones: {e.Message}");
            }
        }
        
        private static void DrawBoneLine(Transform bone1, Transform bone2, Color color)
        {
            if (bone1 == null || bone2 == null) return;
            
            Vector3 w2s_bone1 = Camera.main.WorldToScreenPoint(bone1.position);
            Vector3 w2s_bone2 = Camera.main.WorldToScreenPoint(bone2.position);
            
            // Check if bones are in front of camera
            if (w2s_bone1.z <= 0 || w2s_bone2.z <= 0) return;
            
            // Invert Y for GUI coordinates
            w2s_bone1.y = Screen.height - w2s_bone1.y;
            w2s_bone2.y = Screen.height - w2s_bone2.y;
            
            // Draw line between bones
            Render.DrawLine(w2s_bone1.x, w2s_bone1.y, w2s_bone2.x, w2s_bone2.y, color);
        }
    }
}
