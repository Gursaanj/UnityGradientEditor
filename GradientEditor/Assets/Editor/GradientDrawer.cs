using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CustomGradient))]
public class GradientDrawer : PropertyDrawer
{
    private const float LabelBufferValue = 5.0f;

    // public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    // {
    //     //Use this to change the height of the property
    // }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Get Access to current Event
        Event guiEvent = Event.current;
        
        // Need to convert SerializedProperty into customGradient object to use its methods
        CustomGradient gradient = (CustomGradient) fieldInfo.GetValue(property.serializedObject.targetObject);
        
        //Get the rect besides the label on the inspector
        float labelWidth = GUI.skin.label.CalcSize(label).x + LabelBufferValue;
        Rect textureRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
        
        if (guiEvent.type == EventType.Repaint)
        {
            GUI.Label(position, new GUIContent(label.text, "Double Click to Open the GradientEditor"));
        
            // Create a new GUIStyle to preview gradient : Possible 2017 only issue
            // GUIStyle gradientStyle = new GUIStyle();
            // gradientStyle.normal.background = gradient.GetTexture((int) position.width);
            // GUI.Label(textureRect, GUIContent.none, gradientStyle);
        
            GUI.DrawTexture(textureRect, gradient.GetTexture((int) position.width));
        }
        else
        {
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.clickCount == 2)
            {
                if (textureRect.Contains(guiEvent.mousePosition))
                {
                    // If window is already opened GetWindow will refocus window
                    EditorWindow.GetWindow<GradientEditor>();
                }
            }
        }
    }
}
