using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

[CustomEditor(typeof(SpriteSheetInfo))]
public class SpriteSheetInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpriteSheetInfo spriteSheetInfo = (SpriteSheetInfo)target;

        if (GUILayout.Button("Load Sprite Info"))
        {
            string path = AssetDatabase.GetAssetPath(spriteSheetInfo.spriteSheet);
            string metaPath = path + ".meta";
            if (File.Exists(metaPath))
            {
                string metaContent = File.ReadAllText(metaPath);
                string guid = AssetDatabase.AssetPathToGUID(path);
                spriteSheetInfo.guid = guid;
                spriteSheetInfo.fileIDs.Clear();
                spriteSheetInfo.sprites.Clear();

                var lines = metaContent.Split('\n');
                bool isSpriteSheet = false;
                long currentFileID = 0;
                foreach (var line in lines)
                {
                    
                    if (line.Trim().StartsWith("first:"))
                    {
                        isSpriteSheet = true;
                        string fileID = line.Split(':')[1].Trim();
                        currentFileID = long.Parse(fileID);
                    }
                    if (isSpriteSheet && line.Trim().StartsWith("second:"))
                    {
                        string spriteName = line.Split(':')[1].Trim();
                        spriteSheetInfo.fileIDs.Add(currentFileID);
                        spriteSheetInfo.sprites.Add(spriteName);
                    }
                }

                if (!isSpriteSheet)
                {
                    // Treat as a single sprite asset
                    spriteSheetInfo.fileIDs.Add(21300000); // Common fileID for single sprite assets
                    spriteSheetInfo.sprites.Add(Path.GetFileNameWithoutExtension(path));
                    Debug.Log(Path.GetFileNameWithoutExtension(path));
                    Debug.Log("test");
                    DisplayURL(spriteSheetInfo);
                }

                EditorUtility.SetDirty(spriteSheetInfo);
            }
        }

        if (!string.IsNullOrEmpty(spriteSheetInfo.guid))
        {
            EditorGUILayout.LabelField("GUID", spriteSheetInfo.guid);

            DisplayURLSheet(spriteSheetInfo);
        }
    }

    public void DisplayURLSheet(SpriteSheetInfo spriteSheetInfo)
    {
        for (int i = 0; i < spriteSheetInfo.sprites.Count; i++)
        {
            EditorGUILayout.LabelField($"Sprite: {spriteSheetInfo.sprites[i]}");
            string uxmlUrl = $"&apos;project://database/{AssetDatabase.GetAssetPath(spriteSheetInfo.spriteSheet)}?fileID={spriteSheetInfo.fileIDs[i]}&amp;guid={spriteSheetInfo.guid}&amp;type=3#{spriteSheetInfo.sprites[i]}&apos;";
            EditorGUILayout.TextField("UXML URL", uxmlUrl);
        }
    }
    public void DisplayURL(SpriteSheetInfo spriteSheetInfo)
    {
                EditorGUILayout.LabelField($"Sprite: {spriteSheetInfo.sprites}");
                string uxmlUrl = $"&apos;project://database/{AssetDatabase.GetAssetPath(spriteSheetInfo.spriteSheet)}?fileID=21300000&amp;guid={spriteSheetInfo.guid}&amp;type=3;&apos;";
                EditorGUILayout.TextField("UXML URL", uxmlUrl);
    }
}