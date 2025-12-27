using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SevenDtDAibot.Modules
{
    class ESP
    {
        private static Camera Camera;
        private static Vector3 eb_head, eb_neck, eb_spine, eb_leftshoulder, eb_leftarm, eb_leftforearm, eb_lefthand, eb_rightshoulder, eb_rightarm, eb_rightforearm;
        private static Vector3 eb_righthand, eb_hips, eb_leftupleg, eb_leftleg, eb_leftfoot, eb_rightupleg, eb_rightleg, eb_rightfoot;
        
        // FOV-Aware ESP Configuration
        public static float fovThreshold = 120f; // 120 degrees FOV threshold
        public static bool fovAwareEnabled = true; // Enable/disable FOV-aware rendering
        
        /// <summary>
        /// Checks if an entity is within the player's field of view.
        /// Uses Vector3.Angle to calculate the angle between player's forward vector and direction to target.
        /// </summary>
        /// <param name="targetPosition">World position of the target entity</param>
        /// <returns>True if target is within FOV threshold, false otherwise</returns>
        public static bool IsEntityInFOV(Vector3 targetPosition)
        {
            if (!fovAwareEnabled || GameManager.Instance?.World?.GetPrimaryPlayer() == null || Camera.main == null)
                return true; // Default to visible if FOV-aware is disabled or components missing
            
            try
            {
                var localPlayer = GameManager.Instance.World.GetPrimaryPlayer();
                // Get player's forward direction
                Vector3 playerForward = localPlayer.transform.forward;
                
                // Calculate direction from player to target
                Vector3 directionToTarget = (targetPosition - localPlayer.transform.position).normalized;
                
                // Calculate angle between player's forward vector and direction to target
                float angle = Vector3.Angle(playerForward, directionToTarget);
                
                // Return true if angle is within threshold (half of FOV threshold since we check from center)
                return angle <= fovThreshold / 2f;
            }
            catch (System.Exception)
            {
                return true; // Default to visible on error
            }
        }
        public static void esp_drawBox(EntityEnemy entity, Color color)
        {
            if (HacksManager.Instance == null || HacksManager.Instance.MainCamera == null) return;

            // Performance optimization: early distance culling
            const float maxDist = 100f; 
            var localPlayer = GameManager.Instance.World.GetPrimaryPlayer();
            if (localPlayer == null) return;

            float distSqr = (entity.transform.position - localPlayer.transform.position).sqrMagnitude;
            if (distSqr > maxDist * maxDist) return;

            Vector3 entity_head = entity.transform.position;
            Vector3 entity_feet = new Vector3(entity_head.x, entity_head.y + entity.height, entity_head.z);

            // FOV-Aware ESP: Check if entity is within player's field of view
            if (!IsEntityInFOV(entity_head))
                return; // Skip rendering if outside FOV

            Camera = HacksManager.Instance.MainCamera;
            Vector3 w2s_head = HacksManager.Instance.WorldToScreenPoint(entity_head);
            Vector3 w2s_feet = HacksManager.Instance.WorldToScreenPoint(entity_feet);

            float Distance = Mathf.Sqrt(distSqr);
            Vector3 w2s_test = HacksManager.Instance.WorldToScreenPoint(entity.emodel.GetHeadTransform().position);

            // Enhanced screen bounds checking
            if (w2s_head.z > 0f && w2s_head.x > -50 && w2s_head.x < (float)Screen.width + 50 &&
                w2s_head.y > -50 && w2s_head.y < (float)Screen.height + 50 && Distance <= maxDist)
            {
                if (Config.Settings.ESPBoxes)
                {
                    DrawESPBox(w2s_feet, w2s_head, color, $"{entity.EntityName} [{Mathf.Round(Distance)}m]");
                    DrawESPBox(w2s_test, new Vector3(w2s_test.x - 1f, w2s_test.y - 1f, w2s_test.z), Color.green, "");
                }

                if (Config.Settings.ESPLines)
                {
                    BatchedRenderer.AddLine(new Vector2(w2s_head.x, (float)Screen.height - w2s_head.y), new Vector2((float)Screen.width / 2, (float)Screen.height - 100), color, 1f);
                }

                if (Config.Settings.EnemyBones)
                {
                    // Only compute bones if head is on-screen (saves many W2S calls when off-screen)
                    if (w2s_head.z > 0f && w2s_head.x > 0 && w2s_head.x < (float)Screen.width && w2s_head.y > 0)
                    {
                        try
                        {
                            var smr = entity.GetComponentInChildren<SkinnedMeshRenderer>();
                            if (smr == null || smr.bones == null) return;

                            Transform[] entityBones = smr.bones;
                            int canBone = 0;

                            // Reset bone positions
                            eb_head = eb_neck = eb_spine = Vector3.zero;
                            eb_leftshoulder = eb_leftarm = eb_leftforearm = eb_lefthand = Vector3.zero;
                            eb_rightshoulder = eb_rightarm = eb_rightforearm = eb_righthand = Vector3.zero;
                            eb_hips = eb_leftupleg = eb_leftleg = eb_leftfoot = Vector3.zero;
                            eb_rightupleg = eb_rightleg = eb_rightfoot = Vector3.zero;

                            for (int j = 0; j < entityBones.Length; j++)
                            {
                                if (entityBones[j] == null) continue;
                                var bonePos = entityBones[j].position;
                                var boneName = entityBones[j].name.ToLower();

                                // Enhanced bone name matching with more patterns
                                if (boneName.Contains("head")) { eb_head = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("neck")) { eb_neck = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("spine") || boneName.Contains("chest") || boneName.Contains("torso")) { eb_spine = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("leftshoulder") || boneName.Contains("l_shoulder") || boneName.Contains("l_clavicle")) { eb_leftshoulder = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("leftarm") || boneName.Contains("l_arm") || boneName.Contains("l_upperarm")) { eb_leftarm = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("leftforearm") || boneName.Contains("l_forearm") || boneName.Contains("l_lowerarm")) { eb_leftforearm = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("lefthand") || boneName.Contains("l_hand")) { eb_lefthand = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("rightshoulder") || boneName.Contains("r_shoulder") || boneName.Contains("r_clavicle")) { eb_rightshoulder = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("rightarm") || boneName.Contains("r_arm") || boneName.Contains("r_upperarm")) { eb_rightarm = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("rightforearm") || boneName.Contains("r_forearm") || boneName.Contains("r_lowerarm")) { eb_rightforearm = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("righthand") || boneName.Contains("r_hand")) { eb_righthand = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("hips") || boneName.Contains("pelvis")) { eb_hips = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("leftupleg") || boneName.Contains("l_thigh") || boneName.Contains("l_upperleg")) { eb_leftupleg = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("leftleg") || boneName.Contains("l_leg") || boneName.Contains("l_lowerleg")) { eb_leftleg = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("leftfoot") || boneName.Contains("l_foot")) { eb_leftfoot = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("rightupleg") || boneName.Contains("r_thigh") || boneName.Contains("r_upperleg")) { eb_rightupleg = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("rightleg") || boneName.Contains("r_leg") || boneName.Contains("r_lowerleg")) { eb_rightleg = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                                else if (boneName.Contains("rightfoot") || boneName.Contains("r_foot")) { eb_rightfoot = HacksManager.Instance.WorldToScreenPoint(bonePos); canBone++; }
                            }

                            // Enhanced skeleton drawing with adaptive quality based on distance
                            if (canBone >= 6) // Further reduced threshold for better compatibility
                            {
                                // Use different colors based on distance for better visibility
                                Color boneColor = Distance > 100f ? Color.yellow : Color.green;

                                // Spine
                                if (eb_head != Vector3.zero && eb_neck != Vector3.zero) DrawESPLine(eb_head, eb_neck, boneColor);
                                if (eb_neck != Vector3.zero && eb_spine != Vector3.zero) DrawESPLine(eb_neck, eb_spine, boneColor);
                                if (eb_spine != Vector3.zero && eb_hips != Vector3.zero) DrawESPLine(eb_spine, eb_hips, boneColor);

                                // Left arm
                                if (eb_neck != Vector3.zero && eb_leftshoulder != Vector3.zero) DrawESPLine(eb_neck, eb_leftshoulder, boneColor);
                                if (eb_leftshoulder != Vector3.zero && eb_leftarm != Vector3.zero) DrawESPLine(eb_leftshoulder, eb_leftarm, boneColor);
                                if (eb_leftarm != Vector3.zero && eb_leftforearm != Vector3.zero) DrawESPLine(eb_leftarm, eb_leftforearm, boneColor);
                                if (eb_leftforearm != Vector3.zero && eb_lefthand != Vector3.zero) DrawESPLine(eb_leftforearm, eb_lefthand, boneColor);

                                // Right arm
                                if (eb_neck != Vector3.zero && eb_rightshoulder != Vector3.zero) DrawESPLine(eb_neck, eb_rightshoulder, boneColor);
                                if (eb_rightshoulder != Vector3.zero && eb_rightarm != Vector3.zero) DrawESPLine(eb_rightshoulder, eb_rightarm, boneColor);
                                if (eb_rightarm != Vector3.zero && eb_rightforearm != Vector3.zero) DrawESPLine(eb_rightarm, eb_rightforearm, boneColor);
                                if (eb_rightforearm != Vector3.zero && eb_righthand != Vector3.zero) DrawESPLine(eb_rightforearm, eb_righthand, boneColor);

                                // Left leg
                                if (eb_hips != Vector3.zero && eb_leftupleg != Vector3.zero) DrawESPLine(eb_hips, eb_leftupleg, boneColor);
                                if (eb_leftupleg != Vector3.zero && eb_leftleg != Vector3.zero) DrawESPLine(eb_leftupleg, eb_leftleg, boneColor);
                                if (eb_leftleg != Vector3.zero && eb_leftfoot != Vector3.zero) DrawESPLine(eb_leftleg, eb_leftfoot, boneColor);

                                // Right leg
                                if (eb_hips != Vector3.zero && eb_rightupleg != Vector3.zero) DrawESPLine(eb_hips, eb_rightupleg, boneColor);
                                if (eb_rightupleg != Vector3.zero && eb_rightleg != Vector3.zero) DrawESPLine(eb_rightupleg, eb_rightleg, boneColor);
                                if (eb_rightleg != Vector3.zero && eb_rightfoot != Vector3.zero) DrawESPLine(eb_rightleg, eb_rightfoot, boneColor);
                            }
                        }
                        catch (System.Exception)
                        {
                            // Silently handle bone ESP errors to avoid crashes
                        }
                    }
                }
                
            }
            
        }
        public static void esp_drawBox(EntityItem entity, Color color)
        {
            Vector3 entity_head = entity.transform.position;
            Vector3 entity_feet = new Vector3(entity_head.x, entity_head.y + entity.height, entity_head.z);

            if (HacksManager.Instance == null || HacksManager.Instance.MainCamera == null) return;
            const float maxDist = 100f;
            var localPlayer = GameManager.Instance.World.GetPrimaryPlayer();
            if (localPlayer == null) return;
            float distSqr = (entity.transform.position - localPlayer.transform.position).sqrMagnitude;
            if (distSqr > maxDist * maxDist) return;

            // FOV-Aware ESP: Check if item is within player's field of view
            if (!IsEntityInFOV(entity_head))
                return; // Skip rendering if outside FOV

            Camera = HacksManager.Instance.MainCamera;
            Vector3 w2s_head = HacksManager.Instance.WorldToScreenPoint(entity_head);
            Vector3 w2s_feet = HacksManager.Instance.WorldToScreenPoint(entity_feet);

            float Distance = Mathf.Sqrt(distSqr);

            if (w2s_head.z > 0f && w2s_head.x > 0 && w2s_head.x < (float)Screen.width && w2s_head.y > 0 && Distance <= maxDist)
            {
                DrawESPBox(w2s_feet, w2s_head, color, entity.name);
            }
        }

        public static void esp_drawBox(EntitySupplyCrate entity, Color color)
        {
            Vector3 entity_head = entity.transform.position;
            Vector3 entity_feet = new Vector3(entity_head.x, entity_head.y + entity.height, entity_head.z);

            if (HacksManager.Instance == null || HacksManager.Instance.MainCamera == null) return;
            const float maxDist = 100f;
            var localPlayer = GameManager.Instance.World.GetPrimaryPlayer();
            if (localPlayer == null) return;
            float distSqr = (entity.transform.position - localPlayer.transform.position).sqrMagnitude;
            if (distSqr > maxDist * maxDist) return;

            Camera = HacksManager.Instance.MainCamera;
            Vector3 w2s_head = HacksManager.Instance.WorldToScreenPoint(entity_head);
            Vector3 w2s_feet = HacksManager.Instance.WorldToScreenPoint(entity_feet);

            float Distance = Mathf.Sqrt(distSqr);

            if (w2s_head.z > 0f && w2s_head.x > 0 && w2s_head.x < (float)Screen.width && w2s_head.y > 0 && Distance <= maxDist)
            {
                DrawESPText(w2s_feet, w2s_head, color, entity.name);
            }
        }
        public static void esp_drawBox(EntityNPC entity, Color color)
        {
            Vector3 entity_head = entity.transform.position;
            Vector3 entity_feet = new Vector3(entity_head.x, entity_head.y + entity.height, entity_head.z);

            if (HacksManager.Instance == null || HacksManager.Instance.MainCamera == null) return;
            Camera = HacksManager.Instance.MainCamera;
            Vector3 w2s_head = HacksManager.Instance.WorldToScreenPoint(entity_head);
            Vector3 w2s_feet = HacksManager.Instance.WorldToScreenPoint(entity_feet);

            var localPlayer = GameManager.Instance.World.GetPrimaryPlayer();
            if (localPlayer == null) return;
            float Distance = Vector3.Distance(entity.transform.position, localPlayer.transform.position);

            if (w2s_head.z > 0f && w2s_head.x > 0 && w2s_head.x < (float)Screen.width && w2s_head.y > 0)
            {
                if (Config.Settings.ESPBoxes)
                {
                    DrawESPBox(w2s_feet, w2s_head, color, entity.EntityName);
                }

                if (Config.Settings.ESPLines)
                {
                    BatchedRenderer.AddLine(new Vector2(w2s_head.x, (float)Screen.height - w2s_head.y), new Vector2((float)Screen.width / 2, (float)Screen.height - 100), color, 1f);
                }
            }
        }
        public static void esp_drawBox(EntityPlayer entity, Color color)
        {
            Vector3 entity_head = entity.transform.position;
            Vector3 entity_feet = new Vector3(entity_head.x, entity_head.y + entity.height, entity_head.z);

            if (HacksManager.Instance == null || HacksManager.Instance.MainCamera == null) return;
            const float maxDist = 100f;
            var localPlayer = GameManager.Instance.World.GetPrimaryPlayer();
            if (localPlayer == null) return;
            float distSqr = (entity.transform.position - localPlayer.transform.position).sqrMagnitude;
            if (distSqr > maxDist * maxDist) return;

            // FOV-Aware ESP: Check if player is within player's field of view
            if (!IsEntityInFOV(entity_head))
                return; // Skip rendering if outside FOV

            Camera = HacksManager.Instance.MainCamera;
            Vector3 w2s_head = HacksManager.Instance.WorldToScreenPoint(entity_head);
            Vector3 w2s_feet = HacksManager.Instance.WorldToScreenPoint(entity_feet);
            Vector3 w2s_test = HacksManager.Instance.WorldToScreenPoint(entity.emodel.GetHeadTransform().position);

            float Distance = Mathf.Sqrt(distSqr);

            if (w2s_head.z > 0f && w2s_head.x > 0 && w2s_head.x < (float)Screen.width && w2s_head.y > 0)
            {
                if (Config.Settings.ESPBoxes)
                {
                    if (entity != localPlayer)
                    {
                        if (entity.IsGodMode == true)
                        {
                            DrawESPBox(w2s_feet, w2s_head, color, $"{entity.EntityName} [GOD]");
                        }
                        else
                        {
                            DrawESPBox(w2s_feet, w2s_head, color, entity.EntityName);
                        }
                    }

                    DrawESPBox(w2s_test, new Vector3(w2s_test.x - 1f, w2s_test.y - 1f, w2s_test.z), Color.green, "");
                }

                if (Config.Settings.ESPLines)
                {
                    if (entity != localPlayer)
                    {
                        BatchedRenderer.AddLine(new Vector2(w2s_head.x, (float)Screen.height - w2s_head.y), new Vector2((float)Screen.width / 2, (float)Screen.height - 100), color, 1f);
                    }
                }

            }
        }
        public static void esp_drawBox(EntityAnimal entity, Color color)
        {
            Vector3 entity_head = entity.transform.position;
            Vector3 entity_feet = new Vector3(entity_head.x, entity_head.y + entity.height, entity_head.z);

            if (HacksManager.Instance == null || HacksManager.Instance.MainCamera == null) return;
            const float maxDist = 100f;
            var localPlayer = GameManager.Instance.World.GetPrimaryPlayer();
            if (localPlayer == null) return;
            float distSqr = (entity.transform.position - localPlayer.transform.position).sqrMagnitude;
            if (distSqr > maxDist * maxDist) return;

            // FOV-Aware ESP: Check if animal is within player's field of view
            if (!IsEntityInFOV(entity_head))
                return; // Skip rendering if outside FOV

            Camera = HacksManager.Instance.MainCamera;
            Vector3 w2s_head = HacksManager.Instance.WorldToScreenPoint(entity_head);
            Vector3 w2s_feet = HacksManager.Instance.WorldToScreenPoint(entity_feet);
            Vector3 w2s_test = HacksManager.Instance.WorldToScreenPoint(entity.emodel.GetHeadTransform().position);

            float Distance = Mathf.Sqrt(distSqr);

            if (w2s_head.z > 0f && w2s_head.x > 0 && w2s_head.x < (float)Screen.width && w2s_head.y > 0 && Distance <= maxDist)
            {
                if (Config.Settings.ESPBoxes)
                {
                    DrawESPBox(w2s_feet, w2s_head, color, entity.EntityName.Replace("animal", ""));
                    DrawESPBox(w2s_test, new Vector3(w2s_test.x - 1f, w2s_test.y - 1f, w2s_test.z), Color.green, "");
                }
                if (Config.Settings.ESPLines)
                {
                    BatchedRenderer.AddLine(new Vector2(w2s_head.x, (float)Screen.height - w2s_head.y), new Vector2((float)Screen.width / 2, (float)Screen.height - 100), color, 1f);
                }
            }
        }


        /// <summary>
        /// Draw an ESP line using direct rendering for better performance.
        /// </summary>
        private static void DrawESPLine(Vector3 start, Vector3 end, Color color)
        {
            BatchedRenderer.AddLine(
                new Vector2(start.x, Screen.height - start.y),
                new Vector2(end.x, Screen.height - end.y),
                color,
                1f
            );
        }
        
        /// <summary>
        /// Draw an ESP box using direct rendering for better performance.
        /// </summary>
        private static void DrawESPBox(Vector3 objfootPos, Vector3 objheadPos, Color objColor, String name)
        {
            //Draw Basic ESP Method from Vector3 W2S input
            float height = objheadPos.y - objfootPos.y;
            float widthOffset = 2f;
            float width = height / widthOffset;

            float x = objfootPos.x - (width / 2);
            float y = (float)Screen.height - objfootPos.y - height;

            // Render directly without pooling for better performance
            BatchedRenderer.AddBox(x, y, width, height, objColor, 2f);
            
            if (!string.IsNullOrEmpty(name))
            {
                BatchedRenderer.AddText(new Vector2(x, y), name, Color.white);
            }
        }
        
        /// <summary>
        /// Draw ESP text using object pooling for performance.
        /// </summary>
        private static void DrawESPText(Vector3 objfootPos, Vector3 objheadPos, Color objColor, String name)
        {
            var textData = ESPPool.GetText();
            textData.Set(
                new Vector2(objfootPos.x, (float)Screen.height - objfootPos.y),
                name,
                objColor
            );
            
            BatchedRenderer.AddText(textData.position, textData.text, Color.white);
            
            ESPPool.ReturnText(textData);
            ESPPool.IncrementTextCount();
        }

        /// <summary>
        /// Reset performance counters at the start of each frame.
        /// </summary>
        public static void ResetFrameCounters()
        {
            ESPPool.ResetFrameCounters();
        }

    }
}
