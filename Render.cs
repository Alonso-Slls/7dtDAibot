// Simple Render class for UI compatibility
using UnityEngine;

public static class Render
{
    public static void DrawString(float x, float y, string text, Color color)
    {
        // Empty implementation for UI compatibility
        // Canvas system handles rendering now
    }
    
    public static Vector2 GetStringSize(string text)
    {
        // Return dummy size for UI compatibility
        return new Vector2(100, 20);
    }
}
