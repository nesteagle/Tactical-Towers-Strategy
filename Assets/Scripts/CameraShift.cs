using UnityEngine;

public class CameraShift : MonoBehaviour
{
    private Vector2 _mouseClickPos;
    private Vector2 _mouseCurrentPos;
    private bool _panning = false;
    Camera CameraMain;
    private readonly float _xlimit = 10f;
    private readonly float _ylimit = 8f;
    public RectTransform[] CanvasTransforms;
    //public RectTransform OptionsCanvasTransform;
    //public RectTransform BuildCanvasTransform;
    //public RectTransform TowerConfirmButtons;
    private void Awake()
    {
        CameraMain = Camera.main;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && !_panning)
        {
            _mouseClickPos = CameraMain.ScreenToWorldPoint(Input.mousePosition);
            _panning = true;
        }
        if (_panning)
        {
            _mouseCurrentPos = CameraMain.ScreenToWorldPoint(Input.mousePosition);
            var distance = _mouseCurrentPos - _mouseClickPos;
            transform.position += new Vector3(-distance.x, -distance.y);
            if (transform.position.x > _xlimit / 2)
            {
                transform.position = new Vector3(_xlimit / 2, transform.position.y, -10);
            }
            else if (transform.position.x < -_xlimit / 2)
            {
                transform.position = new Vector3(-_xlimit / 2, transform.position.y, -10);
            }
            if (transform.position.y > _ylimit)
            {
                transform.position = new Vector3(transform.position.x, _ylimit, -10);
            }
            else if (transform.position.y < -_ylimit)
            {
                transform.position = new Vector3(transform.position.x, -_ylimit, -10);
            }
            if (transform.position.x < _xlimit / 2 && transform.position.x > -_ylimit / 2)
            {
                UpdateTransforms(CanvasTransforms, true, distance);
            }
            if (transform.position.y < _xlimit && transform.position.y > -_ylimit)
            {
                UpdateTransforms(CanvasTransforms, false, distance);
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
            _panning = false;
    }
    private void UpdateTransforms(RectTransform[] transforms, bool updatingXPosition, Vector3 distance)
    {
        if (updatingXPosition)
        {
            foreach (RectTransform transform in transforms)
            {
                transform.position += new Vector3(distance.x, 0);
            }
        }
        else foreach (RectTransform transform in transforms)
            {
                transform.position += new Vector3(0, distance.y);
            }
    }
}
