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
        public Button botonObjeto;
        public Button botonPersona;
        public GameObject objetoVisualEnInventario;
        [HideInInspector] public bool yaLoTiene = false;
        [HideInInspector] public bool completada = false;
    }

    [Header("Configuración de Entregas")]
    public List<MisionEntrega> misiones = new List<MisionEntrega>();

    [Header("Paneles de Control")]
    public GameObject panelFinalizacion; // Solo se activa una vez al ganar

    [Header("Feedback de Usuario")]
    public TextMeshProUGUI textoInformativo;
    public float duracionTexto = 1.0f;

    // --- ESTO ES LO QUE BUSCAS ---
    // Al ser 'static', otros scripts pueden acceder a ella fácilmente.
    public static bool MinijuegoFinalizadoGlobal = false;

    private Coroutine rutinaTexto;

    void Start()
    {
        // Si el juego ya se completó antes (en un cambio de escena, por ejemplo)
        // podrías chequearlo aquí. Por ahora, inicia en false.
        MinijuegoFinalizadoGlobal = false;

        foreach (var mision in misiones)
        {
            if (mision.botonObjeto != null)
                mision.botonObjeto.onClick.AddListener(() => RecogerObjeto(mision));

            if (mision.botonPersona != null)
                mision.botonPersona.onClick.AddListener(() => IntentarEntrega(mision));
        }

        if (panelFinalizacion != null) panelFinalizacion.SetActive(false);
    }

    // He eliminado la función de "IntentarAbrirMinijuego" para que 
    // uses tus propios botones de navegación normales para abrir paneles.

    void RecogerObjeto(MisionEntrega mision)
    {
        // Solo podemos recoger si el juego NO ha terminado
        if (!mision.yaLoTiene && !MinijuegoFinalizadoGlobal)
        {
            mision.yaLoTiene = true;
            mision.botonObjeto.gameObject.SetActive(false);
            if (mision.objetoVisualEnInventario != null) mision.objetoVisualEnInventario.SetActive(true);

            ActualizarTexto("Recogiste: " + mision.nombreDeLaMision);
        }
    }

    void IntentarEntrega(MisionEntrega mision)
    {
        // Si ya terminó el minijuego global, el NPC podría hacer otra cosa (diálogo extra)
        if (MinijuegoFinalizadoGlobal)
        {
            ActualizarTexto("¡Gracias de nuevo por tu ayuda!");
            return;
        }

        if (mision.completada) return;

        if (mision.yaLoTiene)
        {
            mision.completada = true;
            mision.yaLoTiene = false;
            if (mision.objetoVisualEnInventario != null) mision.objetoVisualEnInventario.SetActive(false);

            ActualizarTexto("¡Entregado correctamente!");
            VerificarFinalizacion();
        }
        else
        {
            ActualizarTexto("No tienes lo que necesito...");
        }
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
            MinijuegoFinalizadoGlobal = true; // SE ACTIVA LA SEÑAL GLOBAL
            if (panelFinalizacion != null) panelFinalizacion.SetActive(true);
            ActualizarTexto("¡Misión cumplida!");
        }
    }

    void ActualizarTexto(string mensaje)
    {
        if (textoInformativo == null) return;
        if (rutinaTexto != null) StopCoroutine(rutinaTexto);
        rutinaTexto = StartCoroutine(MostrarYBorrarTexto(mensaje));
    }

    IEnumerator MostrarYBorrarTexto(string mensaje)
    {
        textoInformativo.text = mensaje;
        yield return new WaitForSeconds(duracionTexto);
        textoInformativo.text = "";
    }
}