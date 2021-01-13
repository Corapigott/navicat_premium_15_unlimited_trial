using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NavicatPremium15UT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool IsRegeditItemExist()
        {
            string[] subkeyNames;
            RegistryKey key = Registry.CurrentUser;
            RegistryKey software = key.OpenSubKey("SOFTWARE");
            subkeyNames = software.GetSubKeyNames();
            foreach (string keyName in subkeyNames)
            {
                if (keyName == "PremiumSoft")
                {
                    key.Close();
                    return true;
                }
            }
            key.Close();
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsRegeditItemExist())
            {
                MessageBox.Show("Premium Soft Not Found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            RegistryKey key = Registry.CurrentUser;
            RegistryKey software = key.OpenSubKey("SOFTWARE", true);
            RegistryKey PremiumSoft = software.OpenSubKey("PremiumSoft", true);
            RegistryKey NavicatPremium = PremiumSoft.OpenSubKey("NavicatPremium", true);
            if (NavicatPremium == null)
            {
                key.Close();
                MessageBox.Show("Navicat Premium Not Found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            PremiumSoft.DeleteSubKeyTree("NavicatPremium");

            RegistryKey Classes = software.OpenSubKey("Classes", true);
            RegistryKey CLSID = Classes.OpenSubKey("CLSID", true);
            String[] CLSID_SubKeys = CLSID.GetSubKeyNames();
            foreach (string keyName in CLSID_SubKeys)
            {
                RegistryKey SubKey = CLSID.OpenSubKey(keyName, true);
                string[] sks = SubKey.GetSubKeyNames();
                if (sks.Length == 1)
                {
                    Console.WriteLine(sks[0]);
                    if(sks[0].ToLower() == "info")
                    {
                        RegistryKey info = SubKey.OpenSubKey("Info");
                        string[] vals = info.GetValueNames();
                        if(vals.Length == 1)
                        {
                            bool deleteSucc = false;
                            foreach (string val in vals)
                            {
                                Console.WriteLine(val);
                                string value = Convert.ToString(info.GetValue(vals[0], ""));
                                if(vals[0].Length == 32 && value.Length == 32)
                                {
                                    SubKey.DeleteSubKeyTree("Info");
                                    deleteSucc = true;
                                    break;
                                }
                            }
                            if (deleteSucc)
                            {
                                break;
                            }
                        }                        
                    }
                }
            }
            key.Close();

            MessageBox.Show("Navicat Premium 15 the trial period has been reset", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Application.Exit();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Corapigott/navicat_premium_15_unlimited_trial");
        }
    }
}
