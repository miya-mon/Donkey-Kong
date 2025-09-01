// Assets/Scripts/StageGoal.cs
using UnityEngine;

public class StageGoal : MonoBehaviour
{
    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log($"[StageGoal] TriggerEnter by {col.gameObject.name} (Tag={col.tag})");
        if (triggered) return;
        if (col.CompareTag("player"))
        {
            Debug.Log("[StageGoal] Player reached goal!");
            triggered = true;
            GameController.Instance.StageClear();
        }
    }
}
