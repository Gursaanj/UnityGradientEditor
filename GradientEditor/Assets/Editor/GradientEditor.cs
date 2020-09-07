using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GradientEditor : EditorWindow
{
    private const int BorderSize = 10;
    private const int GradientRectHeight = 25;
    private const int KeyWidth = 10;
    private const int KeyHeight = 20;
    private CustomGradient _gradient;

    public void SetGradient(CustomGradient gradient)
    {
        _gradient = gradient;
    }

    private void OnEnable()
    {
        titleContent.text = "Gradient Editor";
    }

    private void OnGUI()
    {
        Event guiEvent = Event.current;
        Rect gradientPreviewRect = new Rect(BorderSize, BorderSize, position.width - BorderSize*2, GradientRectHeight);
        GUI.DrawTexture(gradientPreviewRect, _gradient.GetTexture((int) gradientPreviewRect.width));
        
        for (int i = 0; i < _gradient.NumberOfKeys(); i++)
        { 
            CustomGradient.ColourKey key = _gradient.GetKey(i);
            Rect keyRect = new Rect(gradientPreviewRect.x + gradientPreviewRect.width * key.Time - KeyWidth/2f, gradientPreviewRect.yMax + BorderSize, KeyWidth, KeyHeight);
            EditorGUI.DrawRect(keyRect, key.Colour);
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            Color randomColour = new Color(Random.value, Random.value, Random.value);
            float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
            _gradient.AddKey(randomColour, keyTime);
            Repaint();
        }
    }
}
