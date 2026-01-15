using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Script de Editor que organiza automáticamente el proyecto
/// SIN romper referencias.
/// 
/// USO:
/// 1. Unity debe estar abierto
/// 2. Ve a menú: Tools > Organizar Proyecto
/// 3. Confirma la acción
/// 4. Unity moverá todo automáticamente
/// </summary>
public class ProjectOrganizer : EditorWindow
{
    private bool confirmOrganization = false;

    [MenuItem("Tools/Organizar Proyecto Automáticamente")]
    public static void ShowWindow()
    {
        GetWindow<ProjectOrganizer>("Organizador de Proyecto");
    }

    void OnGUI()
    {
        GUILayout.Label("Organizador Automático de Proyecto", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Este script organizará tu proyecto en carpetas categorizadas:\n\n" +
            "• Scripts/Player/\n" +
            "• Scripts/Obstacles/\n" +
            "• Scripts/Enemies/\n" +
            "• Scripts/Systems/\n" +
            "• Scripts/Animations/\n" +
            "• Scripts/UI/\n" +
            "• Documentation/\n\n" +
            "TODAS las referencias se mantendrán intactas.",
            MessageType.Info
        );

        GUILayout.Space(10);

        confirmOrganization = EditorGUILayout.Toggle("Confirmo que quiero organizar", confirmOrganization);

        GUILayout.Space(10);

        GUI.enabled = confirmOrganization;
        if (GUILayout.Button("ORGANIZAR PROYECTO", GUILayout.Height(40)))
        {
            OrganizeProject();
        }
        GUI.enabled = true;

        GUILayout.Space(10);

        if (GUILayout.Button("Cancelar", GUILayout.Height(30)))
        {
            Close();
        }
    }

    private void OrganizeProject()
    {
        if (!EditorUtility.DisplayDialog(
            "Confirmar Organización",
            "¿Estás seguro de que quieres organizar el proyecto?\n\n" +
            "Esto moverá archivos a carpetas organizadas.\n" +
            "Las referencias se mantendrán intactas.",
            "Sí, Organizar",
            "Cancelar"))
        {
            return;
        }

        Debug.Log("========================================");
        Debug.Log("INICIANDO ORGANIZACIÓN DEL PROYECTO");
        Debug.Log("========================================");

        AssetDatabase.StartAssetEditing();

        try
        {
            // Crear estructura de carpetas
            CreateFolderStructure();

            // Mover scripts
            MovePlayerScripts();
            MoveObstacleScripts();
            MoveEnemyScripts();
            MoveSystemScripts();
            MoveAnimationScripts();
            MoveUIScripts();
            MoveObsoleteScripts();
            
            // Mover documentación
            MoveDocumentation();

            Debug.Log("========================================");
            Debug.Log("✅ ORGANIZACIÓN COMPLETADA CON ÉXITO");
            Debug.Log("========================================");

            EditorUtility.DisplayDialog(
                "Organización Completa",
                "El proyecto ha sido organizado exitosamente.\n\n" +
                "Todas las referencias se mantienen intactas.\n\n" +
                "Revisa la consola para más detalles.",
                "OK"
            );
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }

        Close();
    }

    private void CreateFolderStructure()
    {
        Debug.Log("📁 Creando estructura de carpetas...");

        CreateFolder("Assets/Scripts");
        CreateFolder("Assets/Scripts/Player");
        CreateFolder("Assets/Scripts/Obstacles");
        CreateFolder("Assets/Scripts/Enemies");
        CreateFolder("Assets/Scripts/Systems");
        CreateFolder("Assets/Scripts/Animations");
        CreateFolder("Assets/Scripts/UI");
        CreateFolder("Assets/Scripts/_Obsolete");
        CreateFolder("Assets/Documentation");
    }

    private void CreateFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parentFolder = Path.GetDirectoryName(path).Replace("\\", "/");
            string folderName = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parentFolder, folderName);
            Debug.Log($"  Creada: {path}");
        }
    }

    private void MovePlayerScripts()
    {
        Debug.Log("👤 Moviendo scripts del jugador...");
        
        MoveAsset("Assets/personajecontroller.cs", "Assets/Scripts/Player/personajecontroller.cs");
        MoveAsset("Assets/goflballcontroller.cs", "Assets/Scripts/Player/goflballcontroller.cs");
        MoveAsset("Assets/camerafollow.cs", "Assets/Scripts/Player/camerafollow.cs");
    }

    private void MoveObstacleScripts()
    {
        Debug.Log("🏗️  Moviendo scripts de obstáculos...");

        string[] obstacles = new string[]
        {
            "MovingWall.cs", "WindZone.cs", "RotatingPlatform.cs", "LaserBarrier.cs",
            "JumpPad.cs", "Pendulum.cs", "DisappearingPlatform.cs", "TeleportPortal.cs",
            "ConveyorBelt.cs", "PressureButton.cs", "SpikeTrap.cs", "GravityZone.cs",
            "IcePlatform.cs", "BlackHole.cs"
        };

        foreach (string script in obstacles)
        {
            MoveAsset($"Assets/{script}", $"Assets/Scripts/Obstacles/{script}");
        }
    }

    private void MoveEnemyScripts()
    {
        Debug.Log("👾 Moviendo scripts de enemigos...");

        string[] enemies = new string[]
        {
            "EnemyHealth.cs", "EnemyShipMover.cs", "EnemyShipTarget.cs",
            "ShieldBarrierController.cs", "ShieldModuleScript.cs"
        };

        foreach (string script in enemies)
        {
            MoveAsset($"Assets/{script}", $"Assets/Scripts/Enemies/{script}");
        }
    }

    private void MoveSystemScripts()
    {
        Debug.Log("⚙️  Moviendo scripts de sistemas...");

        string[] systems = new string[]
        {
            "CheckpointSystem.cs", "Checkpoint.cs", "FreeCameraController.cs",
            "gamepuntaje.cs", "cambiadorEscena.cs"
        };

        foreach (string script in systems)
        {
            MoveAsset($"Assets/{script}", $"Assets/Scripts/Systems/{script}");
        }
    }

    private void MoveAnimationScripts()
    {
        Debug.Log("🎬 Moviendo scripts de animaciones...");

        MoveAsset("Assets/CharacterAnimationHelper.cs", "Assets/Scripts/Animations/CharacterAnimationHelper.cs");
        MoveAsset("Assets/SaltarAnimacion.cs", "Assets/Scripts/Animations/SaltarAnimacion.cs");
    }

    private void MoveUIScripts()
    {
        Debug.Log("🖼️  Moviendo scripts de UI...");

        MoveAsset("Assets/ControladorVideoConSalto.cs", "Assets/Scripts/UI/ControladorVideoConSalto.cs");
        MoveAsset("Assets/LogicaHuecoAvanzada.cs", "Assets/Scripts/UI/LogicaHuecoAvanzada.cs");
    }

    private void MoveObsoleteScripts()
    {
        Debug.Log("🗑️  Moviendo scripts obsoletos...");

        MoveAsset("Assets/MovingBarrier.cs", "Assets/Scripts/_Obsolete/MovingBarrier.cs");
    }

    private void MoveDocumentation()
    {
        Debug.Log("📚 Moviendo documentación...");

        MoveAsset("Assets/GUIA_OBSTACULOS.md", "Assets/Documentation/GUIA_OBSTACULOS.md");
        MoveAsset("Assets/GUIA_ORGANIZACION.md", "Assets/Documentation/GUIA_ORGANIZACION.md");
        MoveAsset("Assets/Readme.asset", "Assets/Documentation/Readme.asset");
    }

    private void MoveAsset(string oldPath, string newPath)
    {
        // Usar AssetDatabase para verificar si el asset existe
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(oldPath);
        
        if (asset != null)
        {
            string error = AssetDatabase.MoveAsset(oldPath, newPath);
            
            if (string.IsNullOrEmpty(error))
            {
                Debug.Log($"  ✅ Movido: {Path.GetFileName(oldPath)}");
            }
            else
            {
                Debug.LogWarning($"  ⚠️  Error moviendo {Path.GetFileName(oldPath)}: {error}");
            }
        }
        else
        {
            // Verificar si ya existe en el destino
            Object destAsset = AssetDatabase.LoadAssetAtPath<Object>(newPath);
            if (destAsset != null)
            {
                Debug.Log($"  ℹ️  Ya existe en destino: {Path.GetFileName(newPath)}");
            }
            else
            {
                Debug.LogWarning($"  ⚠️  No se encontró: {oldPath}");
            }
        }
    }
}
