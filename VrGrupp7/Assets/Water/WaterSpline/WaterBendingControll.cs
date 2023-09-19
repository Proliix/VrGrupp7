using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
public class WaterBendingControll : MonoBehaviour
{
    [SerializeField] float _PointCount;
    [SerializeField] float _Radius;
    [SerializeField] float _HeightDelta;
    [SerializeField] Vector3 _Scale;

    [SerializeField] Spline _Spline;
    [SerializeField] ExampleContortAlong _ContortAlong;

    [SerializeField] float _SpeedDelta;
    [SerializeField] float _AnimSpeed;
    [SerializeField] ParticleSystem _PuddleParticle;
    [SerializeField] ParticleSystem _SplashParticle;
    [SerializeField] float _SplashActivationOffset;

    [SerializeField] float _PuddleScaleSpeed;

    private Vector3 _target;
    public void WaterBend(Vector3 target)
    {
        _target = target;
        StopAllCoroutines();
        StartCoroutine(Coroutine_WaterBend());
    }

    public void WaterBend()
    {
        var ln = TrajectoryProjector.instance.GetComponent<LineRenderer>();

        _target = ln.GetPosition(ln.positionCount - 1);

        StartCoroutine(Coroutine_WaterBend());
    }

    IEnumerator Coroutine_WaterBend()
    {
        _Spline.gameObject.SetActive(false);
        _SplashParticle.gameObject.SetActive(false);
        _PuddleParticle.gameObject.SetActive(false);

        //ConfigureSpline();
        ConfigureSpline(TrajectoryProjector.instance.GetComponent<LineRenderer>());

        _ContortAlong.Init();
        float meshLength = _ContortAlong.MeshBender.Source.Length;
        meshLength = meshLength == 0 ? 1 : meshLength;
        float totalLength = meshLength + _Spline.Length;

        Vector3 startScale = _Scale;
        startScale.x = 0;
        Vector3 targetScale = _Scale;

        float speedCurveLerp = 0;
        float length = 0;

        _PuddleParticle.gameObject.SetActive(true);
        _PuddleParticle.transform.localPosition = _Spline.nodes[0].Position;

        Vector3 startPuddleScale = Vector3.zero;
        Vector3 endPuddleScale = _PuddleParticle.transform.localScale;
        float lerp = 0;
        while (lerp < 1)
        {
            _PuddleParticle.transform.localScale = Vector3.Lerp(startPuddleScale, endPuddleScale, lerp);
            lerp += Time.deltaTime* _PuddleScaleSpeed;
            yield return null;
        }
        _Spline.gameObject.SetActive(true);
        _PuddleParticle.Play();
        while (length < (totalLength + 100))
        {
            if (length < meshLength)
            {
                _ContortAlong.ScaleMesh(Vector3.Lerp(startScale, targetScale, length / meshLength));
            }
            else
            {
                if (_PuddleParticle.isPlaying)
                {
                    _PuddleParticle.Stop();
                }

                //meshLength < length < totalLength

                //_ContortAlong.Contort((length - meshLength) / _Spline.Length);
                if (length + meshLength > totalLength+ _SplashActivationOffset)
                {
                    if (!_SplashParticle.isPlaying)
                    {
                        _SplashParticle.gameObject.SetActive(true);
                        _SplashParticle.transform.position=_target;
                        _SplashParticle.Play();
                    }
                }
            }

            float maxSpeed = 10;

            length += Time.deltaTime * _AnimSpeed * speedCurveLerp;
            speedCurveLerp =  Mathf.Clamp(speedCurveLerp + _SpeedDelta * Time.deltaTime, 0, maxSpeed);
            //speedCurveLerp += _SpeedDelta * Time.deltaTime;
            yield return null;
        }
        _Spline.gameObject.SetActive(false);
        _SplashParticle.Stop();
        Destroy(gameObject, 2f);
    }

    private void ConfigureSpline()
    {
        List<SplineNode> nodes = new List<SplineNode>(_Spline.nodes);
        for (int i = 2; i < nodes.Count; i++)
        {
            _Spline.RemoveNode(nodes[i]);
        }

        Vector3 targetDirection = (_target - transform.position);
        transform.forward = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;

        int sign = Random.Range(0, 2) == 0 ? 1 : -1;
        float angle = 90* sign;
        float height = 0;
        for (int i = 0; i < _PointCount; i++)
        {
            if (_Spline.nodes.Count <= i)
            {
                _Spline.AddNode(new SplineNode(Vector3.zero, Vector3.forward));
            }
            Vector3 normal = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 pos = transform.position + normal * _Radius;
            pos.y = height;
            Vector3 direction = pos + Quaternion.Euler(Random.Range(-30,30), Random.Range(60,120)*sign, Random.Range(-30, 30)) * normal * _Radius / 2f;
            if (i == 0)
            {
                direction = pos + Vector3.up * _Radius;
            }

            _Spline.nodes[i].Position = transform.InverseTransformPoint(pos);
            _Spline.nodes[i].Direction = transform.InverseTransformPoint(direction);

            height += _HeightDelta;
            angle += 90* sign;
        }

        Vector3 targetNodePosition = transform.InverseTransformPoint(_target);

        Quaternion randomRotation = Quaternion.Euler(Random.Range(0, 90), Random.Range(-40, 40), 0);
        Vector3 targetNodeDirection = _target + randomRotation * (transform.forward * (_target-transform.position).magnitude*Random.Range(0.2f,1f));

        targetNodeDirection= transform.InverseTransformPoint(targetNodeDirection);
        SplineNode node = new SplineNode(targetNodePosition, targetNodeDirection);
        _Spline.AddNode(node);
    }

    private void ConfigureSpline(LineRenderer line)
    {
        Vector3 targetDirection = (TrajectoryProjector.instance.transform.forward);
        transform.forward = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;

        List<SplineNode> nodes = new List<SplineNode>(_Spline.nodes);
        for (int i = 2; i < nodes.Count; i++)
        {
            _Spline.RemoveNode(nodes[i]);
        }

        _PointCount = line.positionCount - 1;

        for (int i = 0; i < _PointCount; i++)
        {


            if (_Spline.nodes.Count <= i)
            {
                _Spline.AddNode(new SplineNode(Vector3.zero, Vector3.forward));
            }

            Vector3 pos = line.GetPosition(i);

            Vector3 myAngle = line.GetPosition(i) - line.GetPosition(i + 1);

            Vector3 normal = Quaternion.Euler(myAngle) * transform.forward;
            
            //pos.y = height;
            Vector3 direction = pos + normal / 4f;
            //if (i == 0)
            //{
            //    direction = pos + Vector3.up * _Radius;
            //}

            _Spline.nodes[i].Position = transform.InverseTransformPoint(pos);
            _Spline.nodes[i].Direction = transform.InverseTransformPoint(direction);

        }
        

        //Quaternion randomRotation = Quaternion.Euler(Random.Range(0, 90), Random.Range(-40, 40), 0);
        //Vector3 targetNodeDirection = _target + randomRotation * (transform.forward * (_target - transform.position).magnitude * Random.Range(0.2f, 1f));

        //targetNodeDirection = transform.InverseTransformPoint(targetNodeDirection);
        //SplineNode node = new SplineNode(targetNodePosition, targetNodeDirection);
        //_Spline.AddNode(node);
    }
}
