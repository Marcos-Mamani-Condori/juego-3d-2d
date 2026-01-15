# 📁 GUÍA DE ORGANIZACIÓN DEL PROYECTO
## Estructura de Carpetas Profesional para Unity

---

## 🎯 ESTRUCTURA RECOMENDADA

```
Assets/
│
├── 📁 Scripts/
│   ├── 📁 Player/
│   │   ├── personajecontroller.cs
│   │   ├── goflballcontroller.cs
│   │   └── camerafollow.cs
│   │
│   ├── 📁 Obstacles/
│   │   ├── MovingWall.cs
│   │   ├── WindZone.cs
│   │   ├── RotatingPlatform.cs
│   │   ├── LaserBarrier.cs
│   │   ├── JumpPad.cs
│   │   ├── Pendulum.cs
│   │   ├── DisappearingPlatform.cs
│   │   ├── TeleportPortal.cs
│   │   ├── ConveyorBelt.cs
│   │   ├── PressureButton.cs
│   │   ├── SpikeTrap.cs
│   │   ├── GravityZone.cs
│   │   ├── IcePlatform.cs
│   │   └── BlackHole.cs
│   │
│   ├── 📁 Enemies/
│   │   ├── EnemyHealth.cs
│   │   ├── EnemyShipMover.cs
│   │   ├── EnemyShipTarget.cs
│   │   ├── ShieldBarrierController.cs
│   │   └── ShieldModuleScript.cs
│   │
│   ├── 📁 Systems/
│   │   ├── CheckpointSystem.cs
│   │   ├── Checkpoint.cs
│   │   ├── FreeCameraController.cs
│   │   ├── gamepuntaje.cs (GameManager)
│   │   └── cambiadorEscena.cs
│   │
│   ├── 📁 Animations/
│   │   ├── CharacterAnimationHelper.cs
│   │   └── SaltarAnimacion.cs
│   │
│   ├── 📁 UI/
│   │   ├── ControladorVideoConSalto.cs
│   │   └── LogicaHuecoAvanzada.cs
│   │
│   └── 📁 Barriers/ (Obsoleto - MovingBarrier puede ir aquí)
│       └── MovingBarrier.cs
│
├── 📁 Prefabs/
│   ├── 📁 Player/
│   │   ├── Robot.prefab
│   │   └── GolfBall.prefab
│   │
│   ├── 📁 Obstacles/
│   │   ├── MovingWall.prefab
│   │   ├── WindZone.prefab
│   │   ├── RotatingPlatform.prefab
│   │   └── ... (todos los obstáculos)
│   │
│   ├── 📁 Enemies/
│   │   └── EnemyShip.prefab
│   │
│   └── 📁 Systems/
│       └── Checkpoint.prefab
│
├── 📁 Materials/
│   ├── 📁 Player/
│   ├── 📁 Obstacles/
│   ├── 📁 Terrain/
│   └── 📁 Effects/
│
├── 📁 Textures/
│   ├── 📁 Environment/
│   ├── 📁 UI/
│   └── 📁 Effects/
│
├── 📁 Scenes/
│   ├── IntroVideo.unity
│   ├── MenuPrincipal.unity
│   └── SampleScene.unity (renombrar a "Nivel_01.unity")
│
├── 📁 Audio/
│   ├── 📁 Music/
│   ├── 📁 SFX/
│   │   ├── 📁 Player/
│   │   ├── 📁 Obstacles/
│   │   └── 📁 Enemies/
│   └── 📁 Ambience/
│
├── 📁 Models/
│   ├── 📁 Environment/
│   ├── 📁 Player/
│   └── 📁 Obstacles/
│
├── 📁 Animations/
│   ├── 📁 Player/
│   └── 📁 Enemies/
│
├── 📁 Resources/
│   └── (Recursos que se cargan dinámicamente)
│
├── 📁 Documentation/
│   ├── GUIA_OBSTACULOS.md
│   ├── GUIA_ORGANIZACION.md
│   └── README.md
│
└── 📁 Editor/
    └── (Scripts de editor personalizado)
```

---

## 🚀 PASOS PARA REORGANIZAR (Hazlo desde Unity)

### ⚠️ IMPORTANTE: 
**NO muevas archivos desde el explorador de Windows.** Siempre hazlo desde Unity para mantener las referencias.

### Método 1: Crear carpetas y mover manualmente (RECOMENDADO)

1. **Abre Unity**
2. **En el panel Project**, dentro de Assets:
   - Clic derecho → Create → Folder
   - Crea estas carpetas principales:
     - `Scripts`
     - `Prefabs`
     - `Materials`
     - `Textures`
     - `Scenes`
     - `Audio`
     - `Models`
     - `Documentation`

3. **Dentro de Scripts**, crea subcarpetas:
   - `Player`
   - `Obstacles`
   - `Enemies`
   - `Systems`
   - `Animations`
   - `UI`

4. **Arrastra cada script** a su carpeta correspondiente:
   - Unity actualizará automáticamente las referencias
   - NO se romperá nada si lo haces desde Unity

---

## 📋 LISTA DE MOVIMIENTOS POR SCRIPT

### 📂 Scripts/Player/
```
personajecontroller.cs
goflballcontroller.cs
camerafollow.cs
```

### 📂 Scripts/Obstacles/
```
MovingWall.cs
WindZone.cs
RotatingPlatform.cs
LaserBarrier.cs
JumpPad.cs
Pendulum.cs
DisappearingPlatform.cs
TeleportPortal.cs
ConveyorBelt.cs
PressureButton.cs
SpikeTrap.cs
GravityZone.cs
IcePlatform.cs
BlackHole.cs
```

### 📂 Scripts/Enemies/
```
EnemyHealth.cs
EnemyShipMover.cs
EnemyShipTarget.cs
ShieldBarrierController.cs
ShieldModuleScript.cs
```

### 📂 Scripts/Systems/
```
CheckpointSystem.cs
Checkpoint.cs
FreeCameraController.cs
gamepuntaje.cs
cambiadorEscena.cs
```

### 📂 Scripts/Animations/
```
CharacterAnimationHelper.cs
SaltarAnimacion.cs
```

### 📂 Scripts/UI/
```
ControladorVideoConSalto.cs
LogicaHuecoAvanzada.cs
```

### 📂 Scripts/Barriers/ (Obsoleto)
```
MovingBarrier.cs
```

### 📂 Documentation/
```
GUIA_OBSTACULOS.md
GUIA_ORGANIZACION.md
```

---

## 🎨 ORGANIZACIÓN DE ASSETS VISUALES

### Materials (materiales/):
```
Materials/
├── Player/
│   ├── RobotMaterial.mat
│   └── BallMaterial.mat
├── Obstacles/
│   ├── LaserActive.mat
│   ├── LaserInactive.mat
│   ├── IceMaterial.mat
│   └── BlackHoleMaterial.mat
├── Terrain/
│   └── NewLayer.terrainlayer
└── Effects/
```

### Textures (texturas/):
```
Textures/
├── Environment/
├── UI/
└── Effects/
```

### Models (staticmesh/):
Renombrar `staticmesh` a `Models`:
```
Models/
├── Environment/
├── Player/
└── Props/
```

---

## 🎯 VENTAJAS DE ESTA ORGANIZACIÓN

### ✅ Claridad:
- Encuentras archivos en segundos
- Nuevos desarrolladores entienden la estructura
- Fácil identificar qué hace cada script

### ✅ Escalabilidad:
- Agregar nuevos obstáculos es trivial
- Puedes crear niveles rápidamente
- Fácil hacer backups selectivos

### ✅ Colaboración:
- Estructura estándar de la industria
- Compatible con Git (menos conflictos)
- Fácil asignar tareas por carpeta

### ✅ Performance:
- Unity indexa mejor carpetas organizadas
- Builds más rápidos
- Menos memoria al cargar Project

---

## 🔄 CONVENCIONES DE NOMBRES

### Scripts:
- **PascalCase**: `PlayerController.cs`
- **Descriptivos**: `EnemyShipMover.cs` (no `Enemy1.cs`)
- **Sin espacios**: Usa guiones bajos si necesario

### Prefabs:
- **PascalCase**: `MovingWall.prefab`
- **Prefijo por tipo**: 
  - `PF_MovingWall.prefab` (Prefab)
  - `MAT_Laser.mat` (Material)
  - `TEX_Ground.png` (Texture)

### Scenes:
- **Números**: `Nivel_01.unity`, `Nivel_02.unity`
- **Descriptivos**: `MenuPrincipal.unity`

---

## 📝 RENOMBRAR ARCHIVOS EXISTENTES

### Sugerencias de Renombres:

| Archivo Actual | Nombre Sugerido |
|----------------|-----------------|
| `personajecontroller.cs` | `PlayerController.cs` |
| `goflballcontroller.cs` | `GolfBallController.cs` |
| `camerafollow.cs` | `CameraFollow.cs` |
| `gamepuntaje.cs` | `GameManager.cs` |
| `cambiadorEscena.cs` | `SceneChanger.cs` |
| `staticmesh/` | `Models/` |
| `materiales/` | `Materials/` |
| `texturas/` | `Textures/` |
| `entorno/` | `Environment/` |

**Para renombrar:**
1. Selecciona el archivo en Unity
2. Presiona F2 o clic derecho → Rename
3. Cambia el nombre
4. Unity actualizará referencias automáticamente

---

## 🛡️ CREAR ASSEMBLY DEFINITIONS (Avanzado)

Para proyectos grandes, divide el código en assemblies:

```
Scripts/
├── Player.asmdef
├── Obstacles.asmdef
├── Enemies.asmdef
└── Systems.asmdef
```

**Ventajas:**
- Compilación más rápida (solo recompila lo cambiado)
- Dependencias claras
- Mejor organización lógica

**Cómo crear:**
1. Clic derecho en carpeta → Create → Assembly Definition
2. Nombra igual que la carpeta
3. Define dependencias en el Inspector

---

## 📦 CREAR PREFABS ORGANIZADOS

### Convención de Prefabs:

```
Prefabs/
├── PF_Player_Robot.prefab
├── PF_Obstacle_MovingWall.prefab
├── PF_Enemy_Ship.prefab
└── PF_System_Checkpoint.prefab
```

**Nombrado:**
- `PF_` = Prefab
- `Categoría_NombreDescriptivo.prefab`

---

## 🎬 PLAN DE ACCIÓN PASO A PASO

### Día 1: Estructura Base (30 minutos)
1. ✅ Crear carpetas principales en Assets
2. ✅ Crear subcarpetas en Scripts
3. ✅ Mover scripts a sus carpetas

### Día 2: Assets Visuales (20 minutos)
1. ✅ Organizar Materials
2. ✅ Organizar Textures
3. ✅ Renombrar staticmesh a Models

### Día 3: Prefabs (15 minutos)
1. ✅ Crear carpetas de Prefabs
2. ✅ Convertir obstáculos a Prefabs
3. ✅ Nombrar correctamente

### Día 4: Documentación (10 minutos)
1. ✅ Mover guías a Documentation
2. ✅ Crear README.md
3. ✅ Actualizar referencias

### Día 5: Limpieza (15 minutos)
1. ✅ Eliminar archivos no usados
2. ✅ Renombrar archivos mal nombrados
3. ✅ Verificar que todo funciona

---

## ⚠️ ERRORES COMUNES A EVITAR

### ❌ NO HAGAS:
1. Mover archivos desde Windows Explorer
2. Renombrar clases sin renombrar archivo
3. Crear carpetas con espacios
4. Mezclar scripts de diferentes categorías
5. Borrar archivos .meta

### ✅ SÍ HAZLO:
1. Siempre desde Unity
2. Mantén consistencia en nombres
3. Usa guiones bajos en vez de espacios
4. Mantén cada categoría separada
5. Haz backup antes de reorganizar

---

## 🔍 VERIFICAR QUE TODO FUNCIONA

Después de reorganizar:

1. **Abre cada escena** y verifica que no hay referencias rotas
2. **Presiona Play** y prueba el juego
3. **Revisa la consola** por errores
4. **Verifica prefabs** que no tengan missing scripts
5. **Haz un Build** de prueba

---

## 💾 BACKUP ANTES DE REORGANIZAR

**¡IMPORTANTE! Haz backup primero:**

1. Cierra Unity
2. Copia toda la carpeta del proyecto
3. Pégala en otro lugar como `3djuego_BACKUP`
4. Ahora puedes reorganizar con seguridad

O usa Git:
```bash
git add .
git commit -m "Antes de reorganizar estructura"
```

---

## 📊 ESTRUCTURA FINAL VISUALIZADA

```
Assets/
├── 📜 Scripts/         (32 archivos organizados en 6 categorías)
├── 🎨 Materials/       (Materiales por categoría)
├── 🖼️ Textures/        (Texturas organizadas)
├── 🏗️ Models/          (Modelos 3D)
├── 🎬 Prefabs/         (Prefabs reutilizables)
├── 🎵 Audio/           (Sonidos y música)
├── 🎮 Scenes/          (Niveles del juego)
├── 📚 Documentation/   (Guías y docs)
└── ⚙️ Settings/        (Configuración Unity)
```

---

## 🎯 RESULTADO ESPERADO

### Antes:
```
Assets/
├── personajecontroller.cs
├── goflballcontroller.cs
├── MovingWall.cs
├── ... (30+ archivos mezclados)
```

### Después:
```
Assets/
├── Scripts/
│   ├── Player/ (3 archivos)
│   ├── Obstacles/ (14 archivos)
│   └── ... (organizados lógicamente)
```

---

## 💡 TIPS PROFESIONALES

1. **Usa colores de carpetas** (Unity 2021+)
   - Project Settings → Editor → Folder Icons

2. **Crea carpeta _Project**
   - Para assets del proyecto actual
   - Todo lo demás en carpetas específicas

3. **Usa prefijo de versiones**
   - `Scenes/V1_Nivel_01.unity`
   - Fácil hacer rollback

4. **Documenta cambios**
   - Crea `CHANGELOG.md`
   - Registra cambios importantes

5. **Automatiza con scripts**
   - Crea un script Editor para organizar
   - Un botón para validar estructura

---

¡Sigue esta guía y tendrás un proyecto profesional y fácil de mantener! 🚀

**¿Necesitas ayuda implementando alguna parte específica?**
