using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Events;

/*
    public attack_shape shape;
    public Stat_Interface stats;
    public List<attack_component> components;
    public List<Vector2Int> target_squares;
 */

/*
 * Custom editor for attack entities
 */ 
[CustomEditor(typeof(attack_entity))]
public class attack_editor : Editor
{
    attack_entity attack_ref;
    bool component_foldout;

    public override void OnInspectorGUI()
    {
        attack_ref = target as attack_entity;

        //Set the icon of the attack
        attack_ref.icon = (Sprite)EditorGUILayout.ObjectField("Icon:", attack_ref.icon, typeof(Sprite), false);

        

        //Set the shape, and the range / radius as applicable
        if (attack_ref.shape == attack_shape.single || attack_ref.shape == attack_shape.line)
        {
            EditorGUILayout.BeginHorizontal();
            attack_ref.shape = (attack_shape)EditorGUILayout.EnumPopup("Attack Shape:", attack_ref.shape);
            attack_ref.stats.set_stat("Range",
                EditorGUILayout.IntField("Range", attack_ref.stats.get_stat_value("Range"))
            );
            EditorGUILayout.EndHorizontal();
        }
        else if(attack_ref.shape == attack_shape.square)
        {
            attack_ref.shape = (attack_shape)EditorGUILayout.EnumPopup("Attack Shape:", attack_ref.shape);
            int range = attack_ref.stats.get_stat_value("Range");
            range = EditorGUILayout.IntField("Range", range);
            attack_ref.stats.set_stat("Range", range);

            int radius = attack_ref.stats.get_stat_value("Radius");
            radius = EditorGUILayout.IntField("Radius", radius);
            attack_ref.stats.set_stat("Radius", radius);
        }

        //Edit the components
        component_foldout = EditorGUILayout.Foldout(component_foldout, "Components");
        if (component_foldout && attack_ref.components != null)
        {
            for(int i = 0; i < attack_ref.components.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(attack_ref.components[i].component_name);
                attack_ref.components[i].editor_layout(attack_ref);
                EditorUtility.SetDirty(attack_ref.components[i]);
                if (i != 0)
                {
                    if (GUILayout.Button("▲"))
                    {
                        //Move up in list
                        attack_component tmp = attack_ref.components[i];
                        attack_ref.components[i] = attack_ref.components[i - 1];
                        attack_ref.components[i - 1] = tmp;
                        break;
                    }
                }
                if (i != attack_ref.components.Length - 1)
                {
                    if (GUILayout.Button("▼"))
                    {
                        //Move down in list
                        attack_component tmp = attack_ref.components[i];
                        attack_ref.components[i] = attack_ref.components[i + 1];
                        attack_ref.components[i + 1] = tmp;
                        break;
                    }
                }
                //Delete the component
                if (GUILayout.Button("-"))
                {
                    //Get all assets attached to the object
                    Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(attack_ref));
                    for(int n = 0; n < assets.Length; n++)
                    {
                        //Do a type comparison and destroy if it matches
                        //This will break if there's more than one component of the same type,
                        //but there shouldn't be any purpose for that
                        if(assets[n].GetType() == attack_ref.components[i].GetType())
                        {
                            //If need be, we could compare IDs
                            Object.DestroyImmediate(assets[n], true);
                        }
                    }
                    attack_ref.remove_component(i);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        //Since each menu item has a custom string and custom callback,
        //we don't really save much space by creating a function to add the two parts
        //that stay the same throughout
        if (GUILayout.Button("Add attack component"))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Direct damage"), false, add_component, new direct_damage_component());
            menu.AddItem(new GUIContent("Push"), false, add_component, new push_component());
            menu.AddItem(new GUIContent("Effect on enemy"), false, add_component, new effects_on_enemy_component());
            menu.AddItem(new GUIContent("Effect on attacker"), false, add_component, new effect_on_attacker_component());
            menu.AddItem(new GUIContent("Dash"), false, add_component, new dash_component());
            menu.AddItem(new GUIContent("Create projectile"), false, add_component, new create_projectile_component());
            menu.AddItem(new GUIContent("Limit targets"), false, add_component, new line_limit_targets_component());
            menu.AddItem(new GUIContent("Switch skill"), false, add_component, new switch_skill_component());
            menu.AddItem(new GUIContent("Force bool"), false, add_component, new force_bool_component());
            menu.AddItem(new GUIContent("Temporary condition on target"), false, add_component, new applies_condition_target_component());
            menu.AddItem(new GUIContent("Temporary condition on attacker"), false, add_component, new applies_condition_attacker_component());
            menu.AddItem(new GUIContent("Cooldown"), false, add_component, new cooldown_component());
            menu.AddItem(new GUIContent("Add charge"), false, add_component, new adds_charge_component());
            menu.AddItem(new GUIContent("Consumes charge"), false, add_component, new consumes_charge_component());
            menu.AddItem(new GUIContent("Add charge on event"), false, add_component, new add_charge_on_event_component());
            menu.ShowAsContext();
        }


        if (GUILayout.Button("Check stats"))
        {
            attack_ref.stats.print_values();
        }
        if (GUILayout.Button("Print components"))
        {
            for (int i = 0; i < attack_ref.components.Length; i++)
            {
                Debug.Log("Component " + i + ": " + attack_ref.components[i]);
            }
        }

        EditorUtility.SetDirty(attack_ref);
    }

    //Get the filepath of the current object selected in the editor
    private string get_path()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }
        return path;
    }
    
    //Add a component to the selected attack entity object
    //Keeps the file explorer clean, but we can't delete things without
    //wiping the entire entity
    private void add_object(Object o)
    {
        AssetDatabase.AddObjectToAsset(o, attack_ref);
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(o);
    }

    //Create a new component
    private void add_component(object component)
    {
        attack_component comp = (attack_component)ScriptableObject.CreateInstance(component.GetType());
        comp.add(attack_ref);
        comp.name = comp.component_name;
        add_object(comp);

    }
}
