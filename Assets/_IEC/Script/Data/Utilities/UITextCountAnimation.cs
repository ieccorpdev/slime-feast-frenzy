using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace IEC
{
	public class UITextCountAnimation : _BaseBehaviour
	{
		#region Properties
		
		private enum TextFormat
		{
			Default,
			Prefix,
			Suffix,
			PrefixAndSuffix
		}
		
		public UnityEvent OnTweenComplete;
		
		[SerializeField] private TextFormat textFormat;
		[SerializeField] private TextMeshProUGUI m_TextMeshPro;

		[SerializeField, ShowIf("Prefix")] private string prefix;
		[SerializeField, ShowIf("Suffix")] private string suffix;
		
		private static readonly Dictionary<(int, TextFormat), string> CachedTexts = new Dictionary<(int, TextFormat), string>();
		private Tween m_Tween;
		
		private int m_CurrentValue;
		private int m_TargetValue;
		
		private void OnValidate()
		{
			if (m_TextMeshPro == null)
				m_TextMeshPro = GetComponent<TextMeshProUGUI>();
		}
		
		private bool Prefix => textFormat == TextFormat.Prefix || textFormat == TextFormat.PrefixAndSuffix;
		private bool Suffix => textFormat == TextFormat.Suffix || textFormat == TextFormat.PrefixAndSuffix;

		#endregion

		#region Set Text

		private string GetCachedText(int number, TextFormat format)
		{
			if (CachedTexts.TryGetValue((number, format), out var cachedText))
			{
				return cachedText;
			}
			
			var newText = textFormat switch
			{
				TextFormat.Default => $"{number}",
				TextFormat.Prefix => $"{prefix}{number}",
				TextFormat.Suffix => $"{number}{suffix}",
				TextFormat.PrefixAndSuffix => $"{prefix}{number}{suffix}",
				_ => $"{number}"
			};
			
			CachedTexts.Add((number, format), newText);
			return newText;
		}
		
		public void SetText(int number)
		{
			m_CurrentValue = number;
			m_TextMeshPro.text = GetCachedText(number, textFormat);
		}
		
		public void SetText(string text) => m_TextMeshPro.text = text;
		
		public void SetTargetValue(int newTarget, bool force = false, float duration = 1f)
		{
			m_TargetValue = newTarget;
			
			m_Tween?.Kill();
			if (force)
			{
				m_CurrentValue = m_TargetValue;
				SetText(m_TargetValue);
			}
			else
			{
				m_Tween = DOVirtual.Float(m_CurrentValue, m_TargetValue, duration, t => { SetText(Mathf.RoundToInt(t)); })
					.OnComplete(OnTweenComplete.Invoke);
			}
		}

		#endregion
	}
}

