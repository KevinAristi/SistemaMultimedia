using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinijuegoGlobal : MonoBehaviour
{
    [System.Serializable]
    public class MisionEntrega
    {
        public string nombreDeLaMision;

        [Header("Referencias de UI")]
        public Button botonObjeto;
        public Button botonPersona;
        public GameObject objetoVisualEnInventario;

        [Header("Textos Personalizados")]
        [TextArea(2, 4)] public string mensajeAlRecoger;
        [TextArea(2, 4)] public string mensajeAlEntregar;
        [TextArea(2, 4)] public string mensajeSiNoTiene;

        [HideInInspector] public bool yaLoTiene = false;
        [HideInInspector] public bool completada = false;
    }

    [Header("Configuración de Entregas")]
    public List<MisionEntrega> misiones = new List<MisionEntrega>();

    [Header("Paneles de Control")]
    public GameObject panelFinalizacion;
    public GameObject panelTextoFeedback; // <--- EL NUEVO PANEL DE FONDO

    [Header("Feedback de Usuario")]
    public TextMeshProUGUI textoInformativo;
    public float duracionTexto = 2.0f; // Te sugiero 2 segundos para que alcancen a leer

    public static bool MinijuegoFinalizadoGlobal = false;
    private Coroutine rutinaTexto;

    void Start()
    {
        MinijuegoFinalizadoGlobal = false;

        // Asegurarnos de que el panel y el texto empiecen apagados
        if (panelTextoFeedback != null) panelTextoFeedback.SetActive(false);
        if (textoInformativo != null) textoInformativo.text = "";

        List<Button> botonesRegistrados = new List<Button>();

        foreach (var m in misiones)
        {
            MisionEntrega misionLocal = m;

            if (misionLocal.botonObjeto != null)
                misionLocal.botonObjeto.onClick.AddListener(() => RecogerObjeto(misionLocal));

            if (misionLocal.botonPersona != null && !botonesRegistrados.Contains(misionLocal.botonPersona))
            {
                botonesRegistrados.Add(misionLocal.botonPersona);
                misionLocal.botonPersona.onClick.AddListener(() => ProcesarEntregaGeneral(misionLocal.botonPersona));
            }
        }

        if (panelFinalizacion != null) panelFinalizacion.SetActive(false);
    }

    // ... (Mantengo las funciones RecogerObjeto, ProcesarEntregaGeneral y VerificarFinalizacion igual que antes) ...

    void RecogerObjeto(MisionEntrega mision)
    {
        if (!mision.yaLoTiene && !MinijuegoFinalizadoGlobal)
        {
            mision.yaLoTiene = true;
            mision.botonObjeto.gameObject.SetActive(false);
            if (mision.objetoVisualEnInventario != null) mision.objetoVisualEnInventario.SetActive(true);
            ActualizarTexto(mision.mensajeAlRecoger);
        }
    }

    void ProcesarEntregaGeneral(Button npcPresionado)
    {
        if (MinijuegoFinalizadoGlobal)
        {
            ActualizarTexto("¡Gracias por todo tu esfuerzo!");
            return;
        }

        MisionEntrega misionParaCompletar = null;
        string mensajeDeError = "";

        foreach (var m in misiones)
        {
            if (m.botonPersona == npcPresionado && !m.completada)
            {
                if (m.yaLoTiene)
                {
                    misionParaCompletar = m;
                    break;
                }
                else
                {
                    mensajeDeError = m.mensajeSiNoTiene;
                }
            }
        }

        if (misionParaCompletar != null)
        {
            CompletarMision(misionParaCompletar);
        }
        else if (!string.IsNullOrEmpty(mensajeDeError))
        {
            ActualizarTexto(mensajeDeError);
        }
    }

    void CompletarMision(MisionEntrega mision)
    {
        mision.completada = true;
        mision.yaLoTiene = false;
        if (mision.objetoVisualEnInventario != null) mision.objetoVisualEnInventario.SetActive(false);

        ActualizarTexto(mision.mensajeAlEntregar);
        VerificarFinalizacion();
    }

    void VerificarFinalizacion()
    {
        bool todasListas = true;
        foreach (var m in misiones)
        {
            if (!m.completada) todasListas = false;
        }

        if (todasListas)
        {
            MinijuegoFinalizadoGlobal = true;
            if (panelFinalizacion != null) panelFinalizacion.SetActive(true);
            ActualizarTexto("¡Misión cumplida!");
        }
    }

    // --- LÓGICA DE TEXTO Y PANEL ACTUALIZADA ---
    void ActualizarTexto(string mensaje)
    {
        if (string.IsNullOrEmpty(mensaje)) return;

        if (rutinaTexto != null) StopCoroutine(rutinaTexto);
        rutinaTexto = StartCoroutine(MostrarYBorrarTexto(mensaje));
    }

    IEnumerator MostrarYBorrarTexto(string mensaje)
    {
        // Encendemos ambos
        if (panelTextoFeedback != null) panelTextoFeedback.SetActive(true);
        if (textoInformativo != null) textoInformativo.text = mensaje;

        yield return new WaitForSeconds(duracionTexto);

        // Apagamos ambos
        if (textoInformativo != null) textoInformativo.text = "";
        if (panelTextoFeedback != null) panelTextoFeedback.SetActive(false);
    }
}