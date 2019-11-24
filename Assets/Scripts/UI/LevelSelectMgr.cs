using UnityEngine;
using UnityEngine.SceneManagement;
using Menu;

public class LevelSelectMgr : MonoBehaviour
{
    static public LevelSelectMgr ins;
    private const string LEVELSCENE = "Level";

    [SerializeField]
    private Selectable selected;
    [SerializeField]
    private Vector3 boundMin, boundMax;
    private Vector3 targetPos { get {
        return new Vector3(Mathf.Clamp(selected.transform.position.x, boundMin.x, boundMax.x), Mathf.Clamp(selected.transform.position.y, boundMin.y, boundMax.y), boundMin.z);
    } }
    // private float TargetX { get { return selected.transform.position.x > boundMin.x? boundMax.x: (selected.transform.position.x < leftBoundX? leftBoundX: selected.transform.position.x); } }
    [SerializeField]
    private float smoothTime;
    private Vector3 moveSpeed;

    private void Awake() {
        ins = this;
        transform.position = targetPos;
    }

    private void Start() {
        selected.Select = true;
    }

    public void Update()
    {
        if ((transform.position - targetPos).sqrMagnitude >= 0.01f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref moveSpeed, smoothTime);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) selected.Up(ref selected);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) selected.Down(ref selected);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) selected.Left(ref selected);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) selected.Right(ref selected);
        else if (Input.GetButtonDown("Submit")) selected.Submit();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((boundMin + boundMax) / 2, boundMax - boundMin);
        // Gizmos.DrawLine(boun);
        // Gizmos.DrawLine(new Vector3(boundMin.y, -10, 1), new Vector3(boundMin.y, 10, 1));
    }

    static public void LoadLevel(int levelId) {
        SceneManager.LoadScene(LEVELSCENE + levelId);
    }

    static public bool LevelHasUnlock(int levelId) {
        if (levelId < 0) return true;
        return false;
    }
}
