namespace Envs_Manager
{
    public partial class Form1 : Form
    {
        public static List<string> ENVs { get; set; } = new();
        public static string envBasePath = @"C:\ENVs";



        public Form1()
        {
            InitializeComponent();
            LoadApps();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            LoadApps();
        }


        public void LoadApps()
        {
            ENVs.Clear();
            if (!System.IO.Directory.Exists(envBasePath))
            {
                System.IO.Directory.CreateDirectory(envBasePath);
            }

            ENVs.AddRange(System.IO.Directory.GetDirectories(envBasePath));

            listBox1.Items.Clear();
            listBox1.Items.AddRange(ENVs.Select(x => System.IO.Path.GetFileName(x)).ToArray());

        }

        public void LoadAppConfig(string envName)
        {
            comboBox1.Items.Clear();
            string envPath = System.IO.Path.Combine(envBasePath, envName);
            if (System.IO.Directory.Exists(envPath))
            {
                var parameters = System.IO.Directory.GetFiles(envPath);
                comboBox1.Items.AddRange(parameters.Select(x => System.IO.Path.GetFileNameWithoutExtension(x)).ToArray());
            }
        }



        public void AddApp(string appName)
        {
            if (!string.IsNullOrWhiteSpace(appName))
            {
                string newAppName = System.IO.Path.Combine(envBasePath, appName);
                if (!System.IO.Directory.Exists(newAppName))
                {
                    System.IO.Directory.CreateDirectory(newAppName);
                    LoadApps();
                }
                else
                {
                    MessageBox.Show("App already exists.");
                }
            }
        }




        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string newEnvName = Microsoft.VisualBasic.Interaction.InputBox("Enter new Application name:", "New Application", "");
            AddApp(newEnvName);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            string selectedEnv = "";
            if (listBox1.SelectedItems.Count != 1)
            {
                return;
            }
            selectedEnv = listBox1.SelectedItems[0].ToString();

            string newParamName = Microsoft.VisualBasic.Interaction.InputBox("Enter new App Config name:", "New Config", "");

            //AddNewParameter(listBox1.Text, newParamName, "123");




        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count != 1)
            {
                return;
            }

            string selectedApp = listBox1.SelectedItems[0].ToString();
            LoadAppConfig(selectedApp);
        }
    }
}
