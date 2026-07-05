using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractableOptions))]
public class InteractableOptionsEditor : Editor
{
    private SerializedProperty interactionTypesProp;
    private SerializedProperty onEventProp;
    private SerializedProperty endEventProp;
    private SerializedProperty destroyProp;

    private void OnEnable()
    {
        if (serializedObject == null) return;

        interactionTypesProp = serializedObject.FindProperty("interactionTypes");
        onEventProp = serializedObject.FindProperty("onInteract");
        endEventProp = serializedObject.FindProperty("endInteract");
        destroyProp = serializedObject.FindProperty("destroyOnSelect");
    }

    public override void OnInspectorGUI()
    {
        if (serializedObject == null) return;

        serializedObject.Update();

        if (interactionTypesProp == null)
        {
            EditorGUILayout.HelpBox("No se encontró la propiedad 'interactionTypes'.", MessageType.Warning);
            return;
        }

        EditorGUILayout.PropertyField(interactionTypesProp, new GUIContent("Tipos de interacción"));

        var interactionValue = (InteractionType)interactionTypesProp.intValue;

        // Mostrar eventos si InvokeEvent está activo
        if (interactionValue.HasFlag(InteractionType.InvokeEvent))
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Eventos", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(onEventProp, new GUIContent("Evento inicial"));
            EditorGUILayout.PropertyField(endEventProp, new GUIContent("Evento final"));
        }

        SerializedProperty justOneInteraction = serializedObject.FindProperty("justOneInteraction");
        if (justOneInteraction != null)
            EditorGUILayout.PropertyField(justOneInteraction, new GUIContent("Solo una interacción"));

        if (destroyProp != null)
            EditorGUILayout.PropertyField(destroyProp, new GUIContent("Destruir al seleccionar"));

        serializedObject.ApplyModifiedProperties();

        // Feedbacks informativos en el Inspector
        if (interactionValue.HasFlag(InteractionType.InvokeEvent))
        {
            EditorGUILayout.HelpBox("La opción 'InvokeEvent' requiere que se asignen eventos para interacción inicial y final.", MessageType.Info);
        }

        if (interactionValue.HasFlag(InteractionType.GrabAndDrop))
        {
            EditorGUILayout.HelpBox("La opción 'GrabAndDrop' interactuarà directamente con el PlayerController para agarrar/soltar este objeto de forma nativa.", MessageType.Info);
        }
    }
}