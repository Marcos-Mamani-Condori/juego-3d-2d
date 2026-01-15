# 🎮 Guía Rápida: Sistema de Desafío de Patrones (Modo Disparo)

## ✅ Sistema Completado

Has creado un sistema de mini-juego de memoria de patrones que se activa al **disparar** a un enemigo especial:

- ✅ **4 rondas acumulativas** (estilo Simon Says)
- ✅ **Números aleatorios 1-4**
- ✅ **Activación por disparo** (no por pelota)
- ✅ **Canvas automático** con fondo borroso
- ✅ **Reinicio total** del juego si fallas

---

## 🚀 Configuración Rápida (3 Pasos)

### **Paso 1: Crear el Canvas Automáticamente**

1. En Unity, crea un **GameObject vacío** en la escena
2. Nómbralo `CanvasBuilder`
3. Añade el componente **`Pattern Challenge Canvas Builder`**
4. En el Inspector, marca el checkbox **`Create Canvas`**
5. ✅ **¡El Canvas se crea automáticamente!**
6. Elimina el GameObject `CanvasBuilder` (ya no lo necesitas)

**Resultado**: Verás un nuevo Canvas llamado `PatternChallengeCanvas` en tu jerarquía.

---

### **Paso 2: Guardar el Prefab de Número**

1. En la jerarquía, busca: `PatternChallengeCanvas` → `MainPanel` → `CenterContainer` → **`NumberPrefab`**
2. Arrastra `NumberPrefab` a la carpeta `Assets/Prefabs/UI/`
3. Vuelve al Canvas y selecciona `PatternChallengeCanvas`
4. En el Inspector, busca el componente **`Pattern Challenge UI`**
5. Arrastra el prefab `NumberPrefab` al campo **`Number Prefab`**

---

### **Paso 3: Configurar el GameManager**

1. **Buscar o crear GameManager**:
   - Si ya tienes un GameObject con `CheckpointSystem`, úsalo
   - Si no, crea uno: `GameObject` → `Create Empty` (nombre: `GameManager`)

2. **Añadir PatternMemoryGame**:
   - Select `GameManager` → Add Component → **`Pattern Memory Game`**
   - **Asignar referencias**:
     - `Shield Barrier Controller`: Tu muro existente
     - `Challenge UI`: El script `PatternChallengeUI` del Canvas
     - `Golf Ball Controller`: Busca tu pelota de golf en la escena y arrastra el GameObject completo aquí (tiene el script `GolfBallController`)

3. **Configuración**:
   - Total Rounds: `4`
   - Pattern Display Time: `4` segundos
   - Min Number: `1`
   - Max Number: `4`

---

## 🎯 Crear el Enemigo Especial

### **Opción A: Duplicar un enemigo existente**

1. Duplica un enemigo que ya tengas
2. Nómbralo `PatternChallengeEnemy`
3. Asegúrate de que tenga el componente **`Enemy Health`**
4. Add Component → **`Pattern Challenge Enemy`**
5. En el Inspector:
   - `Pattern Game`: Arrastra el `GameManager`
   - `Enable Pulse Effect`: ✓ (para que brille)

### **Opción B: Crear desde cero**

1. Crea un GameObject (Cube, Sphere, o tu modelo)
2. Add Component → **`Enemy Health`**
3. Add Component → **`Pattern Challenge Enemy`**
4. Asignar referencias como arriba

### **Hacerlo Visualmente Distintivo** (Recomendado):

1. Crea un **Material dorado/amarillo**:
   - `Assets` → Create → Material (nombre: `SpecialEnemyMaterial`)
   - Color: `#FFD700` (dorado)
   - Emission: ✓ Activado (color amarillo brillante)
2. Arrastra el material al enemigo
3. En `Pattern Challenge Enemy` → `Special Material`: Asigna el material

---

## 🎮 Cómo Funciona

### **En el Juego:**

1. Jugador cambia a **Modo Shooter** (tecla `F`)
2. Apunta al **enemigo especial** (dorado/brillante)
3. Dispara (tecla `Tab`)
4. **Se activa el desafío**:
   - Fondo se oscurece (efecto blur)
   - Aparece el patrón en el centro
   - Controles de golf se desactivan

### **Secuencia de Rondas:**

```
RONDA 1: Muestra [3]           → Presionas 3
RONDA 2: Muestra [3] → [1]     → Presionas 3, 1
RONDA 3: Muestra [3] → [1] → [4] → Presionas 3, 1, 4
RONDA 4: Muestra [3] → [1] → [4] → [2] → Presionas 3, 1, 4, 2
```

### **Resultados:**

- ✅ **Éxito**: Muro baja, juego continúa
- ❌ **Fallo**: Juego se reinicia completamente

---

## ⚙️ Personalización

### **Cambiar dificultad:**

- `PatternMemoryGame` → `Total Rounds`: Más rondas = más difícil
- `PatternMemoryGame` → `Pattern Display Time`: Menos tiempo = más difícil
- `PatternMemoryGame` → `Max Number`: Usar números 1-5 en lugar de 1-4

### **Cambiar colores del UI:**

- `PatternChallengeUI` → `Correct Color`: Verde (por defecto)
- `PatternChallengeUI` → `Incorrect Color`: Rojo (por defecto)
- `PatternChallengeUI` → `Normal Color`: Blanco (por defecto)

### **Permitir múltiples activaciones:**

- `PatternChallengeEnemy` → `Can Activate Multiple Times`: ✓

---

## 🔧 Solución de Problemas

### **El enemigo no activa el desafío al disparar:**
- ✓ Verifica que el enemigo tenga `EnemyHealth` Y `PatternChallengeEnemy`
- ✓ Verifica que `Pattern Game` esté asignado
- ✓ Asegúrate de estar en **Modo Shooter** (tecla F)
- ✓ Revisa la consola por errores

### **El Canvas no aparece:**
- ✓ Verifica que `PatternChallengeUI` esté en el Canvas
- ✓ Verifica que todas las referencias estén asignadas
- ✓ Asegúrate de que `MainPanel` esté desactivado inicialmente

### **Los números no se muestran:**
- ✓ Verifica que `NumberPrefab` esté asignado en `PatternChallengeUI`
- ✓ Asegúrate de que el prefab tenga `TextMeshProUGUI`

### **El juego no se reinicia al fallar:**
- ✓ Verifica que `CheckpointSystem` exista en la escena
- ✓ Revisa la consola por errores

---

## 📝 Archivos Creados

### **Scripts Principales:**
- `PatternMemoryGame.cs` → Lógica del juego
- `PatternChallengeUI.cs` → Controlador de UI
- `PatternChallengeEnemy.cs` → Enemigo especial (adaptado para disparo)
- `PatternChallengeCanvasBuilder.cs` → Constructor automático de Canvas

### **Modificaciones:**
- `PersonajeControlador.cs` → Añadida detección de enemigo especial
- `ShieldBarrierController.cs` → Añadido método `ReleaseBarrierExternal()`

---

## ✨ Características Especiales

### **Efecto de Fondo Borroso:**
- El Canvas tiene un panel negro semi-transparente (85% opacidad)
- Crea un efecto de "blur" visual que enfoca la atención en el patrón
- Todo el fondo se oscurece automáticamente

### **Enemigo Inmortal:**
- El enemigo especial tiene **999,999 de vida**
- No puede ser destruido por disparos normales
- Solo activa el desafío

### **Integración con Sistema Existente:**
- Compatible con tu sistema de disparo Raycast
- No interfiere con enemigos normales
- Usa tu `CheckpointSystem` para reiniciar

---

## 🎉 ¡Listo para Probar!

1. Guarda la escena
2. Dale Play
3. Presiona `F` para cambiar a Modo Shooter
4. Dispara al enemigo especial (dorado)
5. ¡Memoriza y repite los patrones!

**¡Buena suerte!** 🎯
