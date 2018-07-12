using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Referencing https://answers.unity.com/questions/393992/custom-inspector-multi-select-enum-dropdown.html

public static class bitmask_extension {

    public static int DrawBitMaskField(Rect field_position, int cur_mask, System.Type target_type, GUIContent content_label)
    {
        var item_names = System.Enum.GetNames(target_type);
        var item_values = System.Enum.GetValues(target_type) as int[];

        int val = cur_mask;
        int maskVal = 0;
        //Bitwise operation to work with enum values
        //Treating this as a black box
        for (int i = 0; i < item_values.Length; i++)
        {
            if (item_values[i] != 0)
            {
                if ((val & item_values[i]) == item_values[i])
                    maskVal |= 1 << i;
            }
            else if (val == 0)
                maskVal |= 1 << i;
        }

        int newMaskVal = EditorGUI.MaskField(field_position, content_label, maskVal, item_names);
        int changes = maskVal ^ newMaskVal;

        for (int i = 0; i < item_values.Length; i++)
        {
            if ((changes & (1 << i)) != 0)            // has this list item changed?
            {
                if ((newMaskVal & (1 << i)) != 0)     // has it been set?
                {
                    if (item_values[i] == 0)           // special case: if "0" is set, just set the val to 0
                    {
                        val = 0;
                        break;
                    }
                    else
                        val |= item_values[i];
                }
                else                                  // it has been reset
                {
                    val &= ~item_values[i];
                }
            }
        }
        return val;
    }
}

public class enum_bitmask_property_drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        var typeAttr = attribute as bitmask_attribute;
        // Add the actual int value behind the field name
        label.text = label.text + "(" + prop.intValue + ")";
        prop.intValue = bitmask_extension.DrawBitMaskField(position, prop.intValue, typeAttr.prop_type, label);
    }
}
