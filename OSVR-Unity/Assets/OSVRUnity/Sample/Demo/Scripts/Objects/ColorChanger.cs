/// OSVR-Unity Demo
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2014 Sensics, Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///     http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </copyright>

/// <summary>
/// Author: Bob Berkebile
/// Email: bob@bullyentertainment.com || bobb@pixelplacement.com
/// </summary>

using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour, IColorChanger
{
	#region Public Methods
	public int materialID = 0;
	#endregion

	#region Private Variables
	Material _material;
	Color _targetColor;
	Color _startColor;
	Color _currentColor;
	#endregion

	#region Init
	void Awake()
	{
		_material = GetComponent<Renderer>().materials[materialID];
	}
	#endregion

	#region Event Registration
	void OnEnable()
	{
		ColorManager.Register( this );
	}

	void OnDisable()
	{
		ColorManager.Unregister( this );
	}
	#endregion

	#region Interface Implementation
	public void ChangeColor ( Color newColor )
	{
		_targetColor = newColor;
		StopCoroutine( "ApplyChangeColor" );
		StartCoroutine( "ApplyChangeColor" );
	}
	#endregion

	#region Virtual Methods
	protected virtual void GetColor()
	{
		_startColor = _material.GetColor( "_Tint" );
	}

	protected virtual void SetColor()
	{
		_material.SetColor( "_Tint", _currentColor );
	}
	#endregion

	#region Coroutines
	IEnumerator ApplyChangeColor()
	{
		yield return new WaitForSeconds( Random.Range( 0f, ColorManager.delay ) );

		float startTime = Time.realtimeSinceStartup;

		GetColor();

		while (true)
		{
			float percentage = ( Time.realtimeSinceStartup - startTime ) / ColorManager.duration;

			_currentColor = Color.Lerp( _startColor, _targetColor, percentage );

			SetColor();

			if ( percentage >= 1 )
			{
				_currentColor = _targetColor;
				SetColor();
				yield break;
			}
			yield return null;
		}
	}
	#endregion
}
