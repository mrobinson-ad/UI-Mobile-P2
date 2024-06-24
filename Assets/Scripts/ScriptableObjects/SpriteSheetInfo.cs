using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteSheetInfo", menuName = "Custom/SpriteSheetInfo")]
public class SpriteSheetInfo : ScriptableObject
{
    public Texture2D spriteSheet;
    public string guid;
    public List<long> fileIDs = new List<long>();
    public List<string> sprites = new List<string>();
}