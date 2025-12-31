// DEPRECATED: This file is kept for reference but is no longer used
// The ESP functionality has been moved to CanvasESPManager.cs for better performance
// using Unity's Canvas system instead of the legacy OnGUI system
//
// Migration completed: OnGUI -> Canvas-based rendering
// Benefits: Better performance, object pooling, automatic batching, reduced garbage collection
//
// Use CanvasESPManager instead for all ESP rendering needs.

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Modules
{
    [System.Obsolete("ESP class is deprecated. Use CanvasESPManager instead.", true)]
    public class ESP
    {
        // This class is intentionally left empty as it's deprecated
        // All ESP functionality has been moved to CanvasESPManager
    }
}
