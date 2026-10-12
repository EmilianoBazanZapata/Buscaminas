# 💣 MineGrid: Buscaminas pixelado

<p align="center">
  <strong>Un pequeño mundo pixelado. Una cuadrícula llena de peligro.</strong><br>
  Lee los números, confía en tu lógica y descubre todas las casillas seguras.
</p>

<p align="center">
  <a href="https://emiliano-bazan-zapata.itch.io/minegrid-pixel-minesweeper"><strong>▶ JUGAR / DESCARGAR EN ITCH.IO</strong></a>
</p>

<p align="center">
  <img alt="Unity 6" src="https://img.shields.io/badge/Unity-6000.3.5f2-222222?logo=unity&logoColor=white">
  <img alt="C Sharp" src="https://img.shields.io/badge/Hecho%20con-C%23-68217A?logo=csharp&logoColor=white">
  <img alt="Género" src="https://img.shields.io/badge/G%C3%A9nero-Puzzle%20%2F%20L%C3%B3gica-65B86E">
  <img alt="Estado" src="https://img.shields.io/badge/Estado-En%20desarrollo-F2C14E">
</p>

---

**MineGrid** reimagina el clásico Buscaminas con una estética pixelada. Cada número es una pista, cada casilla que revelas implica una decisión y un solo movimiento equivocado puede terminar la partida. Elige una dificultad, analiza la cuadrícula y descubre todas las casillas seguras.

> Creado por **Binary Suns**.

## ✨ Características

- Tres tamaños de tablero, desde un desafío rápido hasta una cuadrícula amplia llena de minas.
- Minas distribuidas al azar y pistas calculadas automáticamente según las minas vecinas.
- Expansión automática por las zonas vacías conectadas.
- Pantallas de victoria y derrota con opción para volver a intentarlo o regresar al menú.
- Cronómetro y contador de clics en tiempo real, con un resumen al terminar la partida.
- Tablero adaptable y compatible con las áreas seguras de la interfaz de Unity.
- Gráficos y tipografía con estilo pixel art.

## 🎮 Dificultad

| Modo | Tablero | Minas |
|:--|:--:|--:|
| Fácil | 9 × 9 | 10 |
| Normal | 16 × 16 | 40 |
| Difícil | 30 × 16 | 99 |

## 🕹️ Cómo jugar

1. Elige una dificultad en el menú principal e inicia una partida.
2. Haz clic en una casilla cubierta para descubrirla.
3. El número indica cuántas minas hay alrededor de esa casilla, incluidas las diagonales.
4. Si descubres una zona vacía, se abrirán automáticamente las casillas seguras conectadas.
5. Descubre todas las casillas seguras para ganar. Si encuentras una mina, la partida termina.

El cronómetro empieza con el primer clic válido. Durante la partida y en la pantalla de resultados se muestran el tiempo y la cantidad de clics.

## 🧰 Abrir el proyecto de Unity

### Requisitos

- [Unity Hub](https://unity.com/download)
- Unity Editor **6000.3.5f2**

### Primeros pasos

1. Clona o descarga este repositorio.
2. En Unity Hub, selecciona **Add project from disk** (Agregar proyecto desde el disco).
3. Selecciona la carpeta `Buscaminas/` dentro del repositorio.
4. Abre el proyecto con Unity **6000.3.5f2**.
5. Si la escena no está abierta, abre `Assets/Scenes/Game.unity`.
6. Presiona **Play**.

Al abrir el proyecto, Unity resolverá los paquetes declarados en `Packages/manifest.json`.

## 🗂️ Estructura del proyecto

```text
Buscaminas/
├── Assets/
│   ├── Prefabs/       # Prefab reutilizable de una casilla
│   ├── Scenes/        # Escena principal del juego
│   ├── Scripts/       # Reglas del juego y presentación en Unity
│   └── Sprites/       # Recursos visuales con estilo pixel art
├── Packages/          # Manifiesto y archivo de bloqueo de paquetes de Unity
└── ProjectSettings/   # Configuración del proyecto y del editor
```

### Código del juego

- `MinesweeperBoard` y `BoardCell` contienen las reglas del tablero y el estado de cada casilla.
- `GridHelper` construye la cuadrícula visual, distribuye las minas y conecta la entrada del jugador con el tablero.
- `GameManager` coordina el menú, la partida, el reinicio y los estados de victoria y derrota.
- `GameHud` muestra las estadísticas de la partida y ajusta el tablero al espacio disponible en pantalla.
- `DifficultySettings` define las dimensiones y la cantidad de minas de cada modo.

## 📦 Jugar y descargar

Puedes jugar a MineGrid o encontrar las descargas disponibles en [itch.io](https://emiliano-bazan-zapata.itch.io/minegrid-pixel-minesweeper).

Este repositorio de GitHub contiene el código fuente del proyecto de Unity. Las versiones compiladas y las descargas están disponibles en la página del juego en itch.io.

## 🛠️ Tecnologías

- Unity **6000.3.5f2**
- C#
- Unity UI (uGUI)

---

<p align="center">
  <sub>Piensa bien. Descubre con valentía. ¡Buena suerte! 💥</sub>
</p>
