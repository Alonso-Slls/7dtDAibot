using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using SevenDtDAibot.Modules; 

namespace SevenDtDAibot
{
    /// <summary>
    /// Legacy Hacks class - now delegates to HacksManager.
    /// Maintained for backward compatibility during transition.
    /// All functionality moved to HacksManager singleton instance.
    /// </summary>
    class Hacks : MonoBehaviour
    {
        #region Legacy Static Properties (Deprecated)
        
        /// <summary>
        /// [DEPRECATED] Use HacksManager.Instance.MainCamera instead.
        /// </summary>
        [Obsolete("Use HacksManager.Instance.MainCamera instead")]
        public static Camera MainCamera => HacksManager.Instance?.MainCamera;
        
        /// <summary>
        /// [DEPRECATED] Use HacksManager.Instance.Timer instead.
        /// </summary>
        [Obsolete("Use HacksManager.Instance.Timer instead")]
        public static float Timer => HacksManager.Instance?.Timer ?? 0f;
        
        /// <summary>
        /// [DEPRECATED] Use EntityTracker<EntityEnemy>.Instance.GetEntities() instead.
        /// </summary>
        [Obsolete("Use EntityTracker<EntityEnemy>.Instance.GetEntities() instead")]
        public static List<EntityEnemy> eEnemy => EntityTracker<EntityEnemy>.Instance?.GetAllEntities().ToList() ?? new List<EntityEnemy>();
        
        /// <summary>
        /// [DEPRECATED] Use EntityTracker<EntityPlayer>.Instance.GetEntities() instead.
        /// </summary>
        [Obsolete("Use EntityTracker<EntityPlayer>.Instance.GetEntities() instead")]
        public static List<EntityPlayer> ePlayers => EntityTracker<EntityPlayer>.Instance?.GetAllEntities().ToList() ?? new List<EntityPlayer>();
        
        /// <summary>
        /// [DEPRECATED] Use EntityTracker<EntityItem>.Instance.GetEntities() instead.
        /// </summary>
        [Obsolete("Use EntityTracker<EntityItem>.Instance.GetEntities() instead")]
        public static List<EntityItem> eItem => EntityTracker<EntityItem>.Instance?.GetAllEntities().ToList() ?? new List<EntityItem>();
        
        /// <summary>
        /// [DEPRECATED] Use EntityTracker<EntityNPC>.Instance.GetEntities() instead.
        /// </summary>
        [Obsolete("Use EntityTracker<EntityNPC>.Instance.GetEntities() instead")]
        public static List<EntityNPC> eNPC => EntityTracker<EntityNPC>.Instance?.GetAllEntities().ToList() ?? new List<EntityNPC>();
        
        /// <summary>
        /// [DEPRECATED] Use EntityTracker<EntityAnimal>.Instance.GetEntities() instead.
        /// </summary>
        [Obsolete("Use EntityTracker<EntityAnimal>.Instance.GetEntities() instead")]
        public static List<EntityAnimal> eAnimal => EntityTracker<EntityAnimal>.Instance?.GetAllEntities().ToList() ?? new List<EntityAnimal>();
        
        /// <summary>
        /// [DEPRECATED] Legacy zombie list - use EntityTracker<EntityEnemy>.Instance instead.
        /// </summary>
        [Obsolete("Use EntityTracker<EntityEnemy>.Instance.GetEntities() instead")]
        public static List<EntityZombie> eZombie => new List<EntityZombie>();
        
        /// <summary>
        /// [DEPRECATED] Legacy loot list - handle separately if needed.
        /// </summary>
        [Obsolete("Handle EntitySupplyCrate tracking separately if needed")]
        public static List<EntitySupplyCrate> eLoot => new List<EntitySupplyCrate>();
        
        /// <summary>
        /// [DEPRECATED] Use GameManager.Instance.World.GetPrimaryPlayerId() instead.
        /// </summary>
        [Obsolete("Use GameManager.Instance.World.GetPrimaryPlayerId() instead")]
        public static LocalPlayer localP => null;
        
        /// <summary>
        /// [DEPRECATED] Use EntityPlayerLocal instead.
        /// </summary>
        [Obsolete("Use EntityPlayerLocal from GameManager instead")]
        public static EntityPlayerLocal eLocalPlayer => null;
        
        /// <summary>
        /// [DEPRECATED] Use HacksManager.Instance.W2SCount instead.
        /// </summary>
        [Obsolete("Use HacksManager.Instance.W2SCount instead")]
        public static int W2SCount => HacksManager.Instance?.W2SCount ?? 0;
        
        /// <summary>
        /// [DEPRECATED] Use HacksManager.Instance.PerformanceStats instead.
        /// </summary>
        [Obsolete("Use HacksManager.Instance.PerformanceStats instead")]
        public static long lastScanMs => HacksManager.Instance != null ? long.Parse(HacksManager.Instance.PerformanceStats.Split(':')[1].Replace("ms", "")) : 0;
        
        /// <summary>
        /// [DEPRECATED] Use UI.Menu instead.
        /// </summary>
        [Obsolete("Use UI.Menu instead")]
        public static bool Menu => UI.Menu;
        
        /// <summary>
        /// [DEPRECATED] Use HacksManager.Instance.IsLoaded instead.
        /// </summary>
        [Obsolete("Use HacksManager.Instance.IsLoaded instead")]
        public static bool isLoaded => HacksManager.Instance?.IsLoaded ?? false;
        
        #endregion
        
        #region Legacy Static Methods (Deprecated)
        
        /// <summary>
        /// [DEPRECATED] Use HacksManager.Instance.WorldToScreenPoint() instead.
        /// </summary>
        [Obsolete("Use HacksManager.Instance.WorldToScreenPoint() instead")]
        public static Vector3 WorldToScreenPoint(Vector3 pos)
        {
            return HacksManager.Instance?.WorldToScreenPoint(pos) ?? Vector3.zero;
        }
        
        #endregion
        
        #region Unity Lifecycle
        
        /// <summary>
        /// Ensures HacksManager instance exists.
        /// </summary>
        private void Awake()
        {
            // Ensure HacksManager is initialized
            if (HacksManager.Instance == null)
            {
                var go = new GameObject("HacksManager");
                go.AddComponent<HacksManager>();
            }
        }
        
        /// <summary>
        /// Delegates all functionality to HacksManager.
        /// </summary>
        private void Start()
        {
            // All functionality moved to HacksManager
            // This class is now just a compatibility layer
            ErrorHandler.LogWarning("Hacks", "This class is deprecated. Use HacksManager.Instance instead.");
        }
        
        /// <summary>
        /// Delegates all functionality to HacksManager.
        /// </summary>
        private void Update()
        {
            // All functionality moved to HacksManager
            // This class is now just a compatibility layer
        }
        
        /// <summary>
        /// Delegates all functionality to HacksManager.
        /// </summary>
        private void OnGUI()
        {
            // All functionality moved to HacksManager
            // This class is now just a compatibility layer
        }
        
        #endregion
        
        #region Legacy Methods (No-Op)
        
        /// <summary>
        /// [DEPRECATED] Functionality moved to HacksManager.
        /// </summary>
        [Obsolete("Functionality moved to HacksManager")]
        public static void updateObjects()
        {
            // No-op - functionality moved to HacksManager
        }
        
        /// <summary>
        /// [DEPRECATED] Functionality moved to HacksManager.
        /// </summary>
        [Obsolete("Functionality moved to HacksManager")]
        public static void RenderESPWithSubscription()
        {
            // No-op - functionality moved to ESPRenderer
        }
        
        /// <summary>
        /// [DEPRECATED] Functionality moved to HacksManager.
        /// </summary>
        [Obsolete("Functionality moved to HacksManager")]
        public static void Unload()
        {
            // No-op - functionality moved to HacksManager
        }
        
        /// <summary>
        /// [DEPRECATED] Functionality moved to HacksManager.
        /// </summary>
        [Obsolete("Functionality moved to HacksManager")]
        public void stopCoro()
        {
            // No-op - functionality moved to HacksManager
        }
        
        /// <summary>
        /// [DEPRECATED] Functionality moved to HacksManager.
        /// </summary>
        [Obsolete("Functionality moved to HacksManager")]
        private void checkState()
        {
            // No-op - functionality moved to HacksManager
        }
        
        #endregion
    }
}
