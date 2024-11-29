# PruebaTecnica-Unity-Developer

Para iniciar la aplicacion ir a la carpeta builds, ahí está el ejecutable.

1. Creación del Cube

Primero de todo se tiene que crear un cube con un transform.scale de 1x1x1.

1.1 Creación de cubeScript

El CubeScript es un script simple que se adjunta a un objeto de tipo cubo en Unity. Su propósito principal es almacenar referencias a los materiales que pueden ser aplicados al cubo. Aquí tienes una explicación detallada de sus componentes y funcionalidades.

Componentes del CubeScript

Variables Públicas:
pathMaterial: Un material que se usará para el cubo cuando forme parte de un camino.
terrainMaterial: Un material que se usará para el cubo cuando forme parte del terreno.
Header Attribute:

[Header("Cube Settings")]: Un atributo que añade un encabezado en el Inspector de Unity para organizar y categorizar las variables públicas.

2. Creacion del gameObject ProceduralTerrain.

El ProceduralTerrain es un GameObject vacío en Unity que se encarga de la generación y gestión del terreno de manera procedural. Este objeto actúa como un contenedor y controlador para los scripts y componentes que generan el terreno, los caminos y otros elementos del entorno de forma dinámica durante la ejecución del juego.

2.1 Creacion del Script ProceduralTerrainScript

El ProceduralTerrainScript es un script que se encarga de la generación procedural del terreno y los caminos en tu juego. Inicializa la semilla para la generación procedural, genera los chunks del terreno y los caminos, y proporciona un método para limpiar todos los objetos generados previamente. Este script es esencial para crear un entorno dinámico y adaptable en tu juego.

Explicación de las funciones y lógica:

Variables Privadas:
seed: Semilla para la generación procedural. Si es 0, se usa la hora actual para generar una semilla aleatoria.
chunkGenerator: Referencia al generador de chunks, que se encarga de crear las secciones del terreno.
pathGenerator: Referencia al generador de caminos, que se encarga de crear los caminos a través del terreno.

Start():
Este método se llama antes de la primera actualización del frame. Aquí se llama al método GenerateTerrain para generar el terreno al inicio del juego.

GenerateTerrain():
Inicializa la semilla para la generación procedural. Si la semilla es 0, se usa la hora actual para generar una semilla aleatoria.
Verifica que los generadores de chunks y caminos no sean nulos.
Llama al método GenerateChunks del chunkGenerator para generar los chunks del terreno.
Obtiene la posición inicial del primer chunk y calcula su centro.
Llama al método GeneratePath del pathGenerator para generar el camino desde la posición inicial.
Llama al método RemoveOverlappingTerrainCubes del pathGenerator para eliminar los cubos del terreno que se superponen con el camino.

ClearAllGeneratedObjects():
Este método se encarga de limpiar todos los objetos generados previamente, incluyendo los cubos del camino, los chunks del terreno y los cubos del terreno.
Limpiar los cubos del camino:
Itera sobre la lista pathGenerator.pathCubesList y destruye cada cubo.
Limpia la lista pathGenerator.pathCubesList y reinicia el contador de cubos del camino.
Limpiar los chunks del terreno:
Itera sobre la lista chunkGenerator.chunksList y destruye cada chunk.
Limpia la lista chunkGenerator.chunksList y reinicia el contador de chunks.
Limpiar los cubos del terreno:
Itera sobre la lista chunkGenerator.terrainCubesList y destruye cada cubo.
Limpia la lista chunkGenerator.terrainCubesList y reinicia el contador de cubos del terreno.

2.3 Creación del Script GenerateChunk.

El ChunkGenerator es un script que se encarga de la generación procedural de los chunks del terreno y los cubos de terreno dentro de cada chunk. Proporciona métodos para generar los chunks, crear cubos de terreno, obtener posiciones y centros de chunks, y encontrar chunks en posiciones específicas. Este script es esencial para crear un entorno de terreno dinámico y adaptable en tu juego.

Explicación de las funciones y lógica:

Variables Públicas y Privadas:
chunkPrefab: Prefab del chunk que se usará para generar los chunks del terreno.
chunkWidth, chunkLength, chunkHeight: Dimensiones de cada chunk.
numberOfChunks: Número de chunks a generar.
chunkCounter: Contador de chunks generados.
chunkPositions: Lista de posiciones de los chunks generados.
chunksList: Lista de chunks generados.
cubePrefab: Prefab del cubo de terreno.
terrainCubesList: Lista de cubos de terreno generados.
terrainCounter: Contador de cubos de terreno generados.

GenerateChunks():
Limpia las listas de posiciones y chunks generados, y reinicia el contador de chunks.
Crea un objeto padre para los caminos.
Genera el primer chunk en la posición inicial (0, 0, 0) y lo añade a la lista de posiciones.
Genera los chunks restantes en posiciones válidas, añadiéndolos a la lista de posiciones y creando los chunks en esas posiciones.

CreateChunk(Vector3 position):
Instancia un nuevo chunk en la posición especificada.
Hace que el chunk sea hijo del objeto actual.
Añade el chunk a la lista de chunks generados.
Asigna la etiqueta "ChunkTag" y un nombre al chunk.
Incrementa el contador de chunks.
Llama al método CreateTerrainCubes para crear los cubos de terreno dentro del chunk.

CreateTerrainCubes(GameObject chunk, Vector3 position):
Crea una cuadrícula de cubos dentro del chunk, iterando sobre las dimensiones del chunk.
Calcula la posición de cada cubo y lo instancia en esa posición.
Hace que el cubo sea hijo del chunk.
Asigna la etiqueta "TerrainTag" y un nombre al cubo.
Incrementa el contador de cubos.
Añade el cubo a la lista de cubos de terreno generados.
Asigna materiales a los cubos según su altura (y = 0 para "Grass", y = 1 para "Dirt").

GetChunkCenter(Vector3 chunkPosition):
Calcula y devuelve el centro de un chunk dado su posición.

GetNextChunkPosition(Vector3 _currentPosition):
Genera una lista de posibles movimientos para el siguiente chunk.
Intenta encontrar una posición válida para el siguiente chunk eligiendo movimientos al azar y verificando si la posición está ocupada.
Si no se encuentra una posición válida, devuelve Vector3.zero.

GetChunkAt(int chunkX, int chunkZ):
Busca y devuelve el chunk en una posición específica (chunkX, chunkZ) multiplicada por las dimensiones del chunk.

GetChunkPositions():
Devuelve la lista de posiciones de los chunks generados.

2.4 Creacion del Script PathGenerator.

El PathGenerator es un script que se encarga de la generación procedural de caminos a través de los chunks del terreno. Proporciona métodos para calcular el centro de los chunks, eliminar cubos de terreno superpuestos, mover la posición actual hacia un objetivo con cierta irregularidad y expansión, crear cubos de camino, generar el camino a través de los chunks, obtener el borde objetivo para conectar con el siguiente chunk, y verificar si se puede colocar un cubo de camino en una posición dada. Este script es esencial para crear caminos dinámicos y adaptables en tu juego.

Explicación de las funciones y lógica:

Variables Públicas y Privadas:
pathCubePrefab: Prefab del cubo que representa el camino.
chunkGenerator: Referencia al generador de chunks.
pathCubesList: Lista de cubos del camino generados.
pathCounter: Contador de cubos del camino generados.
irregularity: Valor configurable entre 0 y 1 que modifica la irregularidad del camino.
expansionFactor: Valor configurable entre 0 y 1 que modifica la expansión del camino.

GetChunkCenter(int chunkX, int chunkZ):
Calcula y devuelve el centro de un chunk dado sus coordenadas X y Z.

RemoveOverlappingTerrainCubes():
Elimina los cubos de terreno que se superponen con los cubos del camino.

MoveTowards(Vector3 currentPosition, Vector3 target, float irregularity, float expansionFactor):
Mueve la posición actual hacia el objetivo con cierta irregularidad y expansión.

CreatePathCube(Vector3 _position):
Crea un cubo de camino en una posición dada si no hay otro cubo de camino en esa posición y si se puede colocar el cubo en esa posición.

GeneratePath(Vector3 _startPosition, Vector3 _currentChunk):
Genera el camino desde una posición inicial a través de los chunks.

GetTargetEdge(Vector3 currentPosition, Vector3 chunkPosition, int chunkIndex):
Obtiene el borde objetivo para conectar con el siguiente chunk.

CanPlacePathCube(Vector3 position):
Verifica si se puede colocar un cubo de camino en una posición dada, asegurándose de que no haya dos o más cubos de camino adyacentes.

SetMovementFromCenter(float value):
Establece el factor de expansión del camino.

SetIrregularity(float value):
Establece la irregularidad del camino.

2.5 Creación de Script UIManager.

El UIManager es un script que gestiona la interfaz de usuario (UI) en tu aplicación de Unity. Su propósito principal es permitir que el usuario interactúe con la UI para modificar parámetros del juego y actualizar la escena en consecuencia, este script pertenece como componente del prefab ProceduralTerrain.

Responsabilidades del UIManager:

Interacción con la UI:
Campos de entrada (TMP_InputField): Permiten al usuario ingresar valores numéricos para el número de chunks, el ancho de los chunks y la longitud de los chunks.
Sliders (Slider): Permiten al usuario ajustar el movimiento desde el centro y la irregularidad del camino.
Botón (Button): Permite al usuario aplicar los cambios ingresados y regenerar la escena.

Actualización de la UI:
Textos (TMP_Text): Muestran los valores actuales de los sliders para que el usuario pueda ver los ajustes en tiempo real.

Comunicación con otros componentes:
Generadores (PruebaChunkGenerator, PruebaPathGenerator, ProceduralTerrain): El UIManager actualiza estos componentes con los nuevos valores ingresados por el usuario y les indica que regeneren la escena.
Funcionalidades del UIManager

Inicialización:
En el método Start(), se asignan listeners a los botones y sliders para que llamen a métodos específicos cuando se interactúa con ellos.

Actualización de la UI en tiempo real:
En el método Update(), se actualizan los textos de los sliders en cada frame para reflejar sus valores actuales.

Aplicación de cambios:
En el método OnApplyButtonClicked(), se convierten los valores de los campos de entrada a enteros y se asignan al generador de chunks.
Se obtienen los valores de los sliders y se asignan al generador de caminos.
Se limpian todos los objetos generados previamente y se regenera el terreno y el camino.

Actualización de textos de sliders:
Los métodos UpdateExpansionFactor(float value) y UpdateIrregularity(float value) actualizan los textos del factor de expansión y la irregularidad con los valores de los sliders.