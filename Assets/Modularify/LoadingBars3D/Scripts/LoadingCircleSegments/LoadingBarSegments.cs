using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modularify.LoadingBars3D
{
    public class LoadingBarSegments : MonoBehaviour
    {
        #region variables
        [Header("Part Reference")]
        [SerializeField]
        private GameObject _loadingPartsRef;

        [Header("Loading Bar Settings")]
        [SerializeField]
        private PartsDisposition _partsArrangement = PartsDisposition.CircleDivision;

        [Range(1, 10)]
        [SerializeField]
        private int _numberOfParts = 6;

        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _percentage = 0.5f;

        [SerializeField]
        private AnimationCurve _animationCurve = AnimationCurve.EaseInOut(0, 0, 1.0f, 1);

        [Header("Colors")]
        [ColorUsage(true, true)]
        [SerializeField]
        private Color _innerColor = new Color(1, 0, 0, 1);

        [ColorUsage(true, true)]
        [SerializeField]
        private Color _outerColor = new Color(0, 1, 0, 1);


        private List<GameObject> _loadingParts;
        #endregion

        #region methods
        private void Awake()
        {
            _loadingParts = new List<GameObject>();
        }

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
            SetPercentage(_percentage);
        }

        // Update is called once per frame
        void Update()
        {
            SetPercentage(_percentage);
        }

        /// <summary>
        /// Creates a number of loading parts equal to the _numberOfParts attribute equally distributed on a circle or sequential based on the PartsDisposition enum value
        /// </summary>
        public void Initialize()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            _loadingParts = new List<GameObject>();
            float zAngle;
            switch (_partsArrangement)
            {
                case PartsDisposition.CircleDivision:
                    zAngle = 360 / _numberOfParts;
                    break;

                case PartsDisposition.Sequential:
                    zAngle = 36;
                    break;

                default:
                    zAngle = 360 / _numberOfParts;
                    break;
            }
            for (int i = 0; i < _numberOfParts; i++)
            {
                GameObject part = Instantiate(_loadingPartsRef, transform.position, Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, i * zAngle)), transform);
                _loadingParts.Add(part);
                part.GetComponent<LoadingSegment>().Initialize(_innerColor, _outerColor);
            }

            SetPercentage(_percentage);

        }

        /// <summary>
        /// Activates only those parts which are under the given percentage as a parameter and updates their values
        /// </summary>
        /// <param name="newPercentage"> Current percentage of the loading circle </param>
        public void SetPercentage(float newPercentage)
        {
            if (_numberOfParts != transform.childCount)
            {
                Initialize();
            }
            else
            {
                Mathf.Clamp01(newPercentage);
                _percentage = newPercentage;
                for (int i = 0; i < _numberOfParts; i++)
                {
                    if (i < _numberOfParts * _percentage)
                    {
                        _loadingParts[i].SetActive(true);
                        if (i < Mathf.Floor(_numberOfParts * _percentage))
                        {
                            _loadingParts[i].GetComponent<LoadingSegment>().SetPartPercentage(1);
                        }
                        else
                        {
                            _loadingParts[i].GetComponent<LoadingSegment>().SetPartPercentage((_numberOfParts * _percentage) % 1);
                        }
                    }
                    else
                    {
                        _loadingParts[i].GetComponent<LoadingSegment>().SetPartPercentage(0);
                        _loadingParts[i].SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// Method the returns the animation curve in order to animate the segments
        /// </summary>
        /// <returns></returns>
        public AnimationCurve GetAnimationCurve()
        {
            return _animationCurve;
        }
        #endregion

        #region custom enums
        private enum PartsDisposition
        {
            CircleDivision,
            Sequential
        }
        #endregion
        private float decreaseTime = 3f;
        private bool isDecreasing = false;

        // 플레이어가 데미지를 받을 때 호출될 메서드입니다.
        public void TakeDamage()
        {
            if (_percentage < 1f)
            {
                SetPercentage(_percentage + 0.2f);
                if (_percentage >= 1f)
                {
                    if (!isDecreasing)
                    {
                        StartCoroutine(DecreasePercentageOverTime());
                    }
                }
            }
        }

        // 시간이 지남에 따라 로딩 바 퍼센테이지를 감소시키는 코루틴입니다.
        private IEnumerator DecreasePercentageOverTime()
        {
            isDecreasing = true;
            float elapsedTime = 0f;

            while (elapsedTime < decreaseTime)
            {
                SetPercentage(Mathf.Lerp(1f, 0f, elapsedTime / decreaseTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            SetPercentage(0f);
            isDecreasing = false;
        }
    }
}

