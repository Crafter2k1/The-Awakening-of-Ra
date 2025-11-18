using System;
using System.Collections;
using UnityEngine;

namespace Core.GamePlay
{
    /// <summary>
    /// Відповідає за рух променя по послідовності точок під час ФАЗИ ПОКАЗУ.
    /// У фазі вводу гравця промінь не використовується.
    /// </summary>
    public class SunBeamController : MonoBehaviour
    {
        [Tooltip("Наскільки близько до точки вважати, що променя вже \"дійшов\".")]
        [SerializeField] private float reachDistance = 0.05f;

        Coroutine _routine;

        /// <summary>
        /// Запускає показ послідовності:
        /// рухається по points, на кожній точці робить паузу stopDuration.
        /// При кожному кроці викликає onStep(index), а після завершення — onFinished().
        /// </summary>
        public void PlaySequence(
            Transform[] points,
            float moveSpeed,
            float stopDuration,
            Action<int> onStep,
            Action onFinished)
        {
            if (points == null || points.Length == 0)
            {
                Debug.LogError("SunBeamController.PlaySequence: no points!");
                return;
            }

            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(PlayRoutine(points, moveSpeed, stopDuration, onStep, onFinished));
        }

        IEnumerator PlayRoutine(
            Transform[] points,
            float moveSpeed,
            float stopDuration,
            Action<int> onStep,
            Action onFinished)
        {
            // стартуємо з позиції першої точки
            transform.position = points[0].position;

            for (int i = 0; i < points.Length; i++)
            {
                Transform target = points[i];

                // рух до точки
                while (Vector3.Distance(transform.position, target.position) > reachDistance)
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        target.position,
                        moveSpeed * Time.deltaTime
                    );
                    yield return null;
                }

                // прийшли до i-го елемента послідовності
                onStep?.Invoke(i);

                // пауза над символом
                yield return new WaitForSeconds(stopDuration);
            }

            onFinished?.Invoke();
            _routine = null;
        }
    }
}
