using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CursoRpa {
    public class Solicitud {
        public string Id { get; set; }
        public string Solicitante { get; set; }
        public string Area { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public string Folio { get; set; }
        public string FechaUtc { get; set; }
    }

    public class Registro {
        public static readonly string[] Areas = { "Administración", "Docencia", "Biblioteca" };
        public static readonly string[] Tipos = { "Acceso", "Equipo", "Mantenimiento" };
        public List<Solicitud> Datos { get; private set; }
        readonly string archivo;
        public Registro(string carpeta) {
            Directory.CreateDirectory(carpeta);
            archivo = Path.Combine(carpeta, "solicitudes.xml");
            Datos = new List<Solicitud>();
            if (File.Exists(archivo)) {
                using (var s = File.OpenRead(archivo))
                    Datos = (List<Solicitud>)new XmlSerializer(typeof(List<Solicitud>)).Deserialize(s);
            }
        }
        public static string Validar(Solicitud s) {
            if (!Regex.IsMatch(s.Id ?? "", @"^SOL-\d{3}$")) return "El identificador debe tener el formato SOL-001.";
            if (String.IsNullOrWhiteSpace(s.Solicitante) || s.Solicitante.Trim().Length < 3) return "Escriba un solicitante de al menos 3 caracteres.";
            if (!Areas.Contains(s.Area)) return "Seleccione un área válida.";
            if (!Tipos.Contains(s.Tipo)) return "Seleccione un tipo válido.";
            if (String.IsNullOrWhiteSpace(s.Descripcion) || s.Descripcion.Trim().Length < 10) return "La descripción debe tener al menos 10 caracteres.";
            if (s.Solicitante.Length > 80 || s.Descripcion.Length > 240) return "El solicitante admite 80 caracteres y la descripción 240.";
            return null;
        }
        public Solicitud Buscar(string id) { return Datos.FirstOrDefault(x => x.Id == id); }
        public Solicitud Guardar(Solicitud s) {
            string error = Validar(s);
            if (error != null) throw new ArgumentException(error);
            if (Buscar(s.Id) != null) throw new InvalidOperationException("La solicitud ya está registrada.");
            s.Folio = "FOL-" + (Datos.Count + 1).ToString("0000");
            s.FechaUtc = DateTime.UtcNow.ToString("o");
            var nuevos = new List<Solicitud>(Datos); nuevos.Add(s);
            string temporal = archivo + ".tmp";
            try {
                using (var f = new FileStream(temporal, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    new XmlSerializer(typeof(List<Solicitud>)).Serialize(f, nuevos);
                    f.Flush(true);
                }
                if (File.Exists(archivo)) File.Replace(temporal, archivo, null);
                else File.Move(temporal, archivo);
                Datos = nuevos;
            } finally { if (File.Exists(temporal)) File.Delete(temporal); }
            return s;
        }
    }

    public class Ventana : Form {
        readonly Registro registro;
        TextBox id, solicitante, descripcion, folio;
        ComboBox area, tipo, variante;
        Button registrar, limpiar;
        Label estado, total;
        TableLayoutPanel campos;
        System.Windows.Forms.Timer demora;
        bool ocupado;

        static T Identificar<T>(T c, string nombre, string accesible) where T : Control {
            c.Name = nombre; c.AccessibleName = accesible; return c;
        }
        static Label Etiqueta(string texto) { return new Label { Text = texto, AutoSize = true, Margin = new Padding(0, 10, 12, 0) }; }
        static Button Boton(string texto, string nombre) {
            return Identificar(new Button { Text = texto, AutoSize = true, MinimumSize = new Size(140, 38), Margin = new Padding(0, 0, 12, 0) }, nombre, texto);
        }
        public Ventana(Registro r) {
            registro = r;
            Name = "frmSolicitudes"; AccessibleName = "Registro de solicitudes";
            Text = "Solicitudes RPA | Laboratorio de escritorio";
            Font = new Font("Segoe UI", 10); BackColor = Color.FromArgb(245,247,250);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(800, 680); MinimumSize = new Size(780, 710);
            StartPosition = FormStartPosition.CenterScreen;
            var basePanel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(28), ColumnCount = 1, RowCount = 7 };
            basePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            basePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            basePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            basePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            basePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            basePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            basePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            Controls.Add(basePanel);
            basePanel.Controls.Add(new Label { Text = "Registro de solicitudes", Font = new Font("Segoe UI", 23, FontStyle.Bold), ForeColor = Color.FromArgb(23,46,67), AutoSize = true },0,0);
            basePanel.Controls.Add(Etiqueta("Sistema administrativo de práctica · Solo datos ficticios"),0,1);
            var barra = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            barra.Controls.Add(Etiqueta("Escenario de prueba"));
            variante = Identificar(new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 280, Margin = new Padding(0,6,0,0) }, "cmbEscenario", "Escenario de prueba");
            variante.Items.AddRange(new object[] { "Base", "Ventana desplazada", "Formulario reorganizado", "Respuesta lenta (4 segundos)" });
            variante.SelectedIndex = 0; barra.Controls.Add(variante); basePanel.Controls.Add(barra,0,2);
            campos = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, BackColor = Color.White, Padding = new Padding(16) };
            campos.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,145)); campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
            for(int i=0;i<4;i++) campos.RowStyles.Add(new RowStyle(SizeType.Absolute,44));
            campos.RowStyles.Add(new RowStyle(SizeType.Percent,100));
            id = Identificar(new TextBox { Dock = DockStyle.Top, MaxLength = 7 }, "txtIdSolicitud", "Identificador de solicitud");
            solicitante = Identificar(new TextBox { Dock = DockStyle.Top, MaxLength = 80 }, "txtSolicitante", "Solicitante");
            area = Identificar(new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList }, "cmbArea", "Área"); area.Items.AddRange(Registro.Areas);
            tipo = Identificar(new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList }, "cmbTipo", "Tipo de solicitud"); tipo.Items.AddRange(Registro.Tipos);
            descripcion = Identificar(new TextBox { Dock = DockStyle.Fill, Multiline = true, MaxLength = 240, ScrollBars = ScrollBars.Vertical }, "txtDescripcion", "Descripción");
            campos.Controls.Add(Etiqueta("Identificador"),0,0); campos.Controls.Add(id,1,0);
            campos.Controls.Add(Etiqueta("Solicitante"),0,1); campos.Controls.Add(solicitante,1,1);
            campos.Controls.Add(Etiqueta("Área"),0,2); campos.Controls.Add(area,1,2);
            campos.Controls.Add(Etiqueta("Tipo"),0,3); campos.Controls.Add(tipo,1,3);
            campos.Controls.Add(Etiqueta("Descripción"),0,4); campos.Controls.Add(descripcion,1,4);
            basePanel.Controls.Add(campos,0,3);
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(0,10,0,0) };
            registrar = Boton("Registrar solicitud", "btnRegistrar"); registrar.BackColor = Color.FromArgb(23,76,105); registrar.ForeColor = Color.White; registrar.FlatStyle = FlatStyle.Flat;
            limpiar = Boton("Nueva solicitud", "btnNueva");
            acciones.Controls.Add(registrar); acciones.Controls.Add(limpiar); basePanel.Controls.Add(acciones,0,4);
            var salida = new FlowLayoutPanel { Dock = DockStyle.Fill };
            salida.Controls.Add(Etiqueta("Último folio"));
            folio = Identificar(new TextBox { ReadOnly = true, Width = 130, Margin = new Padding(0,7,16,0) }, "txtFolio", "Último folio"); salida.Controls.Add(folio);
            estado = Identificar(new Label { Text = "Listo para capturar", AutoSize = false, Width = 420, Height = 55, Padding = new Padding(0,9,0,0) }, "lblEstado", "Estado del registro"); salida.Controls.Add(estado); basePanel.Controls.Add(salida,0,5);
            total = Identificar(new Label { Dock = DockStyle.Fill, Text = "", ForeColor = Color.DimGray }, "lblTotal", "Total de solicitudes"); basePanel.Controls.Add(total,0,6); ActualizarTotal();
            registrar.Click += Registrar;
            limpiar.Click += delegate { id.Clear(); solicitante.Clear(); area.SelectedIndex = -1; tipo.SelectedIndex = -1; descripcion.Clear(); folio.Clear(); estado.Text = "Listo para capturar"; id.Focus(); };
            variante.SelectedIndexChanged += delegate { CambiarEscenario(); };
            FormClosing += delegate(object sender, FormClosingEventArgs e) { if(ocupado) e.Cancel = true; };
        }
        void ActualizarTotal() { total.Text = "Solicitudes guardadas: " + registro.Datos.Count + "   ·   Almacenamiento local"; }
        void CambiarEscenario() {
            if(variante.SelectedIndex == 1) { StartPosition = FormStartPosition.Manual; Location = new Point(Screen.PrimaryScreen.WorkingArea.Left+15,Screen.PrimaryScreen.WorkingArea.Top+15); }
            bool cambiado = variante.SelectedIndex == 2;
            foreach(Control c in campos.Controls) {
                if(c == id || (c is Label && c.Text == "Identificador")) campos.SetRow(c,cambiado ? 1 : 0);
                if(c == solicitante || (c is Label && c.Text == "Solicitante")) campos.SetRow(c,cambiado ? 0 : 1);
                if(c == area || (c is Label && c.Text == "Área")) campos.SetRow(c,cambiado ? 3 : 2);
                if(c == tipo || (c is Label && c.Text == "Tipo")) campos.SetRow(c,cambiado ? 2 : 3);
            }
        }
        Solicitud Leer() { return new Solicitud { Id = id.Text.Trim(), Solicitante = solicitante.Text.Trim(), Area = area.Text, Tipo = tipo.Text, Descripcion = descripcion.Text.Trim() }; }
        void Registrar(object sender, EventArgs e) {
            if(ocupado) return;
            folio.Clear();
            Solicitud s = Leer(); string error = Registro.Validar(s);
            if(error != null) { estado.Text = "Datos inválidos"; Dialogo("Datos inválidos",error,"Aceptar",null); return; }
            var existente = registro.Buscar(s.Id);
            if(existente != null) { estado.Text = "Duplicada: " + s.Id; Dialogo("Solicitud duplicada","Ya existe " + s.Id + ". No se creó otro registro.","Aceptar",existente.Folio); return; }
            if(Dialogo("Confirmar registro", "Registrar " + s.Id + " para " + s.Solicitante + " en " + s.Area + ".", "Confirmar", null, true) != DialogResult.OK) { estado.Text = "Registro cancelado"; return; }
            ocupado = true; campos.Enabled = false; registrar.Enabled = false; limpiar.Enabled = false; variante.Enabled = false;
            estado.Text = "Procesando solicitud…";
            demora = new System.Windows.Forms.Timer { Interval = variante.SelectedIndex == 3 ? 4000 : 250 };
            demora.Tick += delegate {
                demora.Stop(); demora.Dispose();
                try {
                    var guardado = registro.Guardar(s); folio.Text = guardado.Folio; estado.Text = "Registrada: " + s.Id; ActualizarTotal();
                    Dialogo("Registro completado", "Solicitud " + s.Id + " guardada correctamente.", "Aceptar", guardado.Folio);
                } catch(Exception) { estado.Text = "Error de almacenamiento"; Dialogo("Error de almacenamiento","No se pudo completar el guardado. Revise permisos y espacio disponible antes de repetir.","Aceptar",null); }
                finally { ocupado = false; campos.Enabled = true; registrar.Enabled = true; limpiar.Enabled = true; variante.Enabled = true; }
            };
            demora.Start();
        }
        DialogResult Dialogo(string titulo, string mensaje, string aceptar, string numero, bool cancelar = false) {
            using(var d = new Form { Text = titulo, Name = "dlg" + (cancelar ? "Confirmacion" : "Resultado"), AccessibleName = titulo, Font = Font, StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, ClientSize = new Size(530,230), MaximizeBox = false, MinimizeBox = false }) {
                d.Controls.Add(Identificar(new Label { Text = mensaje, Location = new Point(24,24), Size = new Size(480,65) }, "lblMensaje", "Mensaje del diálogo"));
                if(numero != null) d.Controls.Add(Identificar(new TextBox { Text = numero, ReadOnly = true, Location = new Point(24,100), Width = 220 }, "txtFolioDialogo", "Folio generado"));
                var ok = Boton(aceptar,"btnAceptarDialogo"); ok.Location = new Point(24,170); ok.DialogResult = DialogResult.OK; d.Controls.Add(ok); d.AcceptButton = ok;
                if(cancelar) { var no = Boton("Cancelar","btnCancelarDialogo"); no.Location = new Point(200,170); no.DialogResult = DialogResult.Cancel; d.Controls.Add(no); d.CancelButton = no; }
                return d.ShowDialog(this);
            }
        }
    }

    static class Program {
        [STAThread] static int Main(string[] args) {
            Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false);
            string carpeta = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..","runtime"));
            if(args.Length == 2 && args[0] == "--data-dir") carpeta = Path.GetFullPath(args[1]);
            string mutexName = "Local\\CursoRpaSolicitudes_" + Convert.ToBase64String(Encoding.UTF8.GetBytes(carpeta.ToUpperInvariant())).Replace('/','_');
            bool nuevo;
            using(var mutex = new Mutex(true,mutexName,out nuevo)) {
                if(!nuevo) { MessageBox.Show("Ya hay una instancia usando esta carpeta de datos.","Solicitudes RPA"); return 1; }
                try { Application.Run(new Ventana(new Registro(carpeta))); return 0; }
                catch(Exception ex) { MessageBox.Show("No se pudo abrir el registro. Conserve los datos y revise el error:\n" + ex.Message,"Solicitudes RPA"); return 1; }
            }
        }
    }
}
