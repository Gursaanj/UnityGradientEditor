using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GradientEditor : EditorWindow
{
    private const int WindowWidth = 400;
    private const int MinWindowWidth = 200;
    private const int MaxWindowWidth = 1920;
    private const int WindowHeight = 150;
    private const int BorderSize = 10;
    private const int GradientRectHeight = 25;
    private const int KeyWidth = 10;
    private const int KeyHeight = 20;
    private const int SelectedKetOffset = 2;
    private CustomGradient _gradient;
    private Rect _gradientPreviewRect;
    private Rect _keyRect;
    private Rect[] _keyRects;
    private int _selectedKeyIndex = 0;
    private bool _isMouseDownOverKey = false;
    private bool _needsRepaint = false;

    public void SetGradient(CustomGradient gradient)
    {
        _gradient = gradient;
    }

    private void OnEnable()
    {
        titleContent.text = "Gradient Editor";
        position.Set(position.x, position.y,WindowWidth, WindowHeight);
        minSize = new Vector2(MinWindowWidth, WindowHeight);
        maxSize = new Vector2(MaxWindowWidth, WindowHeight);
    }
    
    // Mark the Scene as dirty to save the changes when reopening
    private void OnDisable()
    {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    private void OnGUI()
    {
        Draw();
        HandleInput();

        if (_needsRepaint)
        {
            _needsRepaint = false;
            Repaint();
        }
    }

    private void Draw()
    {
        _gradientPreviewRect = new Rect(BorderSize, BorderSize, position.width - BorderSize*2, GradientRectHeight);
        GUI.DrawTexture(_gradientPreviewRect, _gradient.GetTexture((int) _gradientPreviewRect.width));
        int numberOfKeys = _gradient.NumberOfKeys();

        if (numberOfKeys != -1)
        {
            _keyRects = new Rect[numberOfKeys];
        
            for (int i = 0; i <numberOfKeys; i++)
            { 
                CustomGradient.ColourKey key = _gradient.GetKey(i);
                _keyRect = new Rect(_gradientPreviewRect.x + _gradientPreviewRect.width * key.Time - KeyWidth/2f, _gradientPreviewRect.yMax + BorderSize, KeyWidth, KeyHeight);
                
                //Give a black background to the selected key
                if (i == _selectedKeyIndex)
                {
                    EditorGUI.DrawRect( new Rect(_keyRect.x - SelectedKetOffset, _keyRect.y - SelectedKetOffset, _keyRect.width + (SelectedKetOffset * 2),
                        _keyRect.height + (SelectedKetOffset * 2)), Color.black);
                }

                EditorGUI.DrawRect(_keyRect, key.Colour);
                _keyRects[i] = _keyRect;
            }
            
            Rect settingsRect = new Rect(BorderSize, _keyRect.yMax + BorderSize, position.width -BorderSize*2, position.height);

            using (new GUILayout.AreaScope(settingsRect))
            {
                using (EditorGUI.ChangeCheckScope changeCheck = new EditorGUI.ChangeCheckScope())
                {
                    Color newColour = EditorGUILayout.ColorField(_gradient.GetKey(_selectedKeyIndex).Colour);

                    if (changeCheck.changed)
                    {
                        _gradient.UpdateKeyColour(_selectedKeyIndex, newColour);
                    }
                }

                _gradient.blendMode = (CustomGradient.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", _gradient.blendMode);
                _gradient.shouldRandomizeColour = EditorGUILayout.Toggle("Randomize Colour", _gradient.shouldRandomizeColour);
            }
        }
    }

    private void HandleInput()
    {
        Event guiEvent = Event.current;
       if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            for (int i = 0; i < _keyRects.Length; i++)
            {
                if (_keyRects[i].Contains(guiEvent.mousePosition))
                {
                    _isMouseDownOverKey = true;
                    _selectedKeyIndex = i;
                    _needsRepaint = true;
                    break;
                }
            }


            if (!_isMouseDownOverKey)
            {
                float keyTime = Mathf.InverseLerp(_gradientPreviewRect.x, _gradientPreviewRect.xMax, guiEvent.mousePosition.x);
                Color interpolatedColour = _gradient.Evaluate(keyTime);
                Color randomColour = new Color(Random.value, Random.value, Random.value);
                _selectedKeyIndex = _gradient.AddKey(_gradient.shouldRandomizeColour ? randomColour : interpolatedColour, keyTime);
                _isMouseDownOverKey = true;
                _needsRepaint = true;
            }
        }

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            _isMouseDownOverKey = false;
        }
        
        //Handles dragging of ColourKey
        if (_isMouseDownOverKey && guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
        {
            float keyTime = Mathf.InverseLerp(_gradientPreviewRect.x, _gradientPreviewRect.xMax, guiEvent.mousePosition.x);
            int updatedKeyIndex = _gradient.UpdateKeyTime(_selectedKeyIndex, keyTime);

            if (updatedKeyIndex != -1)
            {
                _selectedKeyIndex = updatedKeyIndex;
                _needsRepaint = true;
            }
        }
        
        //Handles Deleting of ColourKey
        if (guiEvent.type == EventType.KeyDown && (guiEvent.keyCode == KeyCode.Backspace || guiEvent.keyCode == KeyCode.Delete))
        {
            _gradient.RemoveKey(_selectedKeyIndex);
            
            // handles if selectedkey now goes out of range
            if (_selectedKeyIndex >= _gradient.NumberOfKeys())
            {
                _selectedKeyIndex--;
            }

            _needsRepaint = true;
        } 
    }
}
