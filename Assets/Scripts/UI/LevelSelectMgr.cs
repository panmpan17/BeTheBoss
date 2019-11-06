using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectMgr : MonoBehaviour
{
    static public LevelSelectMgr ins;
    private const string LEVELSCENE = "Level";

    [SerializeField]
    private SelectableItem selected;
    [SerializeField]
    private float leftBoundX, rightBoundX;
    private float TargetX { get { return selected.transform.position.x > rightBoundX? rightBoundX: (selected.transform.position.x < leftBoundX? leftBoundX: selected.transform.position.x); } }
    [SerializeField]
    private float smoothTime;
    private float moveSpeed;

    private void Awake() {
        ins = this;
    }

    private void Start() {
        selected.Select();
    }

    public void Update()
    {
        if (Mathf.Abs(transform.position.x - TargetX) >= 0.03f)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.SmoothDamp(pos.x, TargetX, ref moveSpeed, smoothTime);
            transform.position = pos;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selected.NavTop == null) return;
            if (selected.NavTop.Disabled) return;
            selected.Selected = false;
            selected = selected.NavTop;
            selected.Select();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selected.NavBottom == null) return;
            if (selected.NavBottom.Disabled) return;
            selected.Selected = false;
            selected = selected.NavBottom;
            selected.Select();
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selected.NavLeft == null) return;
            if (selected.NavLeft.Disabled) return;
            selected.Selected = false;
            selected = selected.NavLeft;
            selected.Select();
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selected.NavRight == null) return;
            if (selected.NavRight.Disabled) return;
            selected.Selected = false;
            selected = selected.NavRight;
            selected.Select();
        }
        else if (Input.GetButtonDown("Submit")) selected.Activate();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftBoundX, -10, 1), new Vector3(leftBoundX, 10, 1));
        Gizmos.DrawLine(new Vector3(rightBoundX, -10, 1), new Vector3(rightBoundX, 10, 1));
    }

    static public void LoadLevel(int levelId) {
        SceneManager.LoadScene(LEVELSCENE + levelId);
    }

    static public bool LevelHasUnlock(int levelId) {
        if (levelId < 0) return true;
        return false;
    }
}
