/// <summary>
/// Author: Bob Berkebile
/// Email: bob@bullyentertainment.com || bobb@pixelplacement.com
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IColorChanger
{
	void ChangeColor( Color newColor );
}

[System.Serializable]
public class ColorItem
{
	public string name;
	public Color color;

	public ColorItem( string name, Color color )
	{
		this.name = name;
		this.color = color;
	}
}

public class ColorManager : MonoBehaviour
{
	void OnGUI()
	{
		if ( GUILayout.Button( "Color Up" ) ) 
		{
			ChangeColorNext();
		}
		if ( GUILayout.Button( "Color Down" ) ) 
		{
			ChangeColorPrevious();
		}
	}

	#region Public Variables
	public ColorItem[] colors;
	public float changeDuration;
	public float changeDelayRandomMax = .25f;
	public static float delay;
	public static float duration;
	#endregion

	#region Private Variables
	float _item;
	static List<IColorChanger> _colorChangers = new List<IColorChanger>();
	#endregion

	#region Loop
	void Update()
	{
		//avoiding a singleton by forcing static variable updates (lame, I know... but there are only so many hours in a deadline):
		delay = changeDelayRandomMax;
		duration = changeDuration;
	}
	#endregion

	#region Public Methods
	public static void Register( IColorChanger colorChanger )
	{
		_colorChangers.Add( colorChanger );
	}

	public static void Unregister( IColorChanger colorChanger )
	{
		_colorChangers.Remove( colorChanger );
	}

	public void ChangeColor( int index )
	{
		if ( index > colors.Length - 1 || index < 0 ) 
		{
			Debug.LogError( "Color value out of range." );
			return;
		}

		ApplyColor( colors[ index ].color ); 
	}

	public void ChangeColor( string name )
	{
		ColorItem found = null;

		foreach( ColorItem item in colors )
		{
			if ( item.name.ToLower() == name.ToLower() )
			{
				found = item;
				break;
			}
		}

		if ( found != null ) 
		{
			ApplyColor( found.color );
		}else{
			Debug.LogError( "Color not found." );
		}
	}

	public void ChangeColorNext()
	{
		_item = Mathf.Repeat( ++_item, colors.Length - 1 );
		ApplyColor( colors[ (int)_item ].color );
	}

	public void ChangeColorPrevious()
	{
		_item = Mathf.Repeat( --_item, colors.Length - 1 );
		ApplyColor( colors[ (int)_item ].color );
	}
	#endregion

	#region Private Methods
	void ApplyColor( Color color )
	{
		foreach( IColorChanger item in _colorChangers ) 
		{
			item.ChangeColor( color );
		}
	}
	#endregion
}
