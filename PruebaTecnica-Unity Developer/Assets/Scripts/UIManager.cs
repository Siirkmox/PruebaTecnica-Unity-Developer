using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Referencias a otros componentes y UI elements
    public ChunkGenerator chunkGenerator;
    public PathGenerator pathGenerator;
    public ProceduralTerrain proceduralTerrain;
    public TMP_InputField numberOfChunksInput;
    public TMP_InputField chunkWidthInput;
    public TMP_InputField chunkLengthInput;
    public Slider movementFromCenterSlider;
    public Slider irregularitySlider;
    public Button restartButton;
    public Button exitButton;
    public TMP_Text expansionFactorText;
    public TMP_Text irregularityText;

    void Start()
    {
        // Asignar listeners a los botones y sliders
        restartButton.onClick.AddListener(OnApplyButtonClicked);
        movementFromCenterSlider.onValueChanged.AddListener(UpdateExpansionFactor);
        irregularitySlider.onValueChanged.AddListener(UpdateIrregularity);
    }

    void Update()
    {
        // Actualizar los textos de los sliders en cada frame
        expansionFactorText.text = movementFromCenterSlider.value.ToString("0.00");
        irregularityText.text = irregularitySlider.value.ToString("0.00");
        exitButton.onClick.AddListener(ExitApplication);
    }

    void ExitApplication()
    {
        Debug.Log("Exiting application...");
        Application.Quit();
    }

    // Método llamado cuando se hace clic en el botón de reinicio
    void OnApplyButtonClicked()
    {
        int numberOfChunks, chunkWidth, chunkLength;
        float movementFromCenter, irregularity;

        // Intentar convertir los valores de los input fields a enteros
        if (int.TryParse(numberOfChunksInput.text, out numberOfChunks))
        {
            chunkGenerator.numberOfChunks = numberOfChunks;
        }

        if (int.TryParse(chunkWidthInput.text, out chunkWidth))
        {
            chunkGenerator.chunkWidth = chunkWidth;
        }

        if (int.TryParse(chunkLengthInput.text, out chunkLength))
        {
            chunkGenerator.chunkLength = chunkLength;
        }

        // Obtener los valores de los sliders
        movementFromCenter = movementFromCenterSlider.value;
        irregularity = irregularitySlider.value;

        // Asignar los valores a los generadores correspondientes
        pathGenerator.SetMovementFromCenter(movementFromCenter);
        pathGenerator.SetIrregularity(irregularity);
        
        // Limpiar todos los objetos generados previamente
        proceduralTerrain.ClearAllGeneratedObjects();

        // Regenerar el terreno y el camino
        proceduralTerrain.GenerateTerrain();
    }

    // Método para actualizar el texto del factor de expansión
    void UpdateExpansionFactor(float value)
    {
        expansionFactorText.text = value.ToString("0.00");
    }

    // Método para actualizar el texto de la irregularidad
    void UpdateIrregularity(float value)
    {
        irregularityText.text = value.ToString("0.00");
    }
}