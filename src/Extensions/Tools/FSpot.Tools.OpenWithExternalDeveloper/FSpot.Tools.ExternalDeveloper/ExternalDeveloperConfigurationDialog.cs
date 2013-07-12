//  ExternalDeveloperConfigurationDialog.cs
//
//  Author:
//       Camilo Polymeris <cpolymeris@gmail.com>
//
//  Copyright (c) 2013 Camilo Polymeris
//
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
//
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
using System;

namespace FSpot.Tools.ExternalDeveloper
{
	public partial class ExternalDeveloperConfigurationDialog : Gtk.Dialog
	{
		/// <summary>
		/// Build the configuration dialog and populate the "Developers" combobox.
		/// </summary>
		public ExternalDeveloperConfigurationDialog()
		{
			this.Build();

			foreach (string name in ExternalDeveloperFactory.GetAvaliableDevelopers())
			{
				if (name != "F-Spot") //<duh
					developer.AppendText(name);
			}

			developer.Active = 0;

			developer.Changed += OnDeveloperChanged;
			buttonOk.Clicked += OnOkClicked;
			buttonCancel.Clicked += OnCancelClicked;
		}

		protected void OnOkClicked(object sender, EventArgs e)
		{
			ExternalDeveloperFactory.SetPreferredDeveloper(developer.ActiveText);
			this.Hide();
		}

		protected void OnCancelClicked(object sender, EventArgs e)
		{
			this.Hide();
		}

		void OnDeveloperChanged(object sender, EventArgs e)
		{
			if (developer.ActiveText == "Rawstudio" && Rawstudio.SupportedVersionIsInstalled())
			{
				warning.LabelProp = String.Format(
					"Only Rawstudio versions 2.1 and up are supported by this extension.",
					developer.ActiveText);
				warningBox.Show();
			}
			else if (ExternalDeveloperFactory.IsSupported(developer.ActiveText))
				warningBox.Hide();
			else
			{
				warning.LabelProp = String.Format(
					"The program <i>{0}</i> is not supported by this extension.",
					developer.ActiveText);
				warningBox.Show();
			}

			quality.Sensitive =
				ExternalDeveloperFactory.CanSetJpegQualityForDeveloper(developer.ActiveText);
		}
	}
}

