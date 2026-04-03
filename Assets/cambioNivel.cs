using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cambioNivel : MonoBehaviour
{
    [Header("Configuración de Paneles")]
    [Tooltip("El panel que se activará al presionar el botón")]
    public GameObject panelToOpen;

    [Tooltip("El panel que se desactivará al presionar el botón")]
    public GameObject panelToClose;

    // Este método es el que debes asignar en el OnClick() del botón
    public void ExecuteSwitch()
    {
        // Validamos que los paneles no sean nulos para evitar errores en consola
        if (panelToClose != null)
        {
            panelToClose.SetActive(false);
        }

        if (panelToOpen != null)
        {
            panelToOpen.SetActive(true);
        }

        // Aquí podrías añadir lógica extra más adelante, 
        // como disparar sonidos o efectos de distorsión.
    }
}
