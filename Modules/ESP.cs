// COMPATIBILITY LAYER: Redirects old ESP calls to new Canvas system
// This file provides backward compatibility while using the new CanvasESPManager

using UnityEngine;

namespace Modules
{
    public class ESP
    {
        // Legacy method - redirects to Canvas system (no-op for now)
        // The actual ESP rendering is handled by CanvasESPManager in Hacks.cs
        public static void esp_drawBox(EntityEnemy entity, Color color)
        {
            // This method is no longer used - ESP rendering is handled by CanvasESPManager
            // Keeping this for compilation compatibility with any remaining references
        }
    }
}
