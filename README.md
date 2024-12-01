# PruebaTecnica-Unity-Developer

Samuel Alcaraz Rodriguez

Para iniciar la aplicacion se debe descargar la carpeta App, dentro habrá un ejecutable.
Hay que decir que la irregularidad y la expansión no funcionan como deberían.

1. Generador Procedural de Terreno y Caminos

Este proyecto es un generador procedural de terreno y caminos en Unity. Utiliza varios scripts para generar chunks de terreno, caminos que se expanden desde el centro de los chunks, y una interfaz de usuario para controlar los parámetros de generación.

  1.1 Estructura del Proyecto

    1.1.1 Scripts Principales

      - ChunkGenerator.cs: Genera los chunks de terreno y los cubos de terreno dentro de cada chunk.
      - PathGenerator.cs: Genera caminos que se expanden desde el centro de los chunks.
      - ProceduralTerrain.cs: Coordina la generación del terreno y los caminos.
      - UIManager.cs: Controla la interfaz de usuario y permite ajustar los parámetros de generación.
      - CameraController.cs: Ajusta la posición y el zoom de la cámara para centrarse en el terreno generado.
      - CubeScript.cs: Define los materiales para los cubos de camino y terreno.

    1.1.2 Prefabs

      - chunkPrefab: Prefab que representa un chunk de terreno.
      - cubePrefab: Prefab que representa un cubo de terreno.
      - ProceduraTerrain Prefab: Objeto vacio donde se añaden los componentes Scripts.
      
  1.2 Cómo Funciona

    1.2.1 ChunkGenerator

      - GenerateChunks(): Genera los chunks de terreno y los cubos de terreno dentro de cada chunk.
      - CreateChunk(Vector3 position): Crea un chunk en una posición especificada.
      - CreateTerrainCubes(GameObject chunk, Vector3 position): Crea una cuadrícula de cubos dentro del chunk.
      - GetChunkCenter(Vector3 chunkPosition): Obtiene el centro de un chunk.
      - GetNextChunkPosition(Vector3 currentPosition): Obtiene la posición del siguiente chunk.
      - GetChunkAt(int chunkX, int chunkZ): Obtiene el chunk en una posición específica.

    1.2.2 PathGenerator

      - GeneratePath(Vector3 startPosition, Vector3 currentChunk): Genera el camino desde una posición inicial a través de los chunks.
      - MoveTowards(Vector3 currentPosition, Vector3 target, float irregularity, float expansionFactor): Mueve la posición actual hacia el objetivo con cierta irregularidad y expansión.
      - CreatePathCube(Vector3 position): Crea un cubo de camino en una posición dada.
      - RemoveOverlappingTerrainCubes(): Elimina los cubos de terreno que se superponen con los cubos del camino.
      - GenerateBranch(Vector3 startPosition, Vector3 currentChunk, int chunkIndex): Genera una bifurcación del camino.
      - GetTargetEdge(Vector3 currentPosition, Vector3 chunkPosition, int chunkIndex): Obtiene el borde objetivo para conectar con el siguiente chunk.
      - GetTargetEdgeBranch(Vector3 currentPosition, Vector3 chunkPosition, int chunkIndex): Obtiene el borde objetivo para una bifurcación del camino.
      - CanPlacePathCube(Vector3 position): Verifica si se puede colocar un cubo de camino en una posición dada.
      - SetMovementFromCenter(float value): Establece el factor de expansión del camino.
      - SetIrregularity(float value): Establece la irregularidad del camino.

    1.2.3 ProceduralTerrain

      - GenerateTerrain(): Genera el terreno al inicio.
      - ClearAllGeneratedObjects(): Limpia todos los objetos generados previamente.
      
    1.2.4 UIManager
      
      - OnApplyButtonClicked(): Método llamado cuando se hace clic en el botón de reinicio.
      - UpdateExpansionFactor(float value): Método para actualizar el texto del factor de expansión.
      - UpdateIrregularity(float value): Método para actualizar el texto de la irregularidad.
      - ExitApplication(): Método para salir de la aplicación.
      
    1.2.5 CameraController
      
      - CenterAndZoomCamera(): Método para centrar y ajustar el zoom de la cámara.

  1.3 Cómo Utilizar
      
    1.3.1 Configuración Inicial:

      - Asegúrate de que los prefabs chunkPrefab, cubePrefab, y pathCubePrefab estén asignados en el inspector de Unity.
      - Asigna los scripts ChunkGenerator, PathGenerator, ProceduralTerrain, UIManager, y CameraController a los objetos correspondientes en la escena.
    
    1.3.2 Interfaz de Usuario:

      - Utiliza los campos de entrada para especificar el número de chunks, el ancho y largo de los chunks.
      - Ajusta los sliders para controlar el factor de expansión y la irregularidad del camino.
      - Haz clic en el botón de reinicio para aplicar los cambios y regenerar el terreno y los caminos.
      - Haz clic en el botón de salida para cerrar la aplicación.
    
    1.3.3 Generación del Terreno y Caminos:

      - Al iniciar la aplicación, el terreno y los caminos se generarán automáticamente.
      - La cámara se centrará y ajustará el zoom para mostrar todo el terreno generado.

  1.4 Ejemplo de Uso

      - Inicia la aplicación.
      - Ajusta los parámetros de generación en la interfaz de usuario.
      - Haz clic en el botón de reinicio para aplicar los cambios.
      - Observa cómo se genera el terreno y los caminos en la escena.
      
  1.5 Notas Adicionales
      
      - Puedes ajustar los materiales de los cubos de camino y terreno en el script CubeScript.
      - Asegúrate de que los prefabs y scripts estén correctamente asignados en el inspector de Unity para evitar errores.
      
Este proyecto proporciona una base sólida para la generación procedural de terrenos y caminos en Unity. Puedes expandir y personalizar el sistema según tus necesidades específicas.