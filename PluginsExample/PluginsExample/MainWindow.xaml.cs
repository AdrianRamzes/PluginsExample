using PluginInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PluginsExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Color _currentColor = Colors.Black;
        private int _currentThickness = 3;
        private IPlugin _currentActivePlugin;

        public MainWindow()
        {
            InitializeComponent();
            InitializePlugins();
            this.Closing += MainWindow_Closing;
        }

        private void InitializePlugins()
        {
            var assemblies = GetAssemblies("Plugins");

            List<MenuItem> menuItems = new List<MenuItem>();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass && type.IsPublic && type.GetInterface(typeof(IPlugin).FullName) != null)
                    {
                        var item = Activator.CreateInstance(type) as IPlugin;
                        var menuItem = item.GetMenuItem();
                        menuItem.Tag = item;
                        menuItem.Click += pluginMenuItem_Click;
                        menuItems.Add(menuItem);
                    }
                }
            }

            if (menuItems.Any())
            {
                var tools = new MenuItem();
                tools.Header = "Tools";
                menuItems.ForEach((i) => { tools.Items.Add(i); });

                v_Menu.Items.Add(tools);
            }
        }

        private void pluginMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem == null)
                return;
            
            if (_currentActivePlugin != null)
                _currentActivePlugin.Dispose();

            IPlugin plugin = menuItem.Tag as IPlugin;
            if (plugin != null)
            {
                _currentActivePlugin = plugin;
                _currentActivePlugin.Initialize(v_Canvas, _currentColor, _currentThickness);
            }
        }

        private List<Assembly> GetAssemblies(string directory)
        {
            var assemblies = new List<Assembly>();
            if (Directory.Exists(directory))
            {
                foreach (var file in Directory.GetFiles(directory, "*.dll"))
                {
                    assemblies.Add(Assembly.LoadFrom(file));
                }
            }
            return assemblies;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_currentActivePlugin != null)
            {
                _currentActivePlugin.Dispose();
            }
        }
    }
}
