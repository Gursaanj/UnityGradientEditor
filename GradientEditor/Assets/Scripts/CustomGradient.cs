using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomGradient
{
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
    
    public Color Evaluate(float time)
    {
        if (keys == null || keys.Count == 0)
        {
            return Color.white;
        }

        ColourKey keyLeft = keys[0];
        ColourKey keyRight = keys[keys.Count - 1];

        for (int i = 0; i < keys.Count-1; i++)
        {
            ColourKey currentKey = keys[i];
            ColourKey nextKey = keys[i + 1];
            
            //if the designated time is in between two consecutive keys Create new KeysLeft and KeysRight 
            if (currentKey.Time <= time && nextKey.Time >= time)
            {
                keyLeft = currentKey;
                keyRight = nextKey;
                break;
            }
        }
        
        //return blend between ketLeft and keyRight
        
        // Will convert time to a float (0,1) in reference to keyLeft(0) and keyRight(1)
        float blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
        return Color.Lerp(keyLeft.Colour, keyRight.Colour, blendTime);
    }

    public void AddKey(Color colour, float time)
    {
        ColourKey newKey = new ColourKey(colour, time);

        if (keys == null)
        {
            Debug.LogError("The list Of ColourKeys is empty");
            return;
        }
        
        for (int i = 0; i < keys.Count; i++)
        {
            if (newKey.Time < keys[i].Time) 
            {
                keys.Insert(i, newKey);
                return;
            }
        }
        
        keys.Add(newKey);
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
