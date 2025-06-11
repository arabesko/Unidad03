using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ElevatorPower : MonoBehaviour
{
    [Header("Elevator Settings")]
    [SerializeField] private GameObject elevator;
    [SerializeField] private Transform upPosition;
    [SerializeField] private Transform downPosition;
    [SerializeField] private float speed = 2f;

    [Header("UI")]
    [SerializeField] private GameObject elevatorPromptPanel;
    [SerializeField] private TextMeshProUGUI elevatorPromptText;

    private bool hasPower = false;
    private bool playerOnPlatform = false;
    private bool isMoving = false;
    [SerializeField] private Light statusLight;

    [Header("Sonido")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip elevatorMoveClip;

    [Header("PortaBateria")]
    [SerializeField] private Transform _batteryBox;
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private float _speedBoxBattery;
    [SerializeField] private bool _playerInAreaBatteryBox;
    [SerializeField] private bool _isIntaling = false;

    [SerializeField] private List<Transform> _points;
    [SerializeField] private GameObject _battery;
    [SerializeField] private Rigidbody _rbBattery;
    private int _indexBattery = 0;
    [SerializeField] private float _batterySpeed = 5;
    [SerializeField] private float _batterySpeedRotation = 5;
    [SerializeField] private bool _activateAscensor = false;
    [SerializeField] private PlayerMovement _playerScript;
    [SerializeField] private float offsetY = -90f;

    private void Start()
    {
        if (statusLight != null)
            statusLight.color = Color.red;
    }

    private void Update()
    {
        if (_isIntaling)
        {
            Vector3 dir = (_points[_indexBattery].transform.position - _battery.transform.position).normalized;
            _battery.transform.position += dir * _batterySpeed * Time.deltaTime;
            GirarHacia(_points[_indexBattery].transform.position);
            if (Vector3.Distance(_battery.transform.position, _points[_indexBattery].transform.position) < 0.2f)
            {
                _indexBattery++;
            }
            if (_indexBattery > _points.Count - 1)
            {
                _activateAscensor=true;
                _isIntaling = false;
                _battery.transform.SetParent(_batteryBox);
                StartCoroutine(OpenCloseBoxBattery(_pointA));
                ActivateAscensor();
            }
        }
    }

    private void GirarHacia(Vector3 target)
    {
        Vector3 direccion = (target - _battery.transform.position);
        direccion.y = 0;
        if (direccion.sqrMagnitude < 0.001f) return;

        Quaternion rotDeseada = Quaternion.LookRotation(direccion.normalized, Vector3.up);
        Quaternion rotCorregida = rotDeseada * Quaternion.Euler(0, offsetY, 0);

        _battery.transform.rotation = Quaternion.Slerp(_battery.transform.rotation, rotCorregida,
                                              _batterySpeedRotation * Time.deltaTime);
    }

    private void ActivateAscensor()
    {
        hasPower = true;
        Debug.Log("Batería instalada, elevador con energía.");


        // Cambiar luz a verde
        if (statusLight != null)
            statusLight.color = Color.green;

        if (elevatorPromptPanel != null)
            elevatorPromptPanel.SetActive(false);

        if (playerOnPlatform && !isMoving)
        {
            StartCoroutine(MoveElevator());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            PortableBattery battery = other.GetComponent<PortableBattery>();
            if (battery != null && battery.isCharged)
            {
                var UIBattery = other.GetComponent<InteractableText>();
                UIBattery._isUIActivate = false;
                StartCoroutine(OpenCloseBoxBattery(_pointB));
                _playerScript.colectables.Remove(_battery);
                _playerScript.NoLevitate();
                _rbBattery.isKinematic = true;
                _isIntaling = true;
            }
        }
        else if (other.CompareTag("Player"))
        {
            //SetPlayerOnPlatform(true);

            if (!hasPower)
            {
                if (elevatorPromptPanel != null)
                    elevatorPromptPanel.SetActive(true);

                if (elevatorPromptText != null)
                    elevatorPromptText.text = "Le falta energía al elevador.";
            }

            if (_batteryBox != null && _pointA != null & _pointB != null && _speedBoxBattery > 0)
            {
                StopAllCoroutines();
                if(!_activateAscensor) StartCoroutine(OpenCloseBoxBattery(_pointB));
            }
        }
    }

    private IEnumerator OpenCloseBoxBattery(Transform point)
    {
        while (Vector3.Distance(_batteryBox.position, point.position) > 0.2f)
        {
            var dir = (point.position - _batteryBox.position).normalized;
            _batteryBox.position += dir * _speedBoxBattery * Time.deltaTime;
            yield return null;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInAreaBatteryBox = false;
            StopAllCoroutines();
            if (!_isIntaling) StartCoroutine(OpenCloseBoxBattery(_pointA));
            SetPlayerOnPlatform(false);

            if (elevatorPromptPanel != null)
                elevatorPromptPanel.SetActive(false);
        }
    }

    public void SetPlayerOnPlatform(bool state)
    {
        playerOnPlatform = state;

        if (hasPower && playerOnPlatform)
        {
            StartCoroutine(MoveElevator());
        }
    }

    public bool HasPower()
    {
        return hasPower;
    }

    IEnumerator MoveElevator()
    {
        isMoving = true;

        // Reproducir sonido de elevador
        if (audioSource != null && elevatorMoveClip != null)
        {
            audioSource.PlayOneShot(elevatorMoveClip);
        }

        Vector3 currentPos = elevator.transform.position;
        Vector3 target;

        if (Vector3.Distance(currentPos, downPosition.position) < 0.1f)
        {
            target = upPosition.position;
        }
        else
        {
            target = downPosition.position;
        }

        while (Vector3.Distance(elevator.transform.position, target) > 0.05f)
        {
            elevator.transform.position = Vector3.MoveTowards(elevator.transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
    }

   
}
