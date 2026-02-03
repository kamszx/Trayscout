using System;
using System.Globalization;
using System.Windows.Forms;

namespace Trayscout
{
    public class SettingsForm : Form
    {
        private readonly Ini _ini;
        private readonly Action _onSaved;
        private TextBox _baseUrlTextBox;
        private TextBox _apiSecretTextBox;
        private TextBox _updateIntervalTextBox;
        private TextBox _highTextBox;
        private TextBox _lowTextBox;
        private ComboBox _unitComboBox;
        private CheckBox _useColorCheckBox;
        private CheckBox _useAlarmCheckBox;
        private TextBox _alarmIntervalTextBox;
        private ComboBox _styleComboBox;
        private TextBox _timeRangeTextBox;
        private TextBox _widthTextBox;
        private TextBox _heightTextBox;
        private TextBox _fontFamilyTextBox;
        private TextBox _fontSizeTextBox;

        public SettingsForm(Ini ini, Action onSaved)
        {
            _ini = ini;
            _onSaved = onSaved;

            Text = "Settings";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            TableLayoutPanel layout = new TableLayoutPanel
            {
                ColumnCount = 2,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12),
                Dock = DockStyle.Fill
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _baseUrlTextBox = new TextBox { Width = 280, Text = _ini.ReadString("Config", "BaseUrl") };
            _apiSecretTextBox = new TextBox { Width = 280, Text = _ini.ReadString("Config", "APISecret"), UseSystemPasswordChar = true };
            _updateIntervalTextBox = new TextBox { Width = 100, Text = _ini.ReadInt("Config", "UpdateInterval").ToString(CultureInfo.InvariantCulture) };
            _highTextBox = new TextBox { Width = 100, Text = _ini.ReadFloat("Config", "High").ToString(CultureInfo.InvariantCulture) };
            _lowTextBox = new TextBox { Width = 100, Text = _ini.ReadFloat("Config", "Low").ToString(CultureInfo.InvariantCulture) };
            _unitComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            _unitComboBox.Items.AddRange(Enum.GetNames(typeof(Unit)));
            _unitComboBox.SelectedItem = _ini.ReadEnum<Unit>("Config", "Unit").ToString();
            _useColorCheckBox = new CheckBox { Checked = _ini.ReadBool("Config", "UseColor") };
            _useAlarmCheckBox = new CheckBox { Checked = _ini.ReadBool("Config", "UseAlarm") };
            _alarmIntervalTextBox = new TextBox { Width = 100, Text = _ini.ReadInt("Config", "AlarmInterval").ToString(CultureInfo.InvariantCulture) };
            _styleComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            _styleComboBox.Items.AddRange(Enum.GetNames(typeof(StyleKey)));
            _styleComboBox.SelectedItem = _ini.ReadEnum<StyleKey>("Config", "Style").ToString();
            _timeRangeTextBox = new TextBox { Width = 100, Text = _ini.ReadInt("Config", "TimeRange").ToString(CultureInfo.InvariantCulture) };
            _widthTextBox = new TextBox { Width = 100, Text = _ini.ReadInt("Config", "Width").ToString(CultureInfo.InvariantCulture) };
            _heightTextBox = new TextBox { Width = 100, Text = _ini.ReadInt("Config", "Height").ToString(CultureInfo.InvariantCulture) };
            _fontFamilyTextBox = new TextBox { Width = 180, Text = _ini.ReadString("Config", "FontFamily") };
            _fontSizeTextBox = new TextBox { Width = 100, Text = _ini.ReadInt("Config", "FontSize").ToString(CultureInfo.InvariantCulture) };

            AddRow(layout, "BaseUrl", _baseUrlTextBox);
            AddRow(layout, "APISecret", _apiSecretTextBox);
            AddRow(layout, "UpdateInterval (min)", _updateIntervalTextBox);
            AddRow(layout, "High", _highTextBox);
            AddRow(layout, "Low", _lowTextBox);
            AddRow(layout, "Unit", _unitComboBox);
            AddRow(layout, "UseColor", _useColorCheckBox);
            AddRow(layout, "UseAlarm", _useAlarmCheckBox);
            AddRow(layout, "AlarmInterval (min)", _alarmIntervalTextBox);
            AddRow(layout, "Style", _styleComboBox);
            AddRow(layout, "TimeRange (h)", _timeRangeTextBox);
            AddRow(layout, "Width", _widthTextBox);
            AddRow(layout, "Height", _heightTextBox);
            AddRow(layout, "FontFamily", _fontFamilyTextBox);
            AddRow(layout, "FontSize", _fontSizeTextBox);

            FlowLayoutPanel buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            Button saveButton = new Button { Text = "Save", AutoSize = true };
            saveButton.Click += HandleSaveClick;
            Button cancelButton = new Button { Text = "Cancel", AutoSize = true };
            cancelButton.Click += (sender, args) => Close();

            buttons.Controls.Add(saveButton);
            buttons.Controls.Add(cancelButton);

            Controls.Add(layout);
            Controls.Add(buttons);
        }

        private void AddRow(TableLayoutPanel layout, string labelText, Control control)
        {
            int rowIndex = layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Label label = new Label
            {
                Text = labelText,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 6, 12, 6)
            };
            control.Anchor = AnchorStyles.Left;
            layout.Controls.Add(label, 0, rowIndex);
            layout.Controls.Add(control, 1, rowIndex);
        }

        private void HandleSaveClick(object sender, EventArgs e)
        {
            if (!TryParseInt(_updateIntervalTextBox.Text, "UpdateInterval", out int updateInterval) ||
                !TryParseFloat(_highTextBox.Text, "High", out float high) ||
                !TryParseFloat(_lowTextBox.Text, "Low", out float low) ||
                !TryParseInt(_alarmIntervalTextBox.Text, "AlarmInterval", out int alarmInterval) ||
                !TryParseInt(_timeRangeTextBox.Text, "TimeRange", out int timeRange) ||
                !TryParseInt(_widthTextBox.Text, "Width", out int width) ||
                !TryParseInt(_heightTextBox.Text, "Height", out int height) ||
                !TryParseInt(_fontSizeTextBox.Text, "FontSize", out int fontSize))
            {
                return;
            }

            if (_unitComboBox.SelectedItem == null || _styleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select Unit and Style values.", "Validation error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _ini.WriteString("Config", "BaseUrl", _baseUrlTextBox.Text.Trim());
            _ini.WriteString("Config", "APISecret", _apiSecretTextBox.Text.Trim());
            _ini.WriteInt("Config", "UpdateInterval", updateInterval);
            _ini.WriteFloat("Config", "High", high);
            _ini.WriteFloat("Config", "Low", low);
            _ini.WriteEnum("Config", "Unit", (Unit)Enum.Parse(typeof(Unit), _unitComboBox.SelectedItem.ToString()));
            _ini.WriteBool("Config", "UseColor", _useColorCheckBox.Checked);
            _ini.WriteBool("Config", "UseAlarm", _useAlarmCheckBox.Checked);
            _ini.WriteInt("Config", "AlarmInterval", alarmInterval);
            _ini.WriteEnum("Config", "Style", (StyleKey)Enum.Parse(typeof(StyleKey), _styleComboBox.SelectedItem.ToString()));
            _ini.WriteInt("Config", "TimeRange", timeRange);
            _ini.WriteInt("Config", "Width", width);
            _ini.WriteInt("Config", "Height", height);
            _ini.WriteString("Config", "FontFamily", _fontFamilyTextBox.Text.Trim());
            _ini.WriteInt("Config", "FontSize", fontSize);

            _onSaved?.Invoke();
            Close();
        }

        private bool TryParseInt(string value, string label, out int result)
        {
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
            {
                MessageBox.Show(label + " must be an integer.", "Validation error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private bool TryParseFloat(string value, string label, out float result)
        {
            if (!float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))
            {
                MessageBox.Show(label + " must be a number.", "Validation error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }
}
