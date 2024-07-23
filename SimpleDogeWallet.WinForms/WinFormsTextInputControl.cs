﻿using Microsoft.Xna.Framework;
using SimpleDogeWallet.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SimpleDogeWallet.WinForms
{
	public class WinFormsTextInputControl : TextInputControl
	{
		private bool _selected = false;

		private TextBox _textBox;
		

		public WinFormsTextInputControl(XElement element)
			: base(element)
		{

		}
	

		public TextBox TextBoxControl
		{
			get
			{
				if(_textBox == null)
				{
					_textBox = new TextBox();
					_textBox.Font = new Font("Comic Sans MS", 4.5f * TextSize, FontStyle.Bold, GraphicsUnit.Point);
					((TextBoxBase)_textBox).AutoSize = false;
				}
				return _textBox;
			}
		}

		public override string Text
		{
			get
			{
				return TextBoxControl.Text;
			}
			set
			{
				TextBoxControl.Text = value;
			}
		}



		public override void Draw(GameTime time, IServiceProvider services)
		{

		}

		private int lastHeight = -1;

		public override void Update(GameTime time, IServiceProvider services)
		{
			var virtualScreen = services.GetService<VirtualScreen>();
			var screenCordStart = virtualScreen.VirtualCoordToWindowCoord(StartPosition);
			var screenCordEnd = virtualScreen.VirtualCoordToWindowCoord(EndPosition);

			TextBoxControl.Width = Math.Abs(screenCordEnd.X - screenCordStart.X);
			TextBoxControl.Height = Math.Abs(screenCordEnd.Y - screenCordStart.Y);
			
			TextBoxControl.Location = new System.Drawing.Point(Math.Min(screenCordStart.X, screenCordEnd.X), Math.Min(screenCordStart.Y, screenCordEnd.Y));

			if(lastHeight != TextBoxControl.Height)
			{
				TextBoxControl.Font = new Font("Comic Sans MS", TextBoxControl.Height/2 - 2, FontStyle.Bold, GraphicsUnit.Pixel);
			}

			lastHeight = TextBoxControl.Height;
		}

		public override void OnControlHidden(IServiceProvider services)
		{
			var form = services.GetService<Form>();
			if (form.InvokeRequired)
			{
				form.Invoke((MethodInvoker)delegate {

					form.Controls.Remove(TextBoxControl);
				});
			}
			else
			{
				form.Controls.Remove(TextBoxControl);
			}
		}

		public override void OnControlShown(IServiceProvider services)
		{
			var form = services.GetService<Form>();
			if (form.InvokeRequired)
			{
				form.Invoke((MethodInvoker)delegate {

						form.Controls.Add(TextBoxControl);
				});
			}
			else
			{
				form.Controls.Add(TextBoxControl);
			}
		}
	}
}
