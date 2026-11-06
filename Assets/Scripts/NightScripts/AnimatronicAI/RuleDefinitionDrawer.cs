using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RuleDefinition), true)]
public class RuleDefinitionDrawer : PropertyDrawer
{
    private const float ToggleWidth = 18f;

    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (!HasExtraFields(property) || !property.isExpanded)
            return height;

        SerializedProperty iterator = property.Copy();
        SerializedProperty end = iterator.GetEndProperty();

        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren) &&
               !SerializedProperty.EqualContents(iterator, end))
        {
            enterChildren = false;

            if (iterator.depth != property.depth + 1 ||
                iterator.name == "Allowed")
                continue;

            height += EditorGUIUtility.standardVerticalSpacing;
            height += EditorGUI.GetPropertyHeight(iterator, true);
        }

        return height;
    }

    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty allowed =
            property.FindPropertyRelative("Allowed");

        if (allowed == null)
        {
            EditorGUI.LabelField(position, label.text,
                "Missing Allowed field");
            EditorGUI.EndProperty();
            return;
        }

        bool hasExtraFields = HasExtraFields(property);

        Rect headerRect = new Rect(
            position.x,
            position.y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );

        if (!hasExtraFields)
        {
            allowed.boolValue = EditorGUI.ToggleLeft(
                headerRect,
                label,
                allowed.boolValue
            );
        }
        else
        {
            Rect foldoutRect = headerRect;
            foldoutRect.width -= ToggleWidth;

            property.isExpanded = EditorGUI.Foldout(
                foldoutRect,
                property.isExpanded,
                label,
                true
            );

            Rect toggleRect = headerRect;
            toggleRect.xMin = toggleRect.xMax - ToggleWidth;

            allowed.boolValue = EditorGUI.Toggle(
                toggleRect,
                allowed.boolValue
            );

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                float y = headerRect.yMax +
                          EditorGUIUtility.standardVerticalSpacing;

                SerializedProperty iterator = property.Copy();
                SerializedProperty end = iterator.GetEndProperty();

                bool enterChildren = true;

                while (iterator.NextVisible(enterChildren) &&
                       !SerializedProperty.EqualContents(iterator, end))
                {
                    enterChildren = false;

                    if (iterator.depth != property.depth + 1 ||
                        iterator.name == "Allowed")
                        continue;

                    float childHeight =
                        EditorGUI.GetPropertyHeight(iterator, true);

                    Rect childRect = new Rect(
                        position.x,
                        y,
                        position.width,
                        childHeight
                    );

                    EditorGUI.PropertyField(
                        childRect,
                        iterator,
                        true
                    );

                    y += childHeight +
                         EditorGUIUtility.standardVerticalSpacing;
                }

                EditorGUI.indentLevel--;
            }
        }

        EditorGUI.EndProperty();
    }

    private bool HasExtraFields(SerializedProperty property)
    {
        SerializedProperty iterator = property.Copy();
        SerializedProperty end = iterator.GetEndProperty();

        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren) &&
               !SerializedProperty.EqualContents(iterator, end))
        {
            enterChildren = false;

            if (iterator.depth == property.depth + 1 &&
                iterator.name != "Allowed")
                return true;
        }

        return false;
    }
}