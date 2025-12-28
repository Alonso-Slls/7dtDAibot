using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Game_7D2D.Modules
{
    /// <summary>
    /// Handles entity scanning and filtering with performance optimizations.
    /// Separates scanning logic from rendering for better architecture.
    /// </summary>
    public static class EntityScanner
    {
        /// <summary>
        /// Scans for enemy entities within the specified distance.
        /// </summary>
        /// <param name="playerPosition">Reference position for distance calculations</param>
        /// <param name="maxDistance">Maximum distance to scan</param>
        /// <returns>List of filtered enemy entities</returns>
        public static List<EntityData> ScanEnemies(Vector3 playerPosition, float maxDistance)
        {
            var entities = new List<EntityData>();
            
            try
            {
                var enemies = GameObject.FindObjectsOfType<EntityEnemy>();
                if (enemies == null) return entities;
                
                foreach (var enemy in enemies)
                {
                    if (enemy == null || !enemy.IsAlive()) continue;
                    
                    float distance = Vector3.Distance(playerPosition, enemy.transform.position);
                    if (distance > maxDistance) continue;
                    
                    entities.Add(new EntityData
                    {
                        Entity = enemy,
                        Color = ESPConfig.EnemyColor,
                        Label = "Enemy",
                        Position = enemy.transform.position,
                        Distance = distance
                    });
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[EntityScanner] Error scanning enemies: {ex.Message}");
            }
            
            return entities;
        }
    }
    
    /// <summary>
    /// Data structure for cached entity information.
    /// </summary>
    public class EntityData
    {
        public EntityEnemy Entity { get; set; }
        public Color Color { get; set; }
        public string Label { get; set; }
        public Vector3 Position { get; set; }
        public float Distance { get; set; }
    }
}
