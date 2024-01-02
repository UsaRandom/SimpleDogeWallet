using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace DogecoinTerminal.Common
{

	public class Page : IPage
	{
		public delegate void OnEvent(object value);

		public ICollection<IPageControl> Controls { get; private set; } = new List<IPageControl>();

		private IDictionary<string, OnEvent> _controlEvents;

		public IPageOptions Options { get; private set; }

		public Page(IPageOptions options)
		{
			Options = options;

			_controlEvents = new Dictionary<string, OnEvent>();
		}


		public virtual void Update(GameTime gameTime, IServiceProvider services)
		{
			foreach (var control in Controls)
			{
				control.Update(gameTime, services);
			}
		}

		public virtual void Draw(GameTime gameTime, IServiceProvider services)
		{
			foreach (var control in Controls)
			{
				control.Draw(gameTime, services);
			}
		}

		public void Receive(UserClickMessage message)
		{
			foreach (var control in Controls)
			{
				if (control.ContainsPoint(message.ClickLocation))
				{
					_controlEvents[control.Name].Invoke(message);
				}
			}
		}


		public virtual void Cleanup()
		{

		}

		protected T GetControl<T>(string name) where T : IPageControl
		{
			foreach (var control in Controls)
			{
				if (control.Name == name && typeof(T).IsInstanceOfType(control))
				{
					return (T)control;
				}
			}

			return default;
		}

		public void OnClick(string control, OnEvent onClick)
		{
			_controlEvents[control] = onClick;
		}



		/*
		 * Pages are created dynamically, any dependencies are injected.
		 */
		public static T Create<T>(IPageOptions options, IServiceProvider services) where T : IPage
		{
			var constructor = GetConstructorWithMostParameters<T>();

			if (constructor == default)
			{
				return default(T);
			}

			var newPage = (T)constructor.Invoke(BuildConstructorArguments(constructor, options, services));

			var pageDef = typeof(T).GetCustomAttribute<PageDefAttribute>();

			if (pageDef != null && File.Exists(pageDef.FileName))
			{
				var pageEl = XElement.Load(pageDef.FileName);

				foreach(var element in pageEl.Elements())
				{
					var fullControlName = element.Name.NamespaceName + "." + element.Name.LocalName;

					var target = Type.GetType(fullControlName);

					if(target == null)
					{
						Debug.WriteLine($"Unknown control '{fullControlName}' in {pageDef.FileName}");
						continue;
					}

					var controlConstructor = target.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new Type[] { typeof(XElement) });

					var newControl = (IPageControl) controlConstructor.Invoke(new object[] { element });

					newPage.Controls.Add(newControl);
				}

			}

			return newPage;
		}

		private static object?[]? BuildConstructorArguments(ConstructorInfo constructor, IPageOptions options, IServiceProvider services)
		{
			var parameters = new List<object>();

			foreach (var parameter in constructor.GetParameters())
			{
				if (parameter.ParameterType.IsSubclassOf(typeof(IPageOptions)))
				{
					parameters.Add(options);
				}
				else
				{
					var service = services.GetService(parameter.ParameterType);

					parameters.Add(service);
				}
			}

			return parameters.ToArray();
		}

		private static ConstructorInfo GetConstructorWithMostParameters<T>()
		{
			ConstructorInfo targetConstructor = default;
			var constructors = typeof(T).GetConstructors();
			foreach (var currentConstructor in constructors)
			{
				if (targetConstructor == default)
				{
					targetConstructor = currentConstructor;
					continue;
				}

				var previousTargetArgs = targetConstructor.GetParameters();
				var currentConstructorArgs = currentConstructor.GetParameters();

				if (currentConstructorArgs.Length > previousTargetArgs.Length)
				{
					targetConstructor = currentConstructor;
				}
			}

			return targetConstructor;
		}

	}
}
