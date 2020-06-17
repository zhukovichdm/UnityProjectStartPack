using Scripts.Behaviours.Other;
using Scripts.Component.Actions;
using UnityEngine;

public class ControlOnClick : MonoBehaviour
{
    private (OnClick onClick, int countCameraPivots, bool needReset) _currentTuple;
    private int _id;

    private void Awake()
    {
        GameActions.TempAction.Subscribe(TempCtion_Subscriber);
    }

    private void TempCtion_Subscriber((OnClick onClick, int countCameraPivots, bool needReset) tuple)
    {
        if (tuple.onClick == null || tuple.needReset == false) return;
        _currentTuple = tuple;
        _id = 0;
    }

    public void GoNext()
    {
        if (_currentTuple == default) return;
        if (_id + 1 >= _currentTuple.countCameraPivots)
            _id = 0;
        else
            _id++;

        _currentTuple.onClick.MoveCameraToPivot(_id);
    }

    public void GoBack()
    {
        if (_currentTuple == default) return;
        if (_id - 1 < 0)
            _id = _currentTuple.countCameraPivots - 1;
        else
            _id--;

        _currentTuple.onClick.MoveCameraToPivot(_id);
    }
}