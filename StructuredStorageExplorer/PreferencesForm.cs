﻿using StructuredStorageExplorer.Properties;

namespace StructuredStorageExplorer;

public partial class PreferencesForm : Form
{
    public PreferencesForm()
    {
        InitializeComponent();
    }

    private void BtnSavePreferences_Click(object sender, EventArgs e)
    {
        Settings.Default.EnableValidation = cbEnableValidation.Checked;
        Settings.Default.Save();
        DialogResult = DialogResult.OK;
        Close();
    }

    private void BtnCancelPreferences_Click(object sender, EventArgs e)
    {
        cbEnableValidation.Checked = Settings.Default.EnableValidation;
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void PreferencesForm_Load(object sender, EventArgs e)
    {
        cbEnableValidation.Checked = Settings.Default.EnableValidation;
    }
}
