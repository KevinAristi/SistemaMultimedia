using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniJuegoBiblioteca : MonoBehaviour
{
    [Header("Estado del juego")]
    public bool juegoTerminado = false;

    [Header("Movimiento")]
    public float velocidad = 5f;
    private Rigidbody2D rb;
    private Vector2 movimiento;

    [Header("Referencias")]
    public GameObject[] libros;
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public GameObject panelRetoCompletado;

    [Header("Mensajes")]
    public string mensajeInicial = "Entrega los libros al bibliotecario UAO";
    public string mensajeRecoger = "¡Has encontrado un libro! Presiona 'E' para recogerlo.";
    public string mensajeEntregar = "¡Tienes libros para entregar! Presiona 'E' para entregarlos.";
    public string mensajeNPCInicio = "¡Hola! Necesitamos encontrar 3 libros perdidos. Busca por la biblioteca.";
    public string mensajeAdvertencia = "Oye, ¿no deseas entregar algún libro? Recuerda que si no lo haces a tiempo tendrás una pequeña multa.";

    private int librosRecogidos = 0;
    private int librosEntregados = 0;

    private bool cercaDelLibro = false;
    private bool cercaDelNPC = false;
    private bool primerLibroRecogido = false;

    private GameObject libroActual;

    // Mensaje especial del NPC que permanece mientras sigas en su rango
    private bool mensajeNPCActivo = false;
    private string mensajeNPCActual = "";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (panelDialogo != null)
            panelDialogo.SetActive(true);

        if (panelRetoCompletado != null)
            panelRetoCompletado.SetActive(false);

        actualizarDialogo();
    }

    void Update()
    {
        // Movimiento
        movimiento.x = Input.GetAxisRaw("Horizontal");
        movimiento.y = Input.GetAxisRaw("Vertical");

        // Recoger libro
        if (cercaDelLibro && Input.GetKeyDown(KeyCode.E))
        {
            RecogerLibro();
        }
        // Interacción con NPC
        else if (cercaDelNPC && Input.GetKeyDown(KeyCode.E))
        {
            if (librosRecogidos > 0)
            {
                EntregarLibro();
            }
            else
            {
                ActivarMensajeNPC(mensajeAdvertencia);
            }
        }

        actualizarDialogo();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movimiento * velocidad * Time.fixedDeltaTime);
    }

    void RecogerLibro()
    {
        if (libroActual == null) return;

        librosRecogidos++;
        libroActual.SetActive(false);
        cercaDelLibro = false;
        libroActual = null;

        if (!primerLibroRecogido)
            primerLibroRecogido = true;

        Debug.Log("Libro recogido. Total: " + librosRecogidos);
    }

    void EntregarLibro()
    {
        librosRecogidos--;
        librosEntregados++;

        if (librosEntregados == 1)
        {
            ActivarMensajeNPC("¡Gracias! Pero aún faltan 2 libros, deben estar por la biblioteca.");
        }
        else if (librosEntregados == 2)
        {
            ActivarMensajeNPC("Buen trabajo, solo falta 1 libro más.");
        }
        else if (librosEntregados == 3)
        {
            mensajeNPCActivo = false;
            mensajeNPCActual = "";

            if (panelDialogo != null)
                panelDialogo.SetActive(false);

            if (panelRetoCompletado != null)
                panelRetoCompletado.SetActive(true);
        }

        Debug.Log("Libro entregado. Total entregados: " + librosEntregados);
    }

    void ActivarMensajeNPC(string mensaje)
    {
        mensajeNPCActivo = true;
        mensajeNPCActual = mensaje;
    }

    void actualizarDialogo()
    {
        if (panelRetoCompletado != null && panelRetoCompletado.activeSelf)
        {
            if (panelDialogo != null)
                panelDialogo.SetActive(false);
            return;
        }

        if (panelDialogo != null)
            panelDialogo.SetActive(true);

        // Prioridad 1: mensaje especial del NPC mientras sigas dentro del rango
        if (cercaDelNPC && mensajeNPCActivo)
        {
            textoDialogo.text = mensajeNPCActual;
            return;
        }

        // Prioridad 2: mensajes contextuales normales
        if (cercaDelLibro)
        {
            textoDialogo.text = mensajeRecoger;
        }
        else if (cercaDelNPC && librosRecogidos > 0)
        {
            textoDialogo.text = mensajeEntregar;
        }
        else if (cercaDelNPC && librosEntregados == 0)
        {
            textoDialogo.text = mensajeNPCInicio;
        }
        else if (cercaDelNPC && librosRecogidos == 0 && librosEntregados > 0 && librosEntregados < 3)
        {
            textoDialogo.text = "¿Aún no encuentras los libros? Deben estar cerca.";
        }
        else if (!primerLibroRecogido)
        {
            textoDialogo.text = mensajeInicial;
        }
        else
        {
            panelDialogo.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Libro"))
        {
            if (collision.gameObject.activeSelf)
            {
                cercaDelLibro = true;
                libroActual = collision.gameObject;
            }
        }

        if (collision.CompareTag("NPC"))
        {
            cercaDelNPC = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Libro"))
        {
            if (libroActual == collision.gameObject)
            {
                cercaDelLibro = false;
                libroActual = null;
            }
        }

        if (collision.CompareTag("NPC"))
        {
            cercaDelNPC = false;

            // Al salir del NPC se limpia el mensaje especial
            mensajeNPCActivo = false;
            mensajeNPCActual = "";
        }
    }

    

    public void FinalizarMinijuego()
    {
        juegoTerminado = true;

        if (panelRetoCompletado != null)
            panelRetoCompletado.SetActive(false);
    }

}
