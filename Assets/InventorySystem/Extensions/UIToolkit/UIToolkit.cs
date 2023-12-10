using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace InventorySystem.Extensions.UIToolkit
{
	public static class UIToolkit
	{
		public const string HORIZONTAL = "horizontal-layout";
		public const string VERTICAL = "vertical-layout";
		public const string SPACE_AROUND = "space-around";
		public const string SPACE_BETWEEN = "space-between";
		public const string CENTER = "center";
		public const string LEFT_ALIGNED = "left-aligned";
		public const string MIDDLE_ALIGNED = "middle-aligned";
		public const string RIGHT_ALIGNED = "right-aligned";
		public static VisualElement Create(IEnumerable<string> classNames = null, VisualElement parent = null) => Create<VisualElement>(classNames, parent);
		public static Label Create(string label, IEnumerable<string> classNames = null, VisualElement parent = null)
		{
			var element = Create<Label>(classNames, parent);
			element.text = label;
			return element;
		}
		public static Image Create(Sprite sprite, IEnumerable<string> classNames = null, VisualElement parent = null)
		{
			var element = Create<Image>(classNames, parent);
			element.sprite = sprite;
			return element;
		}
		public static Button Create(string label, Action callback, IEnumerable<string> classNames = null, VisualElement parent = null)
		{
			var element = Create<Button>(classNames, parent);
			element.text = label;
			element.clicked += callback;
			return element;
		}
		public static Toggle Create(bool defaultValue, Action<bool> callback, IEnumerable<string> classNames = null, VisualElement parent = null)
		{
			var element = Create<Toggle>(new string[] { }, parent);
			element.RegisterValueChangedCallback(changeEvent => callback.Invoke(changeEvent.newValue));
			element.value = defaultValue;
			return element;
		}
		public static Slider Create(float defaultValue, Action<float> callback, IEnumerable<string> classNames = null, VisualElement parent = null)
		{
			var element = Create<Slider>(classNames, parent);
			element.RegisterValueChangedCallback(changeEvent => callback.Invoke(changeEvent.newValue));
			element.lowValue = 0f;
			element.highValue = 1f;
			element.value = defaultValue;
			return element;
		}
		public static EnumField Create(Enum defaultValue, Action<Enum> callback, IEnumerable<string> classNames = null, VisualElement parent = null)
		{
			var element = Create<EnumField>(classNames, parent);
			element.RegisterValueChangedCallback(changeEvent => callback.Invoke((Enum)changeEvent.newValue));
			element.Init(defaultValue);
			return element;
		}
		public static T Create<T>(IEnumerable<string> classNames = null, VisualElement parent = null) where T : VisualElement, new()
		{
			var element = new T();
			classNames?.ToList().ForEach(element.AddToClassList);
			parent?.Add(element);
			return element;
		}
	}
}