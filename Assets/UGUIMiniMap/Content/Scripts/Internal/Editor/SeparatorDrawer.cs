using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(SeparatorAttribute))]
public class SeparatorDrawer:DecoratorDrawer
{
	SeparatorAttribute separatorAttribute { get { return ((SeparatorAttribute)attribute); } }


	public override void OnGUI(Rect _position)
	{
		if(separatorAttribute.title == "")
		{
			_position.height = 1;
			_position.y += 19;
			GUI.Box(_position, "");
		} else
		{
			Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(separatorAttribute.title));
            textSize = textSize * 2;
			float separatorWidth = (_position.width - textSize.x) / 2.0f - 5.0f;
			_position.y += 19;

			GUI.Box(new Rect(_position.xMin, _position.yMin, separatorWidth, 3), "");
			GUI.Box(new Rect(_position.xMin + separatorWidth + 5.0f, _position.yMin - 8.0f, textSize.x, 20), separatorAttribute.title);
			GUI.Box(new Rect(_position.xMin + separatorWidth + 10.0f + textSize.x, _position.yMin, separatorWidth, 3), "");
		}
	}

	public override float GetHeight()
	{
		return 41.0f;
	}
}