﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace WindowsAppForOctopusDeployment
{
    public partial class Form1 : Form
    {
        public string user;
        public string baseurl = "https://octopus.ksg.int/api/";
        public string APIKey;
        public string endpoint;
        public string ChannelSelected;
        public string EnvironmentSelected;
        public string ReleaseSelected;
        public string VersionSelected;
        public string skip;
        public Form1()
        {
            InitializeComponent();
 

            //Reading details from User Config file.
            chkboxRememberMe.Checked = Properties.Settings.Default.RememberMe;
            ChkBoxRmbrCnfg.Checked = Properties.Settings.Default.SaveConfiguration;


            rtbConsole.Text =Environment.NewLine+Environment.NewLine+ "<<<<<--------Click On the Populate Button to get data in the Dropdown boxes-------->>>>>>";

            txtbxBDL.Text = "false";
            txtbxDSU.Text = "true";
            txtbxDBR.Text = "none";
            txtbxUGF.Text = "true";
            

            if (Properties.Settings.Default.RememberMe)
            {
                txtboxPassword.Text = Properties.Settings.Default.Password;
                APIKey = txtboxPassword.Text;
            }

            if (Properties.Settings.Default.SaveConfiguration)
            {
                cmbChannels.Text = Properties.Settings.Default.ChannelSelectedName;
                ChannelSelected = Properties.Settings.Default.ChannelSelectedVal;
                cmbEnvironments.Text = Properties.Settings.Default.EnvironmentName;
                EnvironmentSelected = Properties.Settings.Default.EnvironmentValue;
            }

            checkedListBox1.DisplayMember = "StepName";
            checkedListBox1.ValueMember = "Value";
            using (StreamReader sr = File.OpenText("StepDetails.txt"))
            {
                string line;
                int lineNumber = 0;
                while ((line = sr.ReadLine()) != null)
                
                {
                    string Name = line.Split(',')[1];
                    string Value = line.Split(',')[0];
                    checkedListBox1.Items.Insert(lineNumber, new SkipItems { StepName = Name, RealValue = Value });
                    lineNumber++;
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //      Adding FormClosing Event.
            FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        private void txtboxPassword_TextChanged(object sender, EventArgs e)
        {
            //      Saving user input given in the Textbox-  Password/API-Key
            Properties.Settings.Default.Password = txtboxPassword.Text;      
        }



        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        { 
            //Saving the user details on Form Close Event
            if (chkboxRememberMe.Checked)
            {
                Properties.Settings.Default.Password = txtboxPassword.Text;
                Properties.Settings.Default.RememberMe = true;
                Properties.Settings.Default.Save();
            }


                if (!chkboxRememberMe.Checked)
                {
                    Properties.Settings.Default.Password = null;
                    Properties.Settings.Default.RememberMe = false;
                    Properties.Settings.Default.Save();
                }

                if (ChkBoxRmbrCnfg.Checked)
                {
                    Properties.Settings.Default.SaveConfiguration = true;
                    Properties.Settings.Default.ChannelSelectedName = cmbChannels.Text.ToString();
                    Properties.Settings.Default.ChannelSelectedVal = ChannelSelected;
                    Properties.Settings.Default.EnvironmentName = cmbEnvironments.Text.ToString();
                    Properties.Settings.Default.EnvironmentValue = EnvironmentSelected;
                    Properties.Settings.Default.Save();
                }

            if (!ChkBoxRmbrCnfg.Checked)
            {
                Properties.Settings.Default.SaveConfiguration = false;
                Properties.Settings.Default.ChannelSelectedName =null;
                Properties.Settings.Default.ChannelSelectedVal = null;
                Properties.Settings.Default.EnvironmentName = null;
                Properties.Settings.Default.EnvironmentValue =null;
                Properties.Settings.Default.Save();
            }

        }
        // Toggle Password Visibility Functionality
        private void chkboxShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if(chkboxShowPassword.Checked)
            { txtboxPassword.UseSystemPasswordChar = false; }

            else if(!chkboxShowPassword.Checked)
            { txtboxPassword.UseSystemPasswordChar = true; }
        }

 //-------------------------------------------------------Channel Fetch Block------------------------------------------------------
        public void btnConnect_Click(object sender, EventArgs e)
        {
            APIKey = txtboxPassword.Text;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                endpoint = "channels/all";
                
                GetRequest Channel = new GetRequest();

                string responseFromServer = Channel.AllPopulate(APIKey, baseurl,endpoint);
                var Obj = JsonConvert.DeserializeObject<List<EnvironmentDetails.Item>>(responseFromServer);

                cmbChannels.DataBindings.Clear();
                cmbChannels.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbChannels.AutoCompleteSource = AutoCompleteSource.ListItems;

                cmbChannels.DataSource = Obj;
                cmbChannels.DisplayMember = "Name";
                cmbChannels.ValueMember = "ID";
                cmbChannels.Text = "--Select--";
                rtbConsole.Text = "Channels Populated Successfully!";
                ChannelSelected = null;
            }

            catch (WebException ex)
            {              
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        rtbConsole.Text = reader.ReadToEnd();
                    }                
            }
        }

//---------------------------------------------------Channel Select Block-------------------------------------------------------


        private void cbxChannelList_SelectedIndexChanged(object sender, EventArgs e)
        {
          ChannelSelected =cmbChannels.SelectedValue.ToString();
        }


//--------------------------------------------------Environment Populate Block---------------------------------------------------
        private void GetEnv_Click(object sender, EventArgs e)
        {
            APIKey = txtboxPassword.Text;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                endpoint = "environments/all";

                GetRequest Environment = new GetRequest();

                string responseFromServer = Environment.AllPopulate(APIKey,baseurl,endpoint);

                var Obj = JsonConvert.DeserializeObject<List<EnvironmentDetails.Item>>(responseFromServer);

                cmbEnvironments.DataBindings.Clear();
                cmbEnvironments.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbEnvironments.AutoCompleteSource = AutoCompleteSource.ListItems;

                cmbEnvironments.DataSource = Obj;
                cmbEnvironments.DisplayMember = "Name";
                cmbEnvironments.ValueMember = "Id";
                cmbEnvironments.Text = "--Select--";
                rtbConsole.Text = "Environments Populated Successfully!";
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    rtbConsole.Text = reader.ReadToEnd();
                }
            }
        }

 //--------------------------------------------Environment Select Block-------------------------------------------------------------------------

        private void cmbEnvironments_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnvironmentSelected = cmbEnvironments.SelectedValue.ToString();
        }

 //----------------------------------------------Release Populate Block Start------------------------------------------------------------------
        private void btnRelease_Click(object sender, EventArgs e)
        {
            if (ChannelSelected != null)
            {
                try

                {
                    Cursor.Current = Cursors.WaitCursor;

                    endpoint = "channels/" + ChannelSelected + "/releases";

                    GetRequest Release = new GetRequest();

                    string responseFromServer = Release.AllPopulate(APIKey, baseurl, endpoint);

                    ReleaseDetails.RootObject Obj = JsonConvert.DeserializeObject<ReleaseDetails.RootObject>(responseFromServer);
                    cmbRelease.DataBindings.Clear();
                    cmbRelease.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cmbRelease.AutoCompleteSource = AutoCompleteSource.ListItems;

                    cmbRelease.DataSource = Obj.Items;
                    cmbRelease.DisplayMember = "Version";
                    cmbRelease.ValueMember = "Id";
                    cmbRelease.Text = "--Select--";
                    rtbConsole.Text = "Releases Populated Successfully!";

                }
                catch (WebException ex)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        rtbConsole.Text = reader.ReadToEnd();
                    }
                }

            }
            else { rtbConsole.Text = "Please Enter a Valid Channel"; }
        }


//--------------------------------------------------------------------Select Release Block-----------------------------------------------------
        private void cmbRelease_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReleaseSelected = cmbRelease.SelectedValue.ToString();
            
        }

//------------------------------------------------------------Create Release Block-----------------------------------------------------

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (ChannelSelected != null)
            {
                Cursor.Current = Cursors.WaitCursor;
                VersionSelected = txtbxPackageVersion.Text;
                try
                {

                    endpoint = "releases";
                    ServicePointManager.ServerCertificateValidationCallback = (object a, System.Security.Cryptography.X509Certificates.X509Certificate b, System.Security.Cryptography.X509Certificates.X509Chain c, System.Net.Security.SslPolicyErrors d) => { return true; };
                    WebRequest req = WebRequest.Create(baseurl + endpoint);
                    req.Method = "POST";
                    req.Headers["X-Octopus-ApiKey"] = APIKey;
                    req.ContentType = "application/json";

                    if (ChannelSelected == "Channels-621")
                    {
                        string json = File.ReadAllText("CreateReleaseTemplateChannelNetStat.json");

                       /* var obj= JObject.Parse(json);
                        var array = obj.GetValue("SelectedPackages") as JArray;
                        var newMember ="{"+"ActionName"+":" +"CleanupAutomation"+","+"Version"+":"+ ""+"}";
                        var add = JObject.Parse(newMember);
                        array.Add(add);*/


                        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                        int i = 0;
                        foreach (JObject step in jsonObj["SelectedPackages"])

                        {
                            jsonObj["SelectedPackages"][i]["Version"] = VersionSelected;
                            i++;
                        }

                        jsonObj["ChannelId"] = ChannelSelected;
                        jsonObj["Version"] = txtbxReleaseName.Text;


                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

                        File.WriteAllText("CreateReleasePostData.json", output);
                        string json1 = File.ReadAllText("CreateReleasePostData.json");

                        rtbConsole.Text = "Finding Template and Creating Release!";
                        using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                        {
                            streamWriter.Write(json1);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                    }

                    else
                    {
                        string json = File.ReadAllText("CreateReleaseTemplate.json");

                        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                        int i = 0;
                        foreach (JObject step in jsonObj["SelectedPackages"])

                        {
                            jsonObj["SelectedPackages"][i]["Version"] = VersionSelected;
                            i++;
                        }
                        jsonObj["ChannelId"] = ChannelSelected;
                        jsonObj["Version"] = txtbxReleaseName.Text;

                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

                        File.WriteAllText("CreateReleasePostData.json", output);
                        string json1 = File.ReadAllText("CreateReleasePostData.json");

                        rtbConsole.Text = "Finding Template and Creating Release!";
                        using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                        {
                            streamWriter.Write(json1);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                    }
                    HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

                    StreamReader reader = new StreamReader(resp.GetResponseStream());
                    
                    rtbConsole.Text = Environment.NewLine + " Release Created Successfully!" +Environment.NewLine;
                    rtbConsole.Text += Environment.NewLine + reader.ReadToEnd();
                }
                catch (WebException ex)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        rtbConsole.Text = reader.ReadToEnd();

                    }
                }
            }
            else
            {
                rtbConsole.Text = "Please Selecet a Valid Channel!";
            }

            }
//--------------------------------------------------------------Deploy Release Block------------------------------------------------------
        private void btnDeploy_Click(object sender, EventArgs e)
        {
            if (ReleaseSelected != null & EnvironmentSelected!=null)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    endpoint = "deployments";
                    ServicePointManager.ServerCertificateValidationCallback = (object a, System.Security.Cryptography.X509Certificates.X509Certificate b, System.Security.Cryptography.X509Certificates.X509Chain c, System.Net.Security.SslPolicyErrors d) => { return true; };
                    WebRequest req = WebRequest.Create(baseurl + endpoint);
                    req.Method = "POST";
                    req.Headers["X-Octopus-ApiKey"] = APIKey;
                    req.ContentType = "application/json";
                    string json = File.ReadAllText("DeployReleaseTemplate.json");
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    jsonObj["FormValues"]["290d3b75-9591-faa9-13c3-14c06ff0bed5"] = txtbxBDL.Text.ToLower();
                    jsonObj["FormValues"]["9834a393-a509-8cde-68cb-affb5cae972e"] = txtbxDSU.Text.ToLower();
                    jsonObj["FormValues"]["f910eadf-b6e3-25ff-a9b4-24ae30a2e502"] = txtbxDBR.Text.ToLower();
                    jsonObj["ChannelId"] = ChannelSelected;
                    jsonObj["UseGuidedFailure"] = txtbxUGF.Text.ToLower();
                    jsonObj["ReleaseId"] = ReleaseSelected;
                    jsonObj["EnvironmentId"] = EnvironmentSelected;

                    List<string> steps = new List<string>();

                    foreach (SkipItems skipitem in checkedListBox1.CheckedItems)

                    {
                        steps.Add(skipitem.RealValue);
                    }
                    string skip = "[" + "\"" + string.Join("\"" + "," + "\"", steps) + "\"" + "]";

                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText("DeployReleasePostData.json", output);

                    if (checkedListBox1.CheckedItems.Count!=0)
                    {
                        string json1 = File.ReadAllText("DeployReleasePostData.json").TrimEnd('}') + "," + "\"SkipActions\"" + ":" + skip + "}";

                        using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                        {
                            streamWriter.Write(json1);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                        HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

                        StreamReader reader = new StreamReader(resp.GetResponseStream());

                        rtbConsole.Text = Environment.NewLine + "Deployment Started Successfully!" + Environment.NewLine + Environment.NewLine;
                        rtbConsole.Text += reader.ReadToEnd();
                        ProcessStartInfo sInfo = new ProcessStartInfo("https://octopus.ksg.int/app#/");
                        Process.Start(sInfo);
                    }
                
                    else
                    {
                        string json1 = File.ReadAllText("DeployReleasePostData.json");

                        rtbConsole.Text = json1;
                        using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                        {
                            streamWriter.Write(json1);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                        HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

                        StreamReader reader = new StreamReader(resp.GetResponseStream());

                        rtbConsole.Text = Environment.NewLine + "Deployment Started Successfully!" + Environment.NewLine + Environment.NewLine;
                        rtbConsole.Text += reader.ReadToEnd();
                        ProcessStartInfo sInfo = new ProcessStartInfo("https://octopus.ksg.int/app#/");
                        Process.Start(sInfo);
                    }
                }
                catch (WebException ex)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        rtbConsole.Text = reader.ReadToEnd();

                    }
                }
            }
            else
            {
                rtbConsole.Text = "Select a Valid Release Version and Environment for Deployment!";
            }


        }

        private void chkbxSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbxSelectAll.Checked)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
            }
        }


        //---------------------------------------------------------------------------------------------------------------

    }
}

