using UnityEngine;

[CreateAssetMenu(menuName = "Create new OutlineConfig", fileName = "OutlineConfig", order = 0)]
public class OutlineConfig : ScriptableObject
{
    public Outline.Mode outlineMode;
    public Color normalColor;
    public float outlineDefaultWidth = 1;
    
    public Color selectedColor;
    
    public float outlineSelectedWidth = 10;
}