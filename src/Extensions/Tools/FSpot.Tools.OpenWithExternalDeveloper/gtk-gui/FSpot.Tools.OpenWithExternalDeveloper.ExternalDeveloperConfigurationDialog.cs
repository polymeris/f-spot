
// This file has been generated by the GUI designer. Do not modify.
namespace FSpot.Tools.OpenWithExternalDeveloper
{
	public partial class ExternalDeveloperConfigurationDialog
	{
		private global::Gtk.Table table1;
		private global::Gtk.ComboBox developer;
		private global::Gtk.Label developerLabel;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget FSpot.Tools.OpenWithExternalDeveloper.ExternalDeveloperConfigurationDialog
			this.Name = "FSpot.Tools.OpenWithExternalDeveloper.ExternalDeveloperConfigurationDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("External developer configuration");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child FSpot.Tools.OpenWithExternalDeveloper.ExternalDeveloperConfigurationDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "vBox";
			w1.BorderWidth = ((uint)(2));
			// Container child vBox.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(3)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.developer = global::Gtk.ComboBox.NewText ();
			this.developer.AppendText (global::Mono.Unix.Catalog.GetString ("UFRaw"));
			this.developer.Name = "developer";
			this.developer.Active = 0;
			this.table1.Add (this.developer);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.developer]));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.developerLabel = new global::Gtk.Label ();
			this.developerLabel.Name = "developerLabel";
			this.developerLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Preferred developer");
			this.table1.Add (this.developerLabel);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.developerLabel]));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			w1.Add (this.table1);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(w1 [this.table1]));
			w4.Position = 0;
			// Internal child FSpot.Tools.OpenWithExternalDeveloper.ExternalDeveloperConfigurationDialog.ActionArea
			global::Gtk.HButtonBox w5 = this.ActionArea;
			w5.Name = "actionArea";
			w5.Spacing = 10;
			w5.BorderWidth = ((uint)(5));
			w5.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child actionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w6 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w5 [this.buttonCancel]));
			w6.Expand = false;
			w6.Fill = false;
			// Container child actionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w7 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w5 [this.buttonOk]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 300;
			this.Show ();
			this.buttonOk.Activated += new global::System.EventHandler (this.OnOkClicked);
		}
	}
}
