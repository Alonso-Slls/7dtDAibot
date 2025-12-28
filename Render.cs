using UnityEngine;

public static class Render
{
    private static Texture2D whiteTexture;
    
    static Render()
    {
        // Create a simple white texture for drawing
        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
    }
    
    public static void DrawBox(float x, float y, float width, float height, Color color, bool filled = true)
    {
        GUI.color = color;
        
        if (filled)
        {
            GUI.DrawTexture(new Rect(x, y, width, height), whiteTexture);
        }
        else
        {
            // Draw box outline (4 lines)
            GUI.DrawTexture(new Rect(x, y, width, 1), whiteTexture); // Top
            GUI.DrawTexture(new Rect(x, y + height - 1, width, 1), whiteTexture); // Bottom
            GUI.DrawTexture(new Rect(x, y, 1, height), whiteTexture); // Left
            GUI.DrawTexture(new Rect(x + width - 1, y, 1, height), whiteTexture); // Right
        }
        
        GUI.color = Color.white;
    }
    
    public static void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1f)
    {
        GUI.color = color;
        
        // Calculate line direction and length
        float dx = x2 - x1;
        float dy = y2 - y1;
        float length = Mathf.Sqrt(dx * dx + dy * dy);
        
        if (length < 0.01f) return; // Line too short
        
        // Calculate angle
        float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        
        // Save current GUI matrix
        GUIUtility.RotateAroundPivot(-angle, new Vector2(x1, y1));
        
        // Draw the line as a rotated rectangle
        GUI.DrawTexture(new Rect(x1, y1 - thickness/2, length, thickness), whiteTexture);
        
        // Restore GUI matrix
        GUIUtility.RotateAroundPivot(angle, new Vector2(x1, y1));
        
        GUI.color = Color.white;
    }
    
    public static void DrawString(float x, float y, string text, Color color)
    {
        GUI.color = color;
        GUI.Label(new Rect(x, y, 200, 20), text);
        GUI.color = Color.white;
    }
    
    public static void DrawCircle(float x, float y, float radius, Color color, bool filled = true)
    {
        if (filled)
        {
            GUI.color = color;
            GUI.DrawTexture(new Rect(x - radius, y - radius, radius * 2, radius * 2), whiteTexture);
            GUI.color = Color.white;
        }
        else
        {
            // Draw circle outline using multiple lines
            int segments = 32;
            float angleStep = 360f / segments;
            
            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * angleStep * Mathf.Deg2Rad;
                float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;
                
                float x1 = x + Mathf.Cos(angle1) * radius;
                float y1 = y + Mathf.Sin(angle1) * radius;
                float x2 = x + Mathf.Cos(angle2) * radius;
                float y2 = y + Mathf.Sin(angle2) * radius;
                
                DrawLine(x1, y1, x2, y2, color, 1f);
            }
        }
    }
    
    public static void DrawCrosshair(float x, float y, float size, Color color, float thickness = 1f)
    {
        // Draw horizontal line
        DrawLine(x - size, y, x + size, y, color, thickness);
        
        // Draw vertical line
        DrawLine(x, y - size, x, y + size, color, thickness);
    }
    
    public static Vector2 GetStringSize(string text)
    {
        return GUI.skin.label.CalcSize(new GUIContent(text));
    }
    
    public static void DrawHealthBar(float x, float y, float width, float height, float health, float maxHealth)
    {
        // Draw background
        DrawBox(x, y, width, height, Color.black, true);
        
        // Calculate health percentage
        float healthPercent = Mathf.Clamp01(health / maxHealth);
        
        // Determine color based on health
        Color healthColor;
        if (healthPercent > 0.6f)
            healthColor = Color.green;
        else if (healthPercent > 0.3f)
            healthColor = Color.yellow;
        else
            healthColor = Color.red;
        
        // Draw health fill
        DrawBox(x, y, width * healthPercent, height, healthColor, true);
        
        // Draw border
        DrawBox(x, y, width, height, Color.white, false);
    }
}
