using UnityEngine;

public class LerpEntity : MonoBehaviour
{
    public Entity entity;

    public Vector3 lerpFrom;
    public Vector3 lerpTo;
    private bool _reverseLerp = false;
    private int _elapsedLerpFrames = 0;
    public int lerpMaxFrames = 8;

    private bool _isComplete = false;

    // Update is called once per frame
    void Update()
    {
        if (!_reverseLerp && _elapsedLerpFrames == 0)
        {
            lerpFrom = entity.position.ToVector3Int();

            lerpTo = lerpFrom + (lerpTo - lerpFrom) / 2;
        }

        float interpolationRatio = (float)_elapsedLerpFrames / lerpMaxFrames;

        Vector3 interpolatedPosition = Vector3.Lerp(lerpFrom, lerpTo, interpolationRatio);

        _elapsedLerpFrames++;
        if (_elapsedLerpFrames >= lerpMaxFrames)
        {
            if (_reverseLerp)
            {
                _isComplete = true;
                interpolatedPosition = entity.position.ToVector3Int();
            }
            lerpFrom = lerpTo;
            lerpTo = entity.position.ToVector3Int();
            _reverseLerp = !_reverseLerp;
            _elapsedLerpFrames = 0;
        }
        transform.position = interpolatedPosition;

        if(_isComplete)
        {
            Destroy(this);
        }
    }
}
