using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/*
    public rpg_character target_character;
    public int turns_remaining, max_turns;

    public string on_start_s, on_turn_s, on_remove_s;
    public Modifier on_start_m, on_turn_m, on_remove_m;
    public effect_length on_start_l, on_turn_l;
 */

[CustomEditor(typeof(temporary_condition))]
public class temporary_condition_editor : Editor {

    bool start, turn, end;

    public override void OnInspectorGUI()
    {
        temporary_condition condition_ref = target as temporary_condition;

        condition_ref.max_turns = EditorGUILayout.IntField("Duration:", condition_ref.max_turns);

        start = EditorGUILayout.Foldout(start, "On start effect");
        if(start)
        {
            condition_ref.on_start_s = EditorGUILayout.TextField("Target stat", condition_ref.on_start_s);
            EditorGUILayout.BeginHorizontal();
            condition_ref.on_start_m.op = (operation)EditorGUILayout.EnumPopup(condition_ref.on_start_m.op);
            condition_ref.on_start_m.argument = EditorGUILayout.IntField(condition_ref.on_start_m.argument);
            EditorGUILayout.EndHorizontal();
            condition_ref.on_start_l = (effect_length)EditorGUILayout.EnumPopup(condition_ref.on_start_l);
            condition_ref.on_start_e = (GameObject)EditorGUILayout.ObjectField(
                condition_ref.on_start_e,
                typeof(GameObject),
                false);
        }

        turn = EditorGUILayout.Foldout(turn, "Each turn effect");
        if(turn)
        {
            condition_ref.on_turn_s = EditorGUILayout.TextField("Target stat", condition_ref.on_turn_s);
            EditorGUILayout.BeginHorizontal();
            condition_ref.on_turn_m.op = (operation)EditorGUILayout.EnumPopup(condition_ref.on_turn_m.op);
            condition_ref.on_turn_m.argument = EditorGUILayout.IntField(condition_ref.on_turn_m.argument);
            EditorGUILayout.EndHorizontal();
            condition_ref.on_turn_l = (effect_length)EditorGUILayout.EnumPopup(condition_ref.on_turn_l);
            condition_ref.on_turn_e = (GameObject)EditorGUILayout.ObjectField(
                condition_ref.on_turn_e,
                typeof(GameObject),
                false);
        }

        end = EditorGUILayout.Foldout(end, "On end effect");
        if(end)
        {
            condition_ref.on_remove_s = EditorGUILayout.TextField("Target stat", condition_ref.on_remove_s);
            EditorGUILayout.BeginHorizontal();
            condition_ref.on_remove_m.op = (operation)EditorGUILayout.EnumPopup(condition_ref.on_remove_m.op);
            condition_ref.on_remove_m.argument = EditorGUILayout.IntField(condition_ref.on_remove_m.argument);
            EditorGUILayout.EndHorizontal();
            condition_ref.on_remove_e = (GameObject)EditorGUILayout.ObjectField(
                condition_ref.on_remove_e,
                typeof(GameObject),
                false);
        }

        EditorUtility.SetDirty(condition_ref);
    }
}
