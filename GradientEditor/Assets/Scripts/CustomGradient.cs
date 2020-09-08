using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomGradient
{
    public BlendMode blendMode;
    public bool shouldRandomizeColour;
    [SerializeField] private List<ColourKey> keys = new List<ColourKey>();
    
    [Serializable]
    public struct ColourKey
    {
        [SerializeField] private Color colour;
        [SerializeField] private float time;

        public ColourKey(Color colour, float time)
        {
            this.colour = colour;
            this.time = time;
        }

        public Color Colour => colour;
        public float Time => time;
    }
    
    public enum BlendMode
    {
        Linear,
        Discrete
    }

    public CustomGradient()
    {
        AddKey(Color.white, 0);
        AddKey(Color.black, 1);
    }

    public Color Evaluate(float time)
    {
        if (keys == null || keys.Count == 0)
        {
            return Color.white;
        }

        ColourKey keyLeft = keys[0];
        ColourKey keyRight = keys[keys.Count - 1];

        for (int i = 0; i < keys.Count; i++)
        {
            ColourKey currentKey = keys[i];
            
            //if the designated time is in between two consecutive keys Create new KeysLeft and KeysRight 
            if (currentKey.Time <= time)
            {
                keyLeft = currentKey;
            }

            if (currentKey.Time >= time)
            {
                keyRight = currentKey;
                break;
            }
        }
        
        //return blend between ketLeft and keyRight depending on blendmode

        switch (blendMode)
        {
            case BlendMode.Linear:
            {
                // Will convert time to a float (0,1) in reference to keyLeft(0) and keyRight(1)
                float blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
                return Color.Lerp(keyLeft.Colour, keyRight.Colour, blendTime);
            }
            case BlendMode.Discrete:
            {
                return keyRight.Colour;
            }
            default:
            {
                // Will convert time to a float (0,1) in reference to keyLeft(0) and keyRight(1)
                float blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
                return Color.Lerp(keyLeft.Colour, keyRight.Colour, blendTime);
            }
        }
    }
    
    /// <summary>
    /// Adds Colour Key to List and returns index placed
    /// </summary>
    /// <param name="colour"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public int AddKey(Color colour, float time)
    {
        ColourKey newKey = new ColourKey(colour, time);

        if (keys == null)
        {
            Debug.LogError("The list Of ColourKeys is empty");
            return -1;
        }
        
        for (int i = 0; i < keys.Count; i++)
        {
            if (newKey.Time < keys[i].Time) 
            {
                keys.Insert(i, newKey);
                return i;
            }
        }
        
        keys.Add(newKey);
        return keys.Count - 1;
    }
    
    // We always want at least one key in our gradient
    public void RemoveKey(int index)
    {
        if (keys == null || index >= keys.Count)
        {
            Debug.LogError("Either keys is null or the index does not exist in the list");
            return;
        }

        if (keys.Count < 2)
        {
            Debug.LogWarning("Not enough keys to allow remove functionality");
            return;
        }
        
        keys.RemoveAt(index);
    }
    
    public int UpdateKeyTime(int index, float time)
    {
        //Remove key at index and place it again in the list to keep it sorted by time

        if (keys == null || index >= keys.Count)
        {
            Debug.LogError("Either keys is null or the index does not exist in the list");
            return -1;
        }

        Color oldColor = keys[index].Colour;
        RemoveKey(index);

        return AddKey(oldColor, time);
    }

    public void UpdateKeyColour(int index, Color colour)
    {
        if (keys == null || index >= keys.Count)
        {
            Debug.LogError("Either keys is null or the index does not exist in the list");
            return;
        }

        ColourKey oldKey = keys[index];
        
        keys[index] = new ColourKey(colour, oldKey.Time);
    }


    public int NumberOfKeys()
    {
        if (keys == null)
        {
            return -1;
        }

        return keys.Count;
    }

    public ColourKey GetKey(int i)
    {
        if (keys == null || i >= keys.Count)
        {
            return default;
        }

        return keys[i];
    }

    public Texture2D GetTexture(int width)
    {
        if (width == 1)
        {
            return null;
        }

        Texture2D texture = new Texture2D(width, 1);
        Color[] colours = new Color[width];

        for (int i = 0; i < width; i++)
        {
            colours[i] = Evaluate((float) i / (width - 1));
        }
        texture.SetPixels(colours);
        texture.Apply();

        return texture;
    }
}
