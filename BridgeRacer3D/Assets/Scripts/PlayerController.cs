using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;
    public float runningSpeed, limitX, xSpeed;
    private float _currentRunningSpeed;

    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders;
    private bool _spawnBridge;
    public GameObject bridgePiecePrefab;

    private BridgeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;
    public Animator animator;

    private bool _finished = false;
    private float _scoreTimer = 0;

    private float _lastTouchedX;
    private float _dropSoundTimer;
    public AudioSource cylinderAudioSource,triggerAudioSource, itemAudioSource;
    public AudioClip gatherAudioClip, dropAudioClip,coinAudioClip, buyAudioClip,equipItemAudioClip,unEquipItemAudioClip;

    public List<GameObject> wearSpots;


    private void Update()
    {
        if (LevelController.Current == null || !LevelController.Current.gameActive)
        {
            return;
        }
        float newX = 0;
        float touchXDelta = 0;
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _lastTouchedX = Input.GetTouch(0).position.x;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                touchXDelta = 5 * (Input.GetTouch(0).position.x -  _lastTouchedX) / Screen.width;
                _lastTouchedX = Input.GetTouch(0).position.x;
            }

        }
        else if (Input.GetMouseButton(0))
        {
            touchXDelta = Input.GetAxis("Mouse X");
        }
        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);


        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        transform.position = newPosition;

        if (_spawnBridge)
        {

            _creatingBridgeTimer -= Time.deltaTime;
            if (_creatingBridgeTimer < 0)
            {
                PlayDropSound();
                _creatingBridgeTimer = .1f;
                IncrementCylinderVolume(-.1f);
                GameObject createdBridgePiece = Instantiate(bridgePiecePrefab, this.transform);
                createdBridgePiece.transform.SetParent(null);
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                createdBridgePiece.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance;
                newPiecePosition.x = transform.position.x;
                createdBridgePiece.transform.position = newPiecePosition;

                if (_finished)
                {
                    _scoreTimer -= Time.deltaTime * 75;
                    if (_scoreTimer < 0)
                    {
                        _scoreTimer = 0.3f;
                        LevelController.Current.ChangeScore(1);
                    }
                }
            }
        }
    }

    public void ChangeSpeed(float value)
    {
        _currentRunningSpeed = value;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AddCylinder")
        {
            cylinderAudioSource.PlayOneShot(gatherAudioClip, 0.1f);
            Destroy(other.gameObject);
            IncrementCylinderVolume(0.1f);
        }
        else if (other.tag == "SpawnBridge")
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag == "StopSpawnBridge")
        {
            StopSpawnBridge();
            if (_finished)
            {
                LevelController.Current.FinishGame();
            }
        }
        else if (other.tag == "Finish")
        {
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }else if (other.tag == "coin")
        {
            triggerAudioSource.PlayOneShot(coinAudioClip,.1f);
            other.tag = "Untagged";
            LevelController.Current.ChangeScore(10);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (LevelController.Current.gameActive)
        {
            if (other.tag == "Trap")
            {
                PlayDropSound();
                IncrementCylinderVolume(-Time.fixedDeltaTime);
            }
        }

    }

    public void PlayDropSound()
    {
        _dropSoundTimer -= Time.deltaTime * 100;
        if (_dropSoundTimer < 0)
        {
            _dropSoundTimer = .15f;
            cylinderAudioSource.PlayOneShot(dropAudioClip, .1f);
        }
    }
    public void IncrementCylinderVolume(float value)
    {
        if (cylinders.Count == 0)
        {
            if (value > 0)
            {
                CreateCylinder(value);
            }
            else
            {
                if (_finished)
                {
                    LevelController.Current.FinishGame();
                }
                else
                {
                    Die();
                }
            }
        }
        else
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value);
        }
    }

    public void Die()
    {
        animator.SetBool("dead", true);
        gameObject.layer = 6;
        Camera.main.transform.SetParent(null);
        LevelController.Current.GameOver();
    }

    public void CreateCylinder(float value)
    {
        RidingCylinder createdCylinder = Instantiate(ridingCylinderPrefab, transform).GetComponent<RidingCylinder>();
        cylinders.Add(createdCylinder);
        createdCylinder.IncrementCylinderVolume(value);
    }

    public void DestroyCylinder(RidingCylinder cylinder)
    {
        cylinders.Remove(cylinder);
        Destroy(cylinder.gameObject);
    }

    public void StartSpawningBridge(BridgeSpawner spawner)
    {
        _bridgeSpawner = spawner;
        _spawnBridge = true;
    }

    public void StopSpawnBridge()
    {

        _spawnBridge = false;
    }

}
