using UnityEngine;

public class ComboPauseBehaviour : StateMachineBehaviour
{
    [Tooltip("Número de golpe al que salta si bufferizas aquí (2 o 3)")]
    public int nextComboNum = 2;

    // Se llama cada frame mientras dura el estado ComboPause
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Si bufferizas un click, saltamos al siguiente combo inmediatamente:
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetInteger("Control", nextComboNum);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Si no bufferizaste, Control sigue en 0 → vuelve a Idle
        animator.SetInteger("Control", animator.GetInteger("Control"));
    }
}
