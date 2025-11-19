using System;
using System.Collections;
using UnityEngine;

namespace Core.GamePlay
{
    public class SunBeamController : MonoBehaviour
    {
        [Header("Beam Origin (стартова точка)")]
        [SerializeField] private Transform beamOrigin;

        [Header("Beam Prefab (movingPoint + TrailRenderer)")]
        [SerializeField] private GameObject beamPrefab;

        [Header("Settings")]
        [SerializeField] private float reachDistance = 0.05f;

        private GameObject _currentBeam;

        private Coroutine _routine;

        public void PlaySequence(
            Transform[] points,
            float moveSpeed,
            float stopDuration,
            Action<int> onStep,
            Action onFinished)
        {
            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(ShowRoutine(points, moveSpeed, stopDuration, onStep, onFinished));
        }

        private IEnumerator ShowRoutine(
            Transform[] points,
            float moveSpeed,
            float stopDuration,
            Action<int> onStep,
            Action onFinished)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 target = points[i].position;

                // Створюємо новий промінь
                SpawnBeam();

                // Летимо
                yield return MoveBeamTo(target, moveSpeed);

                // повідомляємо GameManager про крок
                onStep?.Invoke(i);

                // чекаємо, поки символ підсвічений
                yield return new WaitForSeconds(stopDuration);

                // Видаляємо промінь
                ClearBeam();
            }

            ClearBeam();

            onFinished?.Invoke();
        }

        /// <summary>
        /// Створення нового променя
        /// </summary>
        private void SpawnBeam()
        {
            // Видаляємо попередній, якщо він був
            ClearBeam();

            _currentBeam = Instantiate(beamPrefab, beamOrigin.position, Quaternion.identity);
        }

        /// <summary>
        /// Рух beamPrefab → target
        /// </summary>
        private IEnumerator MoveBeamTo(Vector3 target, float speed)
        {
            Transform mover = _currentBeam.transform;

            // скидаємо трейл, якщо є
            var trail = mover.GetComponent<TrailRenderer>();
            if (trail) trail.Clear();

            while (Vector3.Distance(mover.position, target) > reachDistance)
            {
                mover.position = Vector3.MoveTowards(
                    mover.position,
                    target,
                    speed * Time.deltaTime
                );

                yield return null;
            }

            mover.position = target;
        }

        /// <summary>
        /// Видаляє поточний промінь
        /// </summary>
        private void ClearBeam()
        {
            if (_currentBeam != null)
            {
                Destroy(_currentBeam);
                _currentBeam = null;
            }
        }
    }
}
