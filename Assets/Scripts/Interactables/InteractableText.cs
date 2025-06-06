using UnityEngine;
using TMPro;

public class InteractableText : MonoBehaviour
{
    [Header("Prefab de texto (TextMeshPro 3D)")]
    [Tooltip("Debe ser un GameObject con un componente TextMeshPro (no UGUI).")]
    [SerializeField] private TextMeshPro textPrefab;

    [Header("Mensaje a mostrar")]
    [TextArea]
    [SerializeField] private string message = "¡Hola!";

    [Header("Distancia de activación")]
    [SerializeField] private float showDistance = 5f;

    [Header("Offset local sobre el objeto")]
    [SerializeField] private Vector3 localOffset = new Vector3(0, 2f, 0);

    [Header("Animación de levitación")]
    [SerializeField] private float floatAmplitude = 0.2f;   // Qué tanto sube y baja
    [SerializeField] private float floatFrequency = 2f;     // Velocidad de oscilación

    [Header("Control externo del texto")]
    public bool interactableEnabled = true;

    private Transform _player;
    [SerializeField] private PlayerMovement _playerScript;
    private TextMeshPro _instanceTMP;
    private float _baseY;

    private void Start()
    {
        // Buscar al jugador por tag
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            _player = go.transform;
        }
        else 
        { 
            Debug.LogWarning("No se encontró ningún GameObject con tag 'Player'."); 
        }

        // Instanciar el texto como objeto independiente
        if (textPrefab != null)
        {
            var goText = Instantiate(textPrefab.gameObject);
            _instanceTMP = goText.GetComponent<TextMeshPro>();
            _instanceTMP.gameObject.SetActive(false);
            goText.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogError("Asigna un TextMeshPro 3D prefab en el Inspector.");
        }

        // Valor base para el movimiento
        _baseY = localOffset.y;
    }

    private void LateUpdate()
    {
        if (_player == null || _instanceTMP == null || !interactableEnabled) return;

        float dist = Vector3.Distance(_player.position, transform.position);
        bool shouldShow = dist <= showDistance; //Estoy cercano??

        //Si la letra se ve y el player esta levitando
        if (_instanceTMP.gameObject.activeSelf && _playerScript.ElementLevitated != null)
        {
            HideUILetter();
        }

        if (_playerScript.ElementLevitated != null) return;

        if (shouldShow && !_instanceTMP.gameObject.activeSelf)
        {
            _instanceTMP.text = message;
            _instanceTMP.gameObject.SetActive(true);
        }
        else if (!shouldShow && _instanceTMP.gameObject.activeSelf)
        {
            _instanceTMP.gameObject.SetActive(false);
        }

        if (shouldShow)
        {
            float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            Vector3 animatedOffset = new Vector3(localOffset.x, _baseY + floatOffset, localOffset.z);
            _instanceTMP.transform.position = transform.position + animatedOffset;

            Vector3 dir = _instanceTMP.transform.position - Camera.main.transform.position;
            _instanceTMP.transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    private void HideUILetter()
    {
        _instanceTMP.gameObject.SetActive(false);
    }
}