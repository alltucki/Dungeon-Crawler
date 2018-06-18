using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

/*
    public trigger_enum trigger;        //Trigger to check the condition
    public string stat_comp;            //The stat we'll be checking to see if the perk applies
    public base_value comp_type;        //Whether the value should be dependant on the relative value to another entity
    public string other_comp;           //The stat to reference on the other involved entity
    public comparison op;               //Operator; greater than, less than, equal
    public int base_comp;               //Base value that the condition is compared to
    public duration_enum duration;      //How long the perk will apply, if condition is true
    public string stat;                 //The stat that will be modified
    public Modifier mod;                //The operation applied to the stat
 */

[CustomEditor(typeof(Perk))]
public class PerkEditor : Editor {
    bool show_mod;

    public override void OnInspectorGUI()
    {
        Perk perk_ref = target as Perk;
        perk_ref.icon = (Sprite)EditorGUILayout.ObjectField("Icon:", perk_ref.icon, typeof(Sprite), false);
        perk_ref.sfx = (AudioClip)EditorGUILayout.ObjectField("Sound Effect:", perk_ref.sfx, typeof(AudioClip), false);
        perk_ref.trigger = (trigger_enum)EditorGUILayout.EnumPopup("Trigger", perk_ref.trigger);
        perk_ref.description = EditorGUILayout.TextArea(perk_ref.description, GUILayout.Height(50f));

        switch (perk_ref.trigger)
        {
            case trigger_enum.always:
                break;
            case trigger_enum.on_attack:
                perk_ref.comp_type = (base_value)EditorGUILayout.EnumPopup("Comparison Type", perk_ref.comp_type);
                comp_type_GUI(perk_ref);
                break;
            case trigger_enum.on_attacked:
                perk_ref.comp_type = (base_value)EditorGUILayout.EnumPopup("Comparison Type", perk_ref.comp_type);
                comp_type_GUI(perk_ref);
                break;
            case trigger_enum.turn_start:
                perk_ref.comp_type = (base_value)EditorGUILayout.EnumPopup("Comparison Type", perk_ref.comp_type);
                comp_type_GUI_no_relative(perk_ref);
                break;
            default:
                Debug.LogError("Perk Editor ran into default case for trigger switch");
                break;
        }

        show_mod = EditorGUILayout.Foldout(show_mod, "Modifier");
        if (show_mod)
        {
            perk_ref.stat = EditorGUILayout.TextField("Stat", perk_ref.stat);
            perk_ref.mod.op = (operation)EditorGUILayout.EnumPopup("Operation", perk_ref.mod.op);
            perk_ref.mod.argument = EditorGUILayout.IntField("Argument", perk_ref.mod.argument);
        }

        if (perk_ref.trigger != trigger_enum.always)
        {
            perk_ref.duration = (duration_enum)EditorGUILayout.EnumPopup("Duration", perk_ref.duration);
            if(perk_ref.duration == duration_enum.turn_limited)
            {
                perk_ref.turn_duration = EditorGUILayout.IntField("Turns: ", perk_ref.turn_duration);
            }
        }
        else
        {
            perk_ref.duration = duration_enum.permanent;
        }
        EditorUtility.SetDirty(perk_ref);
    }

    private void comp_type_GUI(Perk perk_ref)
    {
        switch(perk_ref.comp_type)
        {
            case base_value.set:
                perk_ref.stat_comp = EditorGUILayout.TextField("Stat Reference", perk_ref.stat_comp);
                perk_ref.op = (comparison)EditorGUILayout.EnumPopup(perk_ref.op);
                perk_ref.base_comp = EditorGUILayout.IntField("Comparison Value", perk_ref.base_comp);
                break;
            case base_value.relative:
                perk_ref.stat_comp = EditorGUILayout.TextField("Stat Reference", perk_ref.stat_comp);
                EditorGUILayout.PrefixLabel("Minus");
                perk_ref.other_comp = EditorGUILayout.TextField("Other Entity Stat Reference", perk_ref.other_comp);
                perk_ref.op = (comparison)EditorGUILayout.EnumPopup(perk_ref.op);
                perk_ref.base_comp = EditorGUILayout.IntField("Comparison Value", perk_ref.base_comp);
                break;
            case base_value.random:
                perk_ref.base_comp = EditorGUILayout.IntField("Percent Chance", perk_ref.base_comp);
                perk_ref.op = comparison.less_than_or_equal;
                break;
            default:
                break;
        }
    }

    private void comp_type_GUI_no_relative(Perk perk_ref)
    {
        switch (perk_ref.comp_type)
        {
            case base_value.set:
                perk_ref.stat_comp = EditorGUILayout.TextField("Stat Reference", perk_ref.stat_comp);
                perk_ref.op = (comparison)EditorGUILayout.EnumPopup(perk_ref.op);
                perk_ref.base_comp = EditorGUILayout.IntField("Comparison Value", perk_ref.base_comp);
                break;
            case base_value.random:
                perk_ref.base_comp = EditorGUILayout.IntField("Percent Chance", perk_ref.base_comp);
                break;
            default:
                break;
        }
    }
}
