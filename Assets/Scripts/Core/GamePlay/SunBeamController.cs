using System.Collections;
using UnityEngine;
using Core.GamePlay.Levels;   // <<< важливо

namespace Core.GamePlay
{
    public class SunBeamController : MonoBehaviour
    {
        float _moveSpeed;
        float _stopDuration;
        float _hitWindow;
        float _reachDistance = 0.05f;

        Transform[] _points;

        public bool IsInStopWindow { get; private set; }
        public int CurrentIndex { get; private set; } = -1;

        Coroutine _routine;

        // викликається з GameManager
        public void Init(Transform[] points, LevelData levelData)   // <<< тут теж
        {
            _points = points;

            if (_points == null || _points.Length == 0)
            {
                Debug.LogError("SunBeamController.Init: no points!");
                return;
            }

            _moveSpeed    = levelData.moveSpeed;
            _stopDuration = levelData.stopDuration;
            _hitWindow    = levelData.hitWindow;

            transform.position = _points[0].position;
            IsInStopWindow = false;
            CurrentIndex = -1;

            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(MoveRoutine());
        }

        IEnumerator MoveRoutine()
        {
            for (int i = 0; i < _points.Length; i++)
            {
                Transform target = _points[i];

                while (Vector3.Distance(transform.position, target.position) > _reachDistance)
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        target.position,
                        _moveSpeed * Time.deltaTime
                    );
                    yield return null;
                }

                CurrentIndex = i;

                IsInStopWindow = true;
                yield return new WaitForSeconds(_hitWindow);

                IsInStopWindow = false;
                float rest = Mathf.Max(0f, _stopDuration - _hitWindow);
                yield return new WaitForSeconds(rest);
            }

            Debug.Log("Beam: sequence finished");
        }
    }
}
