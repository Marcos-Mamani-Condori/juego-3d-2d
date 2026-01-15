# 🏁 Configuración del Hoyo Final (Victoria)

## ✅ Ya está listo

El script `GolfHole.cs` está actualizado para mostrar mensaje de victoria automáticamente.

## 🎯 Configuración en Unity (2 opciones)

### **Opción 1: Mensaje + Escena de Victoria (Recomendado)**

1. Selecciona tu hoyo final en la Hierarchy
2. En `Golf Hole` (Inspector):
   - `Win Action`: **Show Victory Screen**
   - `Next Scene Name`: **"VictoryScreen"** (o el nombre que quieras)
   - `Victory Text`: **"¡FELICIDADES!\n¡HAS COMPLETADO EL JUEGO!"**
   - `Delay Before Action`: **3** segundos

3. **Crea la escena de victoria**:
   - File → New Scene
   - Añade lo que quieras (video, créditos, botón de reiniciar)
   - File → Save As → `VictoryScreen`
   - File → Build Settings → Add Open Scenes

### **Opción 2: Solo Mensaje (Sin escena nueva)**

1. Selecciona tu hoyo final
2. En `Golf Hole`:
   - `Win Action`: **Nothing**
   - `Victory Text`: **"¡FELICIDADES!\n¡HAS GANADO!"**

El mensaje aparecerá 3 segundos y el juego se quedará ahí.

## 🎨 Qué hace automáticamente

Cuando la pelota entra al hoyo:
1. ⏸️ Pausa el juego
2. 🎨 Crea un Canvas con fondo oscuro
3. 📝 Muestra el texto amarillo grande en el centro
4. ⏱️ Espera 3 segundos
5. 🎬 Carga la escena de victoria (si configuraste Opción 1)

## 🎥 Si quieres un video de victoria

Usa el script `ControladorVideoConSalto.cs` que ya tienes en la escena de victoria.

**¡Listo!** No necesitas hacer nada más. 🎉
