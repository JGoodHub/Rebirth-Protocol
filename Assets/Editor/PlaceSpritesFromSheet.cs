using UnityEngine;
using UnityEditor;

public class PlaceSpritesFromSheet : EditorWindow
{
    private Texture2D spriteSheet;
    private Vector2 pivot = Vector2.zero;
    private float pixelsPerUnit = 100f;
    private float spacing = 0.01f;

    [MenuItem("Tools/Place Sprites from Sheet")]
    private static void Init()
    {
        GetWindow<PlaceSpritesFromSheet>("Sprite Sheet Placer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Sheet Settings", EditorStyles.boldLabel);

        spriteSheet = (Texture2D) EditorGUILayout.ObjectField("Sprite Sheet", spriteSheet, typeof(Texture2D), false);
        pivot = EditorGUILayout.Vector2Field("Pivot", pivot);
        pixelsPerUnit = EditorGUILayout.FloatField("Pixels Per Unit", pixelsPerUnit);
        spacing = EditorGUILayout.FloatField("Spacing in Units", spacing);

        if (GUILayout.Button("Place Sprites"))
        {
            if (spriteSheet == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a sprite sheet.", "OK");
                return;
            }

            PlaceSprites();
        }
    }

    private void PlaceSprites()
    {
        string path = AssetDatabase.GetAssetPath(spriteSheet);
        Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

        GameObject root = new GameObject("SpriteSheetTiles");

        foreach (Object asset in assets)
        {
            if (asset is not Sprite sprite)
                continue;

            Rect rect = sprite.rect;
            Vector2 localPos = new Vector2(
                rect.x + rect.width * pivot.x,
                rect.y + rect.height * pivot.y
            ) / pixelsPerUnit;

            GameObject go = new GameObject(sprite.name);
            go.transform.parent = root.transform;
            go.transform.position = new Vector3(localPos.x + spacing, localPos.y + spacing, 0);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
        }
    }
}