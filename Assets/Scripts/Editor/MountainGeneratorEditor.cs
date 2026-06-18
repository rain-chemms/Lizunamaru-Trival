using UnityEditor;
using UnityEngine;

//用于山峰生成器的编辑器控制
[CustomEditor(typeof(MountainGenerator))]
public class MountainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        EditorGUILayout.LabelField("⛰️ 山峰生成控制", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        MountainGenerator gen = (MountainGenerator)target;

        // 随机种子 + 生成 一行显示
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("🎲 新种子", GUILayout.Width(80)))
        {
            Undo.RecordObject(gen, "New Mountain Seed");
            gen.seed = Random.Range(int.MinValue, int.MaxValue);
            EditorUtility.SetDirty(gen);
        }

        EditorGUI.BeginChangeCheck();
        int newSeed = EditorGUILayout.IntField("Seed", gen.seed);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(gen, "Change Mountain Seed");
            gen.seed = newSeed;
            EditorUtility.SetDirty(gen);
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(4);

        // 大按钮触发生成
        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.4f);
        if (GUILayout.Button("🏔️ 生成山峰", GUILayout.Height(36)))
        {
            Undo.RecordObject(gen.GetComponent<Terrain>().terrainData, "Generate Mountain");
            gen.Generate();
            SceneView.RepaintAll();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(4);
        
        // 快捷操作
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("平坦重置"))
        {
            var td = gen.GetComponent<Terrain>().terrainData;
            Undo.RecordObject(td, "Flatten Terrain");
            int res = td.heightmapResolution;
            td.SetHeights(0, 0, new float[res, res]);
            SceneView.RepaintAll();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
}