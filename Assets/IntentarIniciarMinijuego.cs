using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntentarIniciarMinijuego : MonoBehaviour
{
    [Header("Referencia al minijuego")]
    public MiniJuegoBiblioteca minijuego;

    [Header("Objetos que se activan al iniciar")]
    public GameObject panelBiblioteca;
    public GameObject juegoBiblioteca;
    public GameObject panelJuego;

    [Header("Mensaje si ya terminó")]
    public GameObject panelMensaje;
    public TextMeshProUGUI textoMensaje;

    public void IntentarIniciar()
    {
        if (minijuego != null && minijuego.juegoTerminado)
        {
            if (panelMensaje != null)
                panelMensaje.SetActive(true);

            if (textoMensaje != null)
                textoMensaje.text = "Gracias por ayudarme a entregar los libros";

            return;
        }

        if (panelBiblioteca != null)
            panelBiblioteca.SetActive(false);

        if (juegoBiblioteca != null)
            juegoBiblioteca.SetActive(true);

        if (panelJuego != null)
            panelJuego.SetActive(true);
    }
}
