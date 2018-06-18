using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Look")]
public class Look_Decision : Decision {

    public override bool Decide(StateController controller)
    {
        bool target_visible = look(controller);
        return target_visible;
    }

    private bool look(StateController controller)
    {
        return util_ref.d_manager.f_manager.is_visible[(int)controller.gameObject.transform.position.x, (int)controller.gameObject.transform.position.y];
    }
}
